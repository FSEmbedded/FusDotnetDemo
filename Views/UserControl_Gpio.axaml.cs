using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Input;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_Gpio : UserControl
{
    /* GPIO functions are in separate class */
    private readonly Gpio_Tests Gpio;
    private bool ledIsOn = false;
    private bool buttonIsActive = false;

    /* GPIO Pin # */
    int gpioNoLed = 1; // GPIO_J1_54
    int ledBank;
    int ledPin;
    int gpioNoInput = 78; // GPIO_J1_52
    int inputBank;
    int inputPin;


    public UserControl_Gpio()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();

        /* Convert GPIO Pin number to gpio bank and pin */
        ledBank = PinConverter.GetGpioBank(gpioNoLed);
        ledPin = PinConverter.GetGpioPin(gpioNoLed);
        /* Convert GPIO Pin # to gpio bank and pin */
        inputBank = PinConverter.GetGpioBank(gpioNoInput);
        inputPin = PinConverter.GetGpioPin(gpioNoInput);

        Gpio = new Gpio_Tests(ledBank, ledPin, inputBank, inputPin);
    }

    void BtnLedSwitch_Clicked(object sender, RoutedEventArgs args)
    {
        /* Convert GPIO Pin # to gpio bank and pin */
        gpioNoLed = Convert.ToInt32(tbLedPin.Text);
        ledBank = PinConverter.GetGpioBank(gpioNoLed);
        ledPin = PinConverter.GetGpioPin(gpioNoLed);

        if (!ledIsOn)
        {
            /* Create new thread, light up LED */
            Thread ledOnThread = new(() => Gpio.TurnOnLed(ledBank, ledPin));
            ledOnThread.Start();
            ledIsOn = true;
            /* Change UI */
            btnLedSwitch.Content = "LED Off";
            btnLedSwitch.Background = Brushes.Red;
            txInfoLed.Text = "LED on Pin J11-8 is on";
        }
        else
        {
            /* Create new thread, turn off LED */
            Thread ledOffThread = new(new ThreadStart(Gpio.TurnOffLed));
            ledOffThread.Start();
            ledIsOn = false;
            /* Change UI */
            btnLedSwitch.Content = "LED On";
            btnLedSwitch.Background = Brushes.LightGreen;
            txInfoLed.Text = "LED on Pin J11-8 is off";
        }
    }

    void BtnLedBlink_Clicked(object sender, RoutedEventArgs args)
    {
        /* Convert GPIO Pin # to gpio bank and pin */
        gpioNoLed = Convert.ToInt32(tbLedPin.Text);
        ledBank = PinConverter.GetGpioBank(gpioNoLed);
        ledPin = PinConverter.GetGpioPin(gpioNoLed);

        Gpio.BlinkLed(ledBank, ledPin);
    }

    void BtnGpioInput_Clicked(object sender, RoutedEventArgs args)
    {
        /* Convert GPIO Pin # to gpio bank and pin */
        gpioNoInput = Convert.ToInt32(tbInputPin.Text);
        inputBank = PinConverter.GetGpioBank(gpioNoInput);
        inputPin = PinConverter.GetGpioPin(gpioNoInput);

        if (!buttonIsActive)
        {
            /* Create new thread, turn off LED */
            Thread inputThread = new(() => Gpio.ActivateButtonInput(inputBank, inputPin));
            inputThread.Start();
            buttonIsActive = true;
            /* Change UI */
            btnGpioInput.Content = "Stop GPIO Input";
            btnGpioInput.Background = Brushes.Red;
            txInfoInput.Text = "Waiting for Hardware-Button click";
        }
        else
        {
            /* Create new thread, turn off LED */
            Thread inputStopThread = new(new ThreadStart(Gpio.StopButtonInput));
            inputStopThread.Start();
            buttonIsActive = false;
            /* Change UI */
            btnGpioInput.Content = "Start GPIO Input";
            btnGpioInput.Background = Brushes.LightGreen;
            txInfoInput.Text = "Hardware-Button deactivated";
        }
    }

    void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        /* Check if the pressed key is a control character (like Backspace) or a digit */
        if (!char.IsControl(Convert.ToChar(e.KeySymbol!)) && !char.IsDigit(Convert.ToChar(e.KeySymbol!)))
        {
            /* If it's not, prevent the character from being entered */
            e.Handled = true;
        }
    }

    void AddButtonHandlers()
    {
        /* GPIO_LED button bindings */
        btnLedSwitch.AddHandler(Button.ClickEvent, BtnLedSwitch_Clicked!);
        btnLedBlink.AddHandler(Button.ClickEvent, BtnLedBlink_Clicked!);
        /* GPIO_Input button bindings */
        btnGpioInput.AddHandler(Button.ClickEvent, BtnGpioInput_Clicked!);
    }

    void WriteStandardValuesInTextBox()
    {
        /* Write standard GPIO pins in textboxes */
        tbLedPin.Text = Convert.ToString(gpioNoLed);
        tbInputPin.Text = Convert.ToString(gpioNoInput);
    }

    void AddTextBoxHandlers()
    {
        /* Handler to only allow number inputs */
        tbLedPin.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbInputPin.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
    }

    void FillTextBlockWithText()
    {
        txDescLed.Text = "Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11"; // GPIO_J1_54
        txDescInput.Text = "Connect Button to PcoreBBDSI Rev1.40 - J11-18 / J11-27"; // GPIO_J1_52
        txInfoLed.Text = "";
        txInfoInput.Text = "";
    }
}