/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Bruegel                              *
*                                                       *
*    This file is part of FusDotnetDemo.                *
*                                                       *
*********************************************************/

using System;
using System.Threading;
using System.Device.Pwm.Drivers;

namespace FusDotnetDemo.Models.Hardware;

internal class Pwm_Demo
{
    private readonly SoftwarePwmChannel pwmChannel;
    private readonly int frequency = 200;
    private double voltageValue;
    private bool sliderIsActive = false;

    public Pwm_Demo(int pin)
    {
        try
        {
            /* Create PWM Channel */
            pwmChannel = new SoftwarePwmChannel(pin, frequency);
        }
        catch (Exception ex)
        {
            throw new($"Exception: {ex.Message}");
        }
    }

    public void PwmDimTimespan(int sleep)
    {
        /* Increase PWM voltage over a defined time span */
        int dutyCycle = 0;
        /* Start with LED off */
        pwmChannel.DutyCycle = dutyCycle;
        pwmChannel.Start();

        /* Increase DutyCycle -> increase brightness */
        for (double fill = 0.0; fill <= 1.0; fill += 0.01)
        {
            pwmChannel.DutyCycle = fill;
            Thread.Sleep(sleep);
        }
        /* Clear channel */
        pwmChannel.Dispose();
    }

    public void PwmDimValue(double startValue)
    {
        /* Change PWM voltage by setting the voltage */
        voltageValue = startValue;
        sliderIsActive = true;

        while (sliderIsActive)
        {
            /* Set PWM voltage to a defined value */
            pwmChannel.DutyCycle = voltageValue;
            pwmChannel.Start();
        }
    }

    public void StopPwmDimValue()
    {
        /* Clear channel */
        sliderIsActive = false;
        pwmChannel.Dispose();
    }

    public void SetVoltageValue(double value)
    {
        /* Set value for while-loop in PwmDimValue */
        if (value >= 0.0 && value <= 1.0)
            voltageValue = value;
    }
}
