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

public partial class UserControl_Gpio : UserControl
{
    /* GPIO functions are in a separate class */
    private Gpio_Demo? Gpio;

    private bool ledIsOn = false;
    private bool buttonIsActive = false;

    /* Default values from boardvalues.json */
    private int GpioNoOutputLed = DefaultValues.GpioNoOutputLed;
    private int GpioNoOutputButton = DefaultValues.GpioNoOutputButton;
    private int GpioNoInputButton = DefaultValues.GpioNoInputButton;

    public UserControl_Gpio()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
    }

    private void BtnLedSwitch_Clicked(object sender, RoutedEventArgs args)
    {
        if (!ledIsOn)
        {
            /* Get Pin numbers from TextBoxes */
            GetValuesFromTextBox();

            /* Create new object Gpio_Tests */
            Gpio = new Gpio_Demo(GpioNoOutputLed);
            /* Create new thread, light up LED */
            Thread ledOnThread = new(() => Gpio.LedSwitchOn());
            ledOnThread.Start();
            ledIsOn = true;
            /* Change UI */
            btnLedSwitch.Content = "Switch Off";
            btnLedSwitch.Background = Brushes.Red;
            txInfoLed.Text = $"LED on GPIO Pin {GpioNoOutputLed} is on";
        }
        else
        {
            /* Create new thread, turn off LED */
            Thread ledOffThread = new(Gpio!.LedSwitchOff);
            ledOffThread.Start();
            ledIsOn = false;
            /* Change UI */
            btnLedSwitch.Content = "Switch On";
            btnLedSwitch.Background = Brushes.LightGreen;
            txInfoLed.Text = $"LED on GPIO Pin {GpioNoOutputLed} is off";
        }
    }

    private void BtnGpioInput_Clicked(object sender, RoutedEventArgs args)
    {
        if (!buttonIsActive)
        {
            /* Get Pin numbers from TextBoxes */
            GetValuesFromTextBox();

            /* Create new object Gpio_Tests */
            Gpio = new Gpio_Demo(GpioNoOutputButton, GpioNoInputButton);
            /* Create new thread, turn off LED */
            Thread inputThread = new(() => Gpio.ActivateInputListener());
            inputThread.Start();
            buttonIsActive = true;
            /* Change UI */
            btnGpioInput.Content = "Deactivate Input";
            btnGpioInput.Background = Brushes.Red;
            txInfoInput.Text = "Waiting for hardware button click";
        }
        else
        {
            /* Create new thread, turn off LED */
            Thread inputStopThread = new(new ThreadStart(Gpio!.StopInputListener));
            inputStopThread.Start();
            buttonIsActive = false;
            /* Change UI */
            btnGpioInput.Content = "Activate Input";
            btnGpioInput.Background = Brushes.LightGreen;
            txInfoInput.Text = "Hardware button deactivated";
        }
    }

    private void GetValuesFromTextBox()
    {
        /* Get Pin numbers from TextBoxes */
        if (!string.IsNullOrEmpty(tbLedPin.Text))
            GpioNoOutputLed = Convert.ToInt32(tbLedPin.Text);
        else
            tbLedPin.Text = Convert.ToString(GpioNoOutputLed);

        if (!string.IsNullOrEmpty(tbInputLedPin.Text))
            GpioNoOutputButton = Convert.ToInt32(tbInputLedPin.Text);
        else
            tbInputLedPin.Text = Convert.ToString(GpioNoOutputButton);

        if (!string.IsNullOrEmpty(tbInputButtonPin.Text))
            GpioNoInputButton = Convert.ToInt32(tbInputButtonPin.Text);
        else
            tbInputButtonPin.Text = Convert.ToString(GpioNoInputButton);
    }

    private void AddButtonHandlers()
    {
        /* GPIO_LED button bindings */
        btnLedSwitch.AddHandler(Button.ClickEvent, BtnLedSwitch_Clicked!);
        /* GPIO_Input button bindings */
        btnGpioInput.AddHandler(Button.ClickEvent, BtnGpioInput_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard GPIO pins in textboxes */
        tbLedPin.Text = Convert.ToString(GpioNoOutputLed);
        tbInputLedPin.Text = Convert.ToString(GpioNoOutputButton);
        tbInputButtonPin.Text = Convert.ToString(GpioNoInputButton);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbLedPin.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbInputLedPin.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbInputButtonPin.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        txDescLed.Text = "This test will light up the LED connected to the selected GPIO pin.";
        txDescInput.Text = "This test will light up the LED on hardware button click."; // 
        txInfoLed.Text = "";
        txInfoInput.Text = "";
    }
}