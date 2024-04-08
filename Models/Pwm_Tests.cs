using System.Threading;
using System.Device.Pwm.Drivers;

namespace IoTLib_Test.Models
{
    internal class Pwm_Tests
    {
        /* PWM Pin Number */
        int pin;

        public void PwmDimLed(int _pin, int sleep)
        {
            pin = _pin;
            int frequency = 200;
            int dutyCycle = 0;

            /* Create a Software PWM Channel */
            using (var pwmChannel = new SoftwarePwmChannel(pin, frequency, dutyCycle))
            {
                pwmChannel.Start();
                /* Increase Duty Cycle */
                for (double fill = 0.0; fill <= 1.0; fill += 0.01)
                {
                    pwmChannel.DutyCycle = fill;
                    Thread.Sleep(sleep);
                }
            }
        }
    }
}
