using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Input;
using System.Threading;
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

        /* GPIO_LED button bindings */
        btnLedSwitch.AddHandler(Button.ClickEvent, BtnLedSwitch_Clicked!);
        btnLedBlink.AddHandler(Button.ClickEvent, BtnLedBlink_Clicked!);

        /* GPIO_Input button bindings */
        btnGpioInput.AddHandler(Button.ClickEvent, BtnGpioInput_Clicked!);

        /* Write standard GPIO pins in textbox */
        tbLedPin.Text = Convert.ToString(gpioNoLed);
        tbInputPin.Text = Convert.ToString(gpioNoInput);

        /* Handler to only allow number inputs */
        tbLedPin.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbInputPin.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);

        tbLedDesc.Text = "Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11"; // GPIO_J1_54
        tbInputDesc.Text = "Connect Button to PcoreBBDSI Rev1.40 - J11-18 / J11-27"; // GPIO_J1_52
        tbLedInfo.Text = "";
        tbInputInfo.Text = "";

        /* Convert GPIO Pin # to gpio bank and pin */
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
            Thread ledOnThread = new(() => Gpio.LedOn(ledBank, ledPin));
            ledOnThread.Start();
            ledIsOn = true;
            /* Change UI */
            btnLedSwitch.Content = "LED Off";
            btnLedSwitch.Background = Brushes.Red;
            tbLedInfo.Text = "LED on Pin J11-8 is on";
        }
        else
        {
            /* Create new thread, turn off LED */
            Thread ledOffThread = new(new ThreadStart(Gpio.LedOff));
            ledOffThread.Start();
            ledIsOn = false;
            /* Change UI */
            btnLedSwitch.Content = "LED On";
            btnLedSwitch.Background = Brushes.LightGreen;
            tbLedInfo.Text = "LED on Pin J11-8 is off";
        }
    }

    void BtnLedBlink_Clicked(object sender, RoutedEventArgs args)
    {
        /* Convert GPIO Pin # to gpio bank and pin */
        gpioNoLed = Convert.ToInt32(tbLedPin.Text);
        ledBank = PinConverter.GetGpioBank(gpioNoLed);
        ledPin = PinConverter.GetGpioPin(gpioNoLed);

        Gpio.LedBlink(ledBank, ledPin);
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
            Thread inputThread = new(() => Gpio.ReadGpioInput(inputBank, inputPin));
            inputThread.Start();
            buttonIsActive = true;
            /* Change UI */
            btnGpioInput.Content = "Stop GPIO Input";
            btnGpioInput.Background = Brushes.Red;
            tbInputInfo.Text = "Waiting for Hardware-Button click";
        }
        else
        {
            /* Create new thread, turn off LED */
            Thread inputStopThread = new(new ThreadStart(Gpio.StopGpioInput));
            inputStopThread.Start();
            buttonIsActive = false;
            /* Change UI */
            btnGpioInput.Content = "Start GPIO Input";
            btnGpioInput.Background = Brushes.LightGreen;
            tbInputInfo.Text = "Hardware-Button deactivated";
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
}