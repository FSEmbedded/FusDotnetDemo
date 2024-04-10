using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models.Tools;
using IoTLib_Test.Models.Hardware_Tests;

namespace IoTLib_Test.Views;

public partial class UserControl_Gpio : UserControl
{
    /* GPIO functions are in separate class */
    private Gpio_Tests? Gpio;
    /* GPIO Pin # */
    private int gpioNoLed = 1; // GPIO_J1_54
    private int gpioNoInput = 78; // GPIO_J1_52

    private bool ledIsOn = false;
    private bool buttonIsActive = false;

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
            /* Create new instance of Gpio_Tests */
            gpioNoLed = Convert.ToInt32(tbLedPin.Text);
            Gpio = new(gpioNoLed);
            /* Create new thread, light up LED */
            Thread ledOnThread = new(() => Gpio.LedSwitchOn());
            ledOnThread.Start();
            ledIsOn = true;
            /* Change UI */
            btnLedSwitch.Content = "Switch Off";
            btnLedSwitch.Background = Brushes.Red;
            txInfoLed.Text = "LED on Pin J11-8 is on";
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
            txInfoLed.Text = "LED on Pin J11-8 is off";
        }
    }

    private void BtnGpioInput_Clicked(object sender, RoutedEventArgs args)
    {
        if (!buttonIsActive)
        {
            /* Create new instance of Gpio_Tests */
            gpioNoInput = Convert.ToInt32(tbInputPin.Text);
            gpioNoLed = Convert.ToInt32(tbLedPin.Text);
            Gpio = new(gpioNoLed, gpioNoInput);
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
            txInfoInput.Text = "hardware button deactivated";
        }
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
        tbLedPin.Text = Convert.ToString(gpioNoLed);
        tbInputPin.Text = Convert.ToString(gpioNoInput);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbLedPin.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbInputPin.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        //TODO: Desc Text verbessern: Pins in Doku/Readme
        txDescLed.Text = "Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11"; // GPIO_J1_54
        txDescInput.Text = "Connect Button to PcoreBBDSI Rev1.40 - J11-18 / J11-27.\r\nThis test will light up the LED defined in \"GPIO LED Test\" on button click."; // GPIO_J1_52
        txInfoLed.Text = "";
        txInfoInput.Text = "";
    }
}