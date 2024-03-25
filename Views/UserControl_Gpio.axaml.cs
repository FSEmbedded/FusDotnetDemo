using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Threading;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_Gpio : UserControl
{
    /* GPIO functions are in separate class */
    private readonly Gpio_Tests Gpio;
    private bool ledIsOn = false;
    private bool buttonIsActive = false;

    public UserControl_Gpio()
    {
        InitializeComponent();

        /* GPIO_LED button bindings */
        btnLedSwitch.AddHandler(Button.ClickEvent, BtnLedSwitch_Clicked!);
        btnLedBlink.AddHandler(Button.ClickEvent, BtnLedBlink_Clicked!);

        /* GPIO_Input button bindings */
        btnGpioInput.AddHandler(Button.ClickEvent, BtnGpioInput_Clicked!);

        tbLedDesc.Text = "Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11";
        tbInputDesc.Text = "Connect Button to PcoreBBDSI Rev1.40 - J11-18 / J11-27";

        Gpio = new Gpio_Tests();
    }

    void BtnLedSwitch_Clicked(object sender, RoutedEventArgs args)
    {
        if(!ledIsOn)
        {
            /* Create new thread, light up LED */
            Thread ledOnThread = new(new ThreadStart(Gpio.LedOn));
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
        /* Create new thread, switch LED on and off */
        Thread ledBlinkThread = new(new ThreadStart(Gpio.LedBlink));
        ledBlinkThread.Start();

        tbLedInfo.Text = "LED on Pin J11-8 is blinking";
    }

    void BtnGpioInput_Clicked(object sender, RoutedEventArgs args)
    {
        if (!buttonIsActive)
        {
            /* Create new thread, turn off LED */
            Thread inputThread = new(new ThreadStart(Gpio.ReadGpioInput));
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
}

//TODO: Abfrage, welcher GPIO genutzt wird - Gpio an Test-Klasse geben, Info über Anschlusspins anzeigen