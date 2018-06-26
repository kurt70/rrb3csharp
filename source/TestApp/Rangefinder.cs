using System;
using System.Collections.Generic;
using System.Text;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace TestApp
{
    public class Rangefinder
    {
        private GpioPin _triggerPin;
        private GpioPin _echoPin;

        public Rangefinder(int triggerPinId, int echoPinId)
        {
            _triggerPin = Pi.Gpio.GetGpioPinByBcmPinNumber(triggerPinId);
            _triggerPin.PinMode = GpioPinDriveMode.Output;
            _echoPin = Pi.Gpio.GetGpioPinByBcmPinNumber(echoPinId);
            _echoPin.PinMode = GpioPinDriveMode.Input;
        }

        public void Start(int secondsTimeBetweenMeasurements)
        {
            do
            {
                var cm = GetDistance();
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}\t{cm.ToString("F1")} cm.");
                Sleep(secondsTimeBetweenMeasurements);
            } while (true);
        }

        public void SendTriggerPulse()
        {
            _triggerPin.Write(true);
            Sleep(0.0001f);
            _triggerPin.Write(false);
        }

        private static void Sleep(double seconds)
        {
            //0.00001
            uint us = Convert.ToUInt32(seconds * 1000000);
            Pi.Timing.SleepMicroseconds(us);
        }

        public void WaitForEcho(bool value, int timeoutInUs)
        {
            var count = timeoutInUs;
            while (_echoPin.Read() != value && count > 0)
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
            var distance_cm = pulse_len / 6.6f;
            return distance_cm;
        }
    }
}
