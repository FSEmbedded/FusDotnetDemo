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
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FusDotnetDemo.Models.Tools;
using FusDotnetDemo.Models.Hardware;

namespace FusDotnetDemo.Views;

public partial class UserControl_Pwm : UserControl
{
    /* PWM functions are in a separate class */
    private Pwm_Demo? PwmTS;
    private Pwm_Demo? PwmV;

    /* GPIO Pin # */
    private int PinPwmTimeSpan;
    private int PinPwmVoltage;

    private bool sliderIsActive = false;

    /* Default values from boardvalues.json */
    private int GpioNo = DefaultValues.PwmGpioNo;
    private int Duration = DefaultValues.PwmDuration;
    private double VoltageValue = DefaultValues.PwmVoltageValue;

    public UserControl_Pwm()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
        SetupSlider();
    }

    private void BtnPwmTS_Clicked(object sender, RoutedEventArgs args)
    {
        /* Clear UI */
        txInfoPwmTimeSpan.Text = "";
        txInfoPwmTimeSpan.Foreground = Brushes.Blue;

        /* Get GPIO Pin # and  Time Span of dimming process */
        GetValuesFromTextBox(0);

        /* Convert GPIO Pin # to gpio pin */
        PinPwmTimeSpan = Helper.GetGpioPin(GpioNo);

        /* PwmDimLed takes 100 steps, sleep is in ms: (duration * 1000)ms / 100 */
        int sleep = Duration * 10;

        try
        {
            /* Create new object Pwm_Tests */
            PwmTS = new Pwm_Demo(PinPwmTimeSpan);
        }
        catch (Exception ex)
        {
            txInfoPwmTimeSpan.Text = ex.Message;
            txInfoPwmTimeSpan.Foreground = Brushes.Red;
            return;
        }
        /* Start dimming */
        PwmTS.PwmDimTimespan(sleep);
    }

    private void BtnPwmV_Clicked(object sender, RoutedEventArgs args)
    {
        /* Clear UI */
        txInfoPwmVoltage.Text = "";
        txInfoPwmVoltage.Foreground = Brushes.Blue;

        if (!sliderIsActive)
        {
            /* Get GPIO Pin # from UI */
            GetValuesFromTextBox(1);

            /* Convert GPIO Pin # to gpio pin */
            PinPwmVoltage = Helper.GetGpioPin(GpioNo);
            VoltageValue = slVoltage.Value;

            try
            {
                /* Create new object Pwm_Tests */
                PwmV = new Pwm_Demo(PinPwmVoltage);
            }
            catch (Exception ex)
            {
                txInfoPwmVoltage.Text = ex.Message;
                txInfoPwmVoltage.Foreground = Brushes.Red;
                return;
            }

            /* Create new thread, light up LED */
            Thread pwmDimValueThread = new(() => PwmV.PwmDimValue(VoltageValue));
            pwmDimValueThread.Start();
            sliderIsActive = true;
            /* Change UI */
            btnPwmVoltage.Content = "Deactivate Slider";
            btnPwmVoltage.Background = Brushes.Red;
            txInfoPwmVoltage.Text = "Move slider to change brightness";
        }
        else
        {
            /* Create new thread, turn off LED */
            Thread stopPwmDimValueThread = new(PwmV!.StopPwmDimValue);
            stopPwmDimValueThread.Start();
            sliderIsActive = false;
            /* Change UI */
            btnPwmVoltage.Content = "Activate Slider";
            btnPwmVoltage.Background = Brushes.LightGreen;
            txInfoPwmVoltage.Text = "Slider is deactivated";
        }
    }

    private void SlVoltage_OnPointerMoved(object sender, RoutedEventArgs args)
    {
        if (sliderIsActive)
        {
            /* Set voltage when slider is moved */
            VoltageValue = slVoltage.Value;
            PwmV!.SetVoltageValue(VoltageValue);
        }
    }

    private void GetValuesFromTextBox(int callerId)
    {
        switch (callerId)
        {
            /* BtnPwmTS_Clicked() */
            case 0:
                if (!string.IsNullOrEmpty(tbPwmPinTimeSpan.Text))
                    GpioNo = Convert.ToInt32(tbPwmPinTimeSpan.Text);
                else
                    tbPwmPinTimeSpan.Text = GpioNo.ToString();
                if (!string.IsNullOrEmpty(tbTimeSpan.Text))
                    Duration = Convert.ToInt32(tbTimeSpan.Text);
                else
                    tbTimeSpan.Text = Duration.ToString();
                break;
            /* BtnPwmV_Clicked() */
            case 1:
                if (!string.IsNullOrEmpty(tbPwmPinVoltage.Text))
                    GpioNo = Convert.ToInt32(tbPwmPinVoltage.Text);
                else
                    tbPwmPinVoltage.Text = GpioNo.ToString();
                break;
        }
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnPwmTimeSpan.AddHandler(Button.ClickEvent, BtnPwmTS_Clicked!);
        btnPwmVoltage.AddHandler(Button.ClickEvent, BtnPwmV_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard GPIO pins in textboxes */
        tbPwmPinTimeSpan.Text = Convert.ToString(GpioNo);
        tbTimeSpan.Text = Convert.ToString(Duration);
        tbPwmPinVoltage.Text = Convert.ToString(GpioNo);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbPwmPinTimeSpan.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbTimeSpan.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbPwmPinVoltage.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        txDescPwmTimeSpan.Text = "This test will light up the LED connected to the selected GPIO pin.\r\n" +
            "The brightness will be increased over time by changing PWM values";
        txDescPwmVoltage.Text = "This test will light up the LED connected to the selected GPIO pin.\r\n" +
            "LED brightness by changing the value for voltage using the slider";
        txInfoPwmTimeSpan.Text = "";
        txInfoPwmVoltage.Text = "";
    }

    private void SetupSlider()
    {
        /* Set values for slider, add handler for movement */
        slVoltage.Minimum = 0;
        slVoltage.Maximum = 1;
        slVoltage.Value = VoltageValue;
        slVoltage.AddHandler(PointerMovedEvent, SlVoltage_OnPointerMoved!);
    }
}
