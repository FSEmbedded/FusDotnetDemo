using System.Threading;
using System.Collections.Generic;
using Iot.Device.BoardLed;

namespace dotnetIot_Demo.Models.Hardware;

internal class Led_Demo
{
    private bool runLedBlinkTest = false;

    public static List<string> GetAllLeds()
    {
        List<string> ledNames = [];
        /* Get all BoardLed instances of on-board LEDs */
        IEnumerable<BoardLed> leds = BoardLed.EnumerateLeds();
        /* Return a list with all LED names */
        foreach (var led in leds)
        {
            ledNames.Add(led.Name);
        }
        return ledNames;
    }

    public void StartLedBlink(string ledName)
    {
        runLedBlinkTest = true;

        /* Open the LED with the specified name */
        using BoardLed led = new(ledName);

        /* Set trigger.
        /* The kernel provides some triggers which let the kernel control the LED.
        /* If you want to operate the LED, you need to remove the trigger -> set it to "none"
        /* Keep default trigger for reset.
        */
        string defaultTrigger = led.Trigger;
        led.Trigger = "none";

        /* Get the max brightness of current LED (Could be 1 or 255) */
        int maxBrightness = led.MaxBrightness;

        /* Let LED blink for 10 times */
        while (runLedBlinkTest)
        {
            /* Set brightness to max */
            led.Brightness = maxBrightness;
            Thread.Sleep(250);
            /* Turn LED off */
            led.Brightness = 0;
            Thread.Sleep(250);
        }
        /* Reset trigger */
        led.Trigger = defaultTrigger;
    }

    public void StopLedBlink()
    {
        runLedBlinkTest = false;
    }
}
