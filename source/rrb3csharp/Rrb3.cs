using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace rrb3csharp
{
    public class Rrb3
    {
        public int MOTOR_DELAY = 200;//ms
        public int RIGHT_PWM_PIN = 14;
        public int RIGHT_1_PIN = 10;
        public int RIGHT_2_PIN = 25;
        public int LEFT_PWM_PIN = 24;
        public int LEFT_1_PIN = 17;
        public int LEFT_2_PIN = 4;
        public int SW1_PIN = 11;
        public int SW2_PIN = 9;
        public int LED1_PIN = 8;
        public int LED2_PIN = 7;
        public int OC1_PIN = 22;
        public int OC2_PIN = 27;
        public int OC2_PIN_R1 = 21;
        public int OC2_PIN_R2 = 27;
        public int TRIGGER_PIN = 18;
        public int ECHO_PIN = 23;
        public GpioPin left_pwm;
        public GpioPin right_pwm;
        public float pwm_scale = 0;

        public int old_left_dir = -1;
        public int old_right_dir = -1;

        public Rrb3() : this(9.0f, 6.0f, 2)
        { }

        public Rrb3(float battery_voltage, float motor_voltage, int revision)
        {
            this.pwm_scale = motor_voltage / battery_voltage;

            if (this.pwm_scale > 1)
            {
                Console.WriteLine("WARNING: Motor voltage is higher than battery vatage. Motor may run slow.");
            }

            left_pwm = InitPwmOutputPin(LEFT_PWM_PIN, 500);
            left_pwm.StartSoftPwm(500, 1000);

            right_pwm = InitPwmOutputPin(RIGHT_PWM_PIN, 500);

            InitPin(RIGHT_1_PIN, GpioPinDriveMode.Output);
            InitPin(RIGHT_2_PIN, GpioPinDriveMode.Output);
            
            InitPin(LED1_PIN, GpioPinDriveMode.Output);
            InitPin(LED2_PIN, GpioPinDriveMode.Output);

            InitPin(OC1_PIN, GpioPinDriveMode.Output);

            if (revision == 1)
            {
                OC2_PIN = OC2_PIN_R1;
            }
            else
            {
                OC2_PIN = OC2_PIN_R2;
            }

            InitPin(OC2_PIN_R2, GpioPinDriveMode.Output);

            InitPin(SW1_PIN, GpioPinDriveMode.Input);
            InitPin(SW2_PIN, GpioPinDriveMode.Input);
            InitPin(TRIGGER_PIN, GpioPinDriveMode.Input);
            InitPin(ECHO_PIN, GpioPinDriveMode.Input);
        }
        public void set_motors(decimal left_pwm, int left_dir, decimal right_pwm, int right_dir)
        {
            if (old_left_dir != left_dir || old_right_dir != right_dir)
            {
                set_driver_pins(0, 0, 0, 0);
                # stop motors between sudden changes of direction
                Sleep(MOTOR_DELAY);
            }

            set_driver_pins(left_pwm, left_dir, right_pwm, right_dir);
            old_left_dir = left_dir;
            old_right_dir = right_dir;

        }

        public void set_driver_pins(decimal left_pwm, int left_dir, decimal right_pwm, int right_dir)
        {

            //left_pwm.ChangeDutyCycle(left_pwm* 100 * pwm_scale)
            SetPinValue(LEFT_1_PIN, left_dir);
            SetPinValue(LEFT_2_PIN, not left_dir);
            //right_pwm.ChangeDutyCycle(right_pwm * 100 * pwm_scale);
            SetPinValue(RIGHT_1_PIN, right_dir);
            SetPinValue(RIGHT_2_PIN, right_dir);
        }

        public void forward(int seconds = 0, decimal speed = 1.0m)
        {
            set_motors(speed, 0, speed, 0);
            if (seconds > 0)
            {
                Sleep(seconds * 1000);
                Stop();
            }
        }

        public void Stop()
        {
            set_motors(0, 0, 0, 0);
        }

        public void Reverse(int seconds = 0, decimal speed = 1.0m)
        {
            set_motors(speed, 1, speed, 1);
            if (seconds > 0)
            {
                Sleep(seconds);
                Stop();
            }
        }

        public void Left(int seconds = 0, decimal speed = 0.5m)
        {
            set_motors(speed, 0, speed, 1);
            if (seconds > 0)
            {
                Sleep(seconds);
                Stop();
            }
        }

        public void Right(int seconds = 0, decimal speed = 0.5m)
        {
            set_motors(speed, 1, speed, 0);
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
                set_driver_pins(1, 1, 1, 0);
                Sleep(delay);
                set_driver_pins(1, 1, 1, 1);
                Sleep(delay);
                set_driver_pins(1, 0, 1, 1);
                Sleep(delay);
                set_driver_pins(1, 0, 1, 0);
                Sleep(delay);}

            set_driver_pins(0, 0, 0, 0);
        }

        //def step_reverse(self, delay, num_steps):
        //    for i in range(0, num_steps) :
        //        self.set_driver_pins(1, 0, 1, 0)
        //        time.sleep(delay)
        //        self.set_driver_pins(1, 0, 1, 1)
        //        time.sleep(delay)
        //        self.set_driver_pins(1, 1, 1, 1)
        //        time.sleep(delay)
        //        self.set_driver_pins(1, 1, 1, 0)
        //        time.sleep(delay)
        //    self.set_driver_pins(0, 0, 0, 0)

        //def sw1_closed(self):
        //    return not GPIO.input(self.SW1_PIN)

        //def sw2_closed(self):
        //    return not GPIO.input(self.SW2_PIN)

        //def set_led1(self, state):
        //    GPIO.output(self.LED1_PIN, state)

        //def set_led2(self, state) :
        //    GPIO.output(self.LED2_PIN, state)

        //def set_oc1(self, state) :
        //    GPIO.output(self.OC1_PIN, state)

        //def set_oc2(self, state) :
        //    GPIO.output(self.OC2_PIN, state)

        public void Send_trigger_pulse()
        {
            SetPinValue(TRIGGER_PIN, true);
            Sleep(0.0001m);
            SetPinValue(TRIGGER_PIN, false);
        }

        public void Wait_for_echo(bool value, int timeoutInUs)
        {
            var count = timeoutInUs;
            while (ReadPinValue(ECHO_PIN) != value && count > 0)
            {
                count -= 1;
            }
        }

        public double Get_distance()
        {
            Send_trigger_pulse();
            Wait_for_echo(true, 10000);
            var start = DateTime.UtcNow;
            Wait_for_echo(false, 10000);
            var finish = DateTime.UtcNow;
            var pulse_len = (finish - start).TotalMilliseconds;
            var distance_cm = pulse_len / 0.000058;
            return distance_cm;
        }

        public void cleanup()
        {

        }//    GPIO.cleanup()


        private GpioPin InitPin(int pinId, GpioPinDriveMode mode)
        {

            var pin = Pi.Gpio.GetGpioPinByBcmPinNumber(pinId);
            pin.PinMode = mode;

            return pin;
        }

        private GpioPin InitPwmOutputPin(int pinId, int initialValue)
        {
            var pin = GpioController.Instance.GetGpioPinByBcmPinNumber(pinId);
            pin.PinMode = GpioPinDriveMode.PwmOutput;
            if (!pin.IsInSoftPwmMode)
            {
                pin.StartSoftPwm(pinId, initialValue);
            }
            pin.SoftPwmValue = initialValue;

            return pin;
        }

        private void SetPinValue(int pinId, int initialValue)
        {
            var pin = GpioController.Instance.GetGpioPinByBcmPinNumber(pinId);
            if (pin.PinMode != GpioPinDriveMode.PwmOutput)
            {
                throw new Exception("Pin must me initialized as a PWM Output pin.");
            }

            pin.SoftPwmValue = initialValue;
        }

        private void SetPinValue(int pinId, bool state)
        {
            var pin = GpioController.Instance.GetGpioPinByBcmPinNumber(pinId);
            if (pin.PinMode != GpioPinDriveMode.Output)
            {
                throw new Exception("Pin must me initialized as a Output pin.");
            }

            pin.Write(state);
        }

        private bool ReadPinValue(int pinId)
        {
            var pin = GpioController.Instance.GetGpioPinByBcmPinNumber(pinId);
            return pin.Read();
        }

        private static void Sleep(decimal seconds)
        {
            uint us = Convert.ToUInt16(seconds * 1000000);
            Pi.Timing.SleepMicroseconds(us);
        }
    }
}