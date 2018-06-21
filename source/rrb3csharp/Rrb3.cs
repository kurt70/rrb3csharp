using System;
using System.Collections.Generic;
using System.Text;
using Unosquare.RaspberryIO.Gpio;

namespace rrb3csharp
{
    public class Rrb3
    {
        public decimal MOTOR_DELAY = 0.2m;
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

            //    GPIO.setmode(GPIO.BCM)
            //    GPIO.setwarnings(False)
            //    GPIO.setup(self.LEFT_PWM_PIN, GPIO.OUT)
            //    left_pwm = GPIO.PWM(self.LEFT_PWM_PIN, 500)
            //    self.left_pwm.start(0)

            left_pwm = InitPwmOutputPin( LEFT_PWM_PIN, 500);
            left_pwm.StartSoftPwm(500, 1000);
            
            //    GPIO.setup(self.LEFT_1_PIN, GPIO.OUT)
            //    GPIO.setup(self.LEFT_2_PIN, GPIO.OUT)

            //    GPIO.setup(self.RIGHT_PWM_PIN, GPIO.OUT)
            //    self.right_pwm = GPIO.PWM(self.RIGHT_PWM_PIN, 500)
            //    self.right_pwm.start(0)

            right_pwm = InitPwmOutputPin(RIGHT_PWM_PIN, 500);

            //    GPIO.setup(self.RIGHT_1_PIN, GPIO.OUT)
            //    GPIO.setup(self.RIGHT_2_PIN, GPIO.OUT)
            InitPin(RIGHT_1_PIN, GpioPinDriveMode.Output);
            InitPin(RIGHT_2_PIN, GpioPinDriveMode.Output);

            //    GPIO.setup(self.LED1_PIN, GPIO.OUT)
            //    GPIO.setup(self.LED2_PIN, GPIO.OUT)
            InitPin(LED1_PIN, GpioPinDriveMode.Output);
            InitPin(LED2_PIN, GpioPinDriveMode.Output);

            //    GPIO.setup(self.OC1_PIN, GPIO.OUT)
            //    if revision == 1:
            //        self.OC2_PIN = self.OC2_PIN_R1
            //    else:
            //        self.OC2_PIN = self.OC2_PIN_R2
            // GPIO.setup(self.OC2_PIN_R2, GPIO.OUT)

            if (revision == 1)
            {
                OC2_PIN = OC2_PIN_R1;
            }
            else
            {
                OC2_PIN = OC2_PIN_R2;
            }

            InitPin(OC2_PIN_R2, GpioPinDriveMode.Output);

            //    GPIO.setup(self.SW1_PIN, GPIO.IN)
            //    GPIO.setup(self.SW2_PIN, GPIO.IN)
            //    GPIO.setup(self.TRIGGER_PIN, GPIO.OUT)
            //    GPIO.setup(self.ECHO_PIN, GPIO.IN)

            InitPin(SW1_PIN, GpioPinDriveMode.Input);
            InitPin(SW2_PIN, GpioPinDriveMode.Input);
            InitPin(TRIGGER_PIN, GpioPinDriveMode.Input);
            InitPin(ECHO_PIN, GpioPinDriveMode.Input);
        }

            //    def set_motors(self, left_pwm, left_dir, right_pwm, right_dir):
            //    if self.old_left_dir != left_dir or self.old_right_dir != right_dir:
            //        self.set_driver_pins(0, 0, 0, 0)    # stop motors between sudden changes of direction
            //        time.sleep(self.MOTOR_DELAY)
            //    self.set_driver_pins(left_pwm, left_dir, right_pwm, right_dir)
            //    self.old_left_dir = left_dir
            //    self.old_right_dir = right_dir

            //def set_driver_pins(self, left_pwm, left_dir, right_pwm, right_dir):
            //    self.left_pwm.ChangeDutyCycle(left_pwm* 100 * self.pwm_scale)
            //    GPIO.output(self.LEFT_1_PIN, left_dir)
            //    GPIO.output(self.LEFT_2_PIN, not left_dir)
            //    self.right_pwm.ChangeDutyCycle(right_pwm* 100 * self.pwm_scale)
            //    GPIO.output(self.RIGHT_1_PIN, right_dir)
            //    GPIO.output(self.RIGHT_2_PIN, not right_dir)

            //def forward(self, seconds= 0, speed= 1.0):
            //    self.set_motors(speed, 0, speed, 0)
            //    if seconds > 0:
            //        time.sleep(seconds)
            //        self.stop()

            //def stop(self):
            //    self.set_motors(0, 0, 0, 0)

            //def reverse(self, seconds= 0, speed= 1.0):
            //    self.set_motors(speed, 1, speed, 1)
            //    if seconds > 0:
            //        time.sleep(seconds)
            //        self.stop()

            //def left(self, seconds= 0, speed= 0.5):
            //    self.set_motors(speed, 0, speed, 1)
            //    if seconds > 0:
            //        time.sleep(seconds)
            //        self.stop()

            //def right(self, seconds= 0, speed= 0.5):
            //    self.set_motors(speed, 1, speed, 0)
            //    if seconds > 0:
            //        time.sleep(seconds)
            //        self.stop()

            //def step_forward(self, delay, num_steps):
            //    for i in range(0, num_steps) :
            //        self.set_driver_pins(1, 1, 1, 0)
            //        time.sleep(delay)
            //        self.set_driver_pins(1, 1, 1, 1)
            //        time.sleep(delay)
            //        self.set_driver_pins(1, 0, 1, 1)
            //        time.sleep(delay)
            //        self.set_driver_pins(1, 0, 1, 0)
            //        time.sleep(delay)
            //    self.set_driver_pins(0, 0, 0, 0)

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

            //def _send_trigger_pulse(self) :
            //    GPIO.output(self.TRIGGER_PIN, True)
            //    time.sleep(0.0001)
            //    GPIO.output(self.TRIGGER_PIN, False)

            //def _wait_for_echo(self, value, timeout) :
            //    count = timeout
            //    while GPIO.input(self.ECHO_PIN) != value and count > 0:
            //        count -= 1

            //def get_distance(self):
            //    self._send_trigger_pulse()
            //    self._wait_for_echo(True, 10000)
            //    start = time.time()
            //    self._wait_for_echo(False, 10000)
            //    finish = time.time()
            //    pulse_len = finish - start
            //    distance_cm = pulse_len / 0.000058
            //    return distance_cm

            //def cleanup(self) :
            //    GPIO.cleanup()
        

        private GpioPin InitPin(int pinId, GpioPinDriveMode mode)
        {

            var pin = GpioController.Instance.GetGpioPinByBcmPinNumber(pinId);
            pin.PinMode = mode;

            return pin;
        }

        private GpioPin InitPwmOutputPin(int pinId, int initialValue)
        {
            var pin = GpioController.Instance.GetGpioPinByBcmPinNumber(pinId);
            pin.PinMode = GpioPinDriveMode.PwmOutput;
            pin.SoftPwmValue = initialValue;

            return pin;
        }
    }
}