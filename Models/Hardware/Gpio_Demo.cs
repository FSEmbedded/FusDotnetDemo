/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Bruegel                              *
*                                                       *
*    This file is part of FusDotnetDemo.                *
*                                                       *
*********************************************************/

using FusDotnetDemo.Models.Tools;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;

namespace FusDotnetDemo.Models.Hardware;

internal class Gpio_Demo
{
    /* GPIO_Output values */
    private GpioDriver? drvGpioOut;
    private readonly int bankOut;
    private readonly int pinOut;
    private bool outputIsActive;

    /* GPIO_Input values */
    private GpioDriver? drvGpioIn;
    private GpioController? controllerIn;
    private readonly int bankIn;
    private readonly int pinIn;

    public Gpio_Demo(int gpioNoOut)
    {
        /* Use this constructor if you only need the LED test */
        /* Get bank and pin for selected GPIO pin */
        bankOut = Helper.GetGpioBank(gpioNoOut);
        pinOut = Helper.GetGpioPin(gpioNoOut);
    }

    public Gpio_Demo(int gpioNoOut, int gpioNoIn)
    {
        /* Use this constructor for GPIO input test - will listen for input and light up LED */
        /* Get bank and pin for selected GPIO pin */
        bankOut = Helper.GetGpioBank(gpioNoOut);
        pinOut = Helper.GetGpioPin(gpioNoOut);
        bankIn = Helper.GetGpioBank(gpioNoIn);
        pinIn = Helper.GetGpioPin(gpioNoIn);
    }

    #region GPIO_LED
    public void LedSwitchOn()
    {
        drvGpioOut = new LibGpiodDriver(bankOut);
        /* using declaration ensures hardware resources will be released properly */
        using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioOut);
        /* Open pin for Output */
        controller.OpenPin(pinOut, PinMode.Output);
        /* Write PinValue high = LED on */
        controller.Write(pinOut, PinValue.High);
        outputIsActive = true;
    }

    public void LedSwitchOff()
    {
        drvGpioOut = new LibGpiodDriver(bankOut);
        /* using declaration ensures hardware resources will be released properly */
        using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioOut);
        /* Open pin for Output */
        controller.OpenPin(pinOut, PinMode.Output);
        /* Write PinValue low = LED off */
        controller.Write(pinOut, PinValue.Low);
        outputIsActive = false;
    }
    #endregion
    #region GPIO_Input
    public void ActivateInputListener()
    {
        drvGpioIn = new LibGpiodDriver(bankIn);
        controllerIn = new GpioController(PinNumberingScheme.Logical, drvGpioIn!);

         /* Open pin for InputPullUp
          * If pin is connected to the ground, it will return PinValue.Low
          * If the circuit is open, the pin returns PinValue.High */
        controllerIn.OpenPin(pinIn, PinMode.InputPullUp);

        /* Set event for hardware button clicked (PinValue.Low / PinEventTypes.Falling) */
        controllerIn.RegisterCallbackForPinValueChangedEvent(
            pinIn,
            PinEventTypes.Falling,
            OnButton_Press);
        /* Set event for hardware button released (PinValue.High / PinEventTypes.Rising) */
        controllerIn.RegisterCallbackForPinValueChangedEvent(
            pinIn,
            PinEventTypes.Rising,
            OnButton_Release);
    }

    public void StopInputListener()
    {
        if (controllerIn != null)
        {
            /* Close pin and release hardware resources */
            controllerIn.ClosePin(pinIn);
            controllerIn.Dispose();
        }
    }

    private void OnButton_Press(object sender, PinValueChangedEventArgs args)
    {
        /* Stop if LED is already on */
        if (outputIsActive)
            return;
        LedSwitchOn();
    }

    private void OnButton_Release(object sender, PinValueChangedEventArgs args)
    {
        /* Stop if LED is already off */
        if (!outputIsActive)
            return;
        LedSwitchOff();
    }
    #endregion
}
