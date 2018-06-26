using System;
using System.Reflection;
using System.Threading;

namespace Rover
{
    class Program
    {
        static void Main(string[] args)
        {
            var welcome = $"RobotRover v '{Assembly.GetExecutingAssembly().FullName}.'";
            Console.WriteLine(welcome);
            Console.WriteLine(string.Empty.PadLeft(welcome.Length, '-'));
            var startIdx = 0;

            if (args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("\tBasic");
            }
            else
            {
                Console.WriteLine($"Running : {args[startIdx + 1].ToLower()}");
                switch (args[startIdx + 1].ToLower())
                {
                    case "Basic":
                        CallBasic();
                        break;
                    default:
                        Console.WriteLine("No program specified.");
                        break;
                }
            }
            Console.Write("Press enter to quit.");
            Console.ReadLine();
        }

        private static void TurnRandom(RRB3CSharp.RRB3CSharp rr)
        {
            var rnd = new Random(DateTime.UtcNow.Millisecond);
            var turnTime = rnd.Next(1, 3);
            if (rnd.Next(1, 2) == 1)
            {
                rr.Left(turnTime, 0.5f);// turn at half speed
            }
            else
            {
                rr.Right(turnTime, 0.5f);
            }
            rr.Stop();
        }

        private static void CallBasic()
        {
            var batteryVoltage = 9f;
            var motorVoltage = 6f;
            var revision = 2;

            var rr = new RRB3CSharp.RRB3CSharp(batteryVoltage, motorVoltage, revision);

            var running = false;

            try
            {
                do
                {
                    var distance = rr.GetDistance();
                    Console.WriteLine(distance);
                    if (distance < 50 && running)
                    {
                        TurnRandom(rr);
                    }
                    if (running)
                    {
                        rr.Forward(0);
                    }
                    Thread.Sleep(200);
                } while (true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
