using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Iot.Device.BoardLed;
using System.Collections.Generic;

namespace IoTLib_Test.Models
{
    internal class Led_Tests
    {
        //TODO: LED Tests
        // PCA9532 auf I2C-Extension-Board

        public void StartLedTest()
        {
            // Get all BoardLed instances of on-board LEDs.
            IEnumerable<BoardLed> leds = BoardLed.EnumerateLeds();

            // Open the LED with the specified name.
            BoardLed led = new BoardLed("led0");

            // Get all triggers of current LED.
            IEnumerable<string> triggers = led.EnumerateTriggers();

            // Set trigger.
            // The kernel provides some triggers which let the kernel control the LED.
            // For example, the red light of Raspberry Pi, whose trigger is "default-on", which makes it keep lighting up.
            // If you want to operate the LED, you need to remove the trigger, which is to set its trigger to "none".
            led.Trigger = "none";

            // Get the max brightness of current LED.
            int maxBrightness = led.MaxBrightness;

            // Set brightness.
            led.Brightness = 255;
        }
    }
}
