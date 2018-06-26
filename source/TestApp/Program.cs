using System;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var startIdx = 0;

            Console.WriteLine($"Number of args :{args.Length}.");

            Console.WriteLine("What to do !");

            if (args.Length == 0)
            {
                Console.WriteLine("blinky {PinId}");
                Console.WriteLine("fade {PinId} {ms from min to max}");
                Console.WriteLine("range {TriggerPinId} {EchoPinId} {sec between meassurments}");
            }
            else
            {
                switch (args[startIdx + 1].ToLower())
                {
                    case "blinky":
                        CallBlinky(args[startIdx + 2]);
                        break;
                    case "fade":
                        CallFadeInNOut(args[startIdx + 2], args[startIdx + 3]);
                        break;
                    case "range":
                        CallRangefinder(args[startIdx + 2], args[startIdx + 3], args[startIdx + 4]);
                        break;
                    default:
                        Console.WriteLine("blinky {PinId}");
                        Console.WriteLine("fade {PinId} {ms from min to max}");
                        Console.WriteLine("range {TriggerPinId} {EchoPinId} {sec between meassurments}");
                        break;
                }
            }
            Console.Write("Press enter to quit.");
            Console.ReadLine();
        }

        public static void CallBlinky(string pinId)
        {
            Console.WriteLine($"Running blinky on pin {pinId}.");
            var pinIdInt = int.Parse(pinId);

            var pin = Pi.Gpio.GetGpioPinByBcmPinNumber(pinIdInt);
            pin.PinMode = GpioPinDriveMode.Output;
            do
            {
                pin.Write(true);
                Thread.Sleep(500);
                pin.Write(false);
                Thread.Sleep(500);
            } while (true);
        }

        public static void CallFadeInNOut(string pinId, string msMinToMax)
        {
            Console.WriteLine($"Running fade in and out on pin {pinId}. Time from min to max {msMinToMax} ms");
            var maxRange = 100;
            var pinIdInt = int.Parse(pinId);
            var msStep = int.Parse(msMinToMax) / maxRange;

            var pin = Pi.Gpio.GetGpioPinByBcmPinNumber(pinIdInt);
            pin.PinMode = GpioPinDriveMode.Output;

            if (pin.IsInSoftPwmMode == false)
            {
                pin.StartSoftPwm(0, maxRange);
            }

            do
            {
                // turn brighter each step
                for (int i = 0; i <= 5; i++)
                {
                    pin.SoftPwmValue = i * 20;
                    Thread.Sleep(msStep);
                }

                // turn darker each step
                for (int i = 10; i >= 0; i--)
                {
                    pin.SoftPwmValue = i * 10;
                    Thread.Sleep(msStep);
                }
            } while (true);
        }

        public static void CallRangefinder(string triggerPin, string echoPin, string secondsBetweenMeassurements)
        {
            var rf = new Rangefinder(Convert.ToInt32(triggerPin), Convert.ToInt32(echoPin));
            rf.Start(Convert.ToInt32(secondsBetweenMeassurements));
        }
    }
}
