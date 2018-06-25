namespace RRB3CSharp
{
    using System;
    using Unosquare.RaspberryIO;
    using Unosquare.RaspberryIO.Gpio;

    public enum Direction
    {
        Uninitialized = -1,
        Forward = 0,
        Reverse = 1
    }

    public class RRB3CSharp
    {
        public int MotorDelay = 200;//ms

        public int RightPwmPinId = 14;
        public int Right1PinId = 10;
        public int Right2PinId = 25;
        public int LeftPwmPinId = 24;
        public int Left1PinId = 17;
        public int Left2PinId = 4;
        public int Sw1PinId = 11;
        public int Sw2PinId = 9;
        public int LED1PinId = 8;
        public int LED2PinId = 7;
        public int OC1PinId = 22;
        public int OC2PinId = 27;
        public int OC2PinIdR1 = 21;
        public int OC2PinIdR2 = 27;
        public int TriggerPinId = 18;
        public int EchoPinId = 23;

        public GpioPin LeftPwmPin;
        public GpioPin RightPwmPin;

        public float PwmScale = 0f;

        public Direction OldLeftDir = Direction.Uninitialized;
        public Direction OldRightDir = Direction.Uninitialized;

        private GpioPin LeftPin1 = null;
        private GpioPin LeftPin2 = null;
        private GpioPin RightPin1 = null;
        private GpioPin RightPin2 = null;
        private GpioPin LedPin1 = null;
        private GpioPin LedPin2 = null;
        private GpioPin TriggerPin = null;
        private GpioPin EchoPin = null;

        public RRB3CSharp()
            : this(9.0f, 6.0f, 2)
        { }

        public RRB3CSharp(float batteryVoltage, float motorVoltage, int revision)
        {
            PwmScale = motorVoltage / batteryVoltage;

            if (this.PwmScale > 1)
            {
                Console.WriteLine("WARNING: Motor voltage is higher than battery vatage. Motor may run slow.");
            }

            LeftPwmPin = InitPwmOutputPin(LeftPwmPinId, 500);
            RightPwmPin = InitPwmOutputPin(RightPwmPinId, 500);

            LeftPin1 = InitPin(Left1PinId, GpioPinDriveMode.Output);
            LeftPin2 = InitPin(Left2PinId, GpioPinDriveMode.Output);

            RightPin1 = InitPin(Right1PinId, GpioPinDriveMode.Output);
            RightPin2 = InitPin(Right2PinId, GpioPinDriveMode.Output);

            LedPin1 = InitPin(LED1PinId, GpioPinDriveMode.Output);
            LedPin2 = InitPin(LED2PinId, GpioPinDriveMode.Output);

            InitPin(OC1PinId, GpioPinDriveMode.Output);

            if (revision == 1)
            {
                OC2PinId = OC2PinIdR1;
            }
            else
            {
                OC2PinId = OC2PinIdR2;
            }

            InitPin(OC2PinIdR2, GpioPinDriveMode.Output);

            InitPin(Sw1PinId, GpioPinDriveMode.Input);
            InitPin(Sw2PinId, GpioPinDriveMode.Input);
            TriggerPin = InitPin(TriggerPinId, GpioPinDriveMode.Output);
            EchoPin = InitPin(EchoPinId, GpioPinDriveMode.Input);
        }

        public void SetMotors(float leftPwmLevel, Direction left, float rightPwmLevel, Direction right)
        {
            if (OldLeftDir != left || OldRightDir != right)
            {
                SetDriverPins(0, Direction.Forward, 0, Direction.Forward);
                // stop motors between sudden changes of direction
                Sleep(MotorDelay);
            }

            SetDriverPins(leftPwmLevel, left, rightPwmLevel, right);
            OldLeftDir = left;
            OldRightDir = right;
        }

        public void SetDriverPins(float leftPwmLevel, Direction leftDirection, float rightPwmLevel, Direction rightDirection)
        {
            int ll = Convert.ToInt32( leftPwmLevel * 100 * PwmScale);
            int rl = Convert.ToInt32(rightPwmLevel * 100 * PwmScale);

            SetPinValue(LeftPwmPin, ll);
            SetPinValue(RightPwmPin, ll);

            SetPinValue(LeftPin1, (int)leftDirection);
            var notDir = leftDirection == Direction.Forward ? Direction.Reverse : Direction.Forward;
            SetPinValue(LeftPin2, (int)notDir);
            
            SetPinValue(RightPin1, (int)rightDirection);
            SetPinValue(RightPin2, (int)rightDirection);
        }

        public void Forward(int seconds = 0, float speed = 1.0f)
        {
            SetMotors(speed, 0, speed, 0);
            if (seconds > 0)
            {
                Sleep(seconds * 1000);
                Stop();
            }
        }

        public void Stop()
        {
            SetMotors(0, 0, 0, 0);
        }

        public void Reverse(int seconds = 0, float speed = 1.0f)
        {
            SetMotors(speed, Direction.Reverse, speed, Direction.Reverse);
            if (seconds > 0)
            {
                Sleep(seconds);
                Stop();
            }
        }

        public void Left(int seconds = 0, float speed = 0.5f)
        {
            SetMotors(speed, Direction.Forward, speed, Direction.Reverse);
            if (seconds > 0)
            {
                Sleep(seconds);
                Stop();
            }
        }

        public void Right(int seconds = 0, float speed = 0.5f)
        {
            SetMotors(speed, Direction.Reverse, speed, Direction.Forward);
            if (seconds > 0)
            {
                Sleep(seconds);
                Stop();
            }
        }

        public void StepForward(int delay, int num_steps)
        {
            for (int i = 0; i < num_steps; i++)
            {
                SetDriverPins(1, Direction.Reverse, 1, Direction.Forward);
                Sleep(delay);
                SetDriverPins(1, Direction.Reverse, 1, Direction.Reverse);
                Sleep(delay);
                SetDriverPins(1, Direction.Forward, 1, Direction.Reverse);
                Sleep(delay);
                SetDriverPins(1, Direction.Forward, 1, Direction.Forward);
                Sleep(delay);}

            SetDriverPins(0, 0, 0, 0);
        }

        public void SendTriggerPulse()
        {
            SetPinValue(TriggerPin, true);
            Sleep(0.0001m);
            SetPinValue(TriggerPin, false);
        }

        public void WaitForEcho(bool value, int timeoutInUs)
        {
            var count = timeoutInUs;
            while (ReadPinValue(EchoPin) != value && count > 0)
            {
                count -= 1;
            }
        }

        public double GetDistance()
        {
            SendTriggerPulse();
            WaitForEcho(true, 10000);
            var start = DateTime.UtcNow;
            WaitForEcho(false, 10000);
            var finish = DateTime.UtcNow;
            var pulse_len = (finish - start).TotalMilliseconds;
            var distance_cm = pulse_len / 0.000058;
            return distance_cm;
        }

        private GpioPin InitPin(int pinId, GpioPinDriveMode mode)
        {
            var pin = Pi.Gpio.GetGpioPinByBcmPinNumber(pinId);
            pin.PinMode = mode;

            return pin;
        }

        private GpioPin InitPwmOutputPin(int pinId, int initialValue)
        {
            var pin = GpioController.Instance.GetGpioPinByBcmPinNumber(pinId);
            pin.PinMode = GpioPinDriveMode.Output;
            if (!pin.IsInSoftPwmMode)
            {
                pin.StartSoftPwm(pinId, initialValue);
            }
            else
            {
                pin.SoftPwmValue = initialValue;
            }

            return pin;
        }

        private void SetPinValue(GpioPin pin, int value)
        {
            pin.SoftPwmValue = value;
        }

        private void SetPinValue(GpioPin pin, bool state)
        {
            pin.Write(state);
        }

        private bool ReadPinValue(GpioPin pin)
        {            
            return pin.Read();
        }

        private static void Sleep(decimal seconds)
        {
            uint us = Convert.ToUInt16(seconds * 1000000);
            Pi.Timing.SleepMicroseconds(us);
        }
    }
}