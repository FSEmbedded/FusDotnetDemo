using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_Gpio : UserControl
{
    /* GPIO functions are in separate class */
    private readonly Gpio_Tests Gpio;

    public UserControl_Gpio()
    {
        InitializeComponent();

        /* GPIO_LED button bindings */
        btnLedOn.AddHandler(Button.ClickEvent, BtnLedOn_Clicked!);
        btnLedOff.AddHandler(Button.ClickEvent, BtnLedOff_Clicked!);
        btnLedBlink.AddHandler(Button.ClickEvent, BtnLedBlink_Clicked!);
        
        /* GPIO_Input button bindings */
        btnStartInput.AddHandler(Button.ClickEvent, BtnStartInput_Clicked!);
        btnStopInput.AddHandler(Button.ClickEvent, BtnStopInput_Clicked!);

        tbLedInfo.Text = "Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11";
        tbInputInfo.Text = "Connect Button to PcoreBBDSI Rev1.40 - J11-18 / J11-27";

        Gpio = new Gpio_Tests();
    }

    void BtnLedOn_Clicked(object sender, RoutedEventArgs args)
    {
        Thread ledOnThread = new(new ThreadStart(Gpio.LedOn));
        ledOnThread.Start();

        //GpioOut!.LedOn();

        btnLedOn.IsEnabled = false;
        btnLedOff.IsEnabled = true;
        tbInfo.Text = "LED on Pin J11-8 is on";
    }

    void BtnLedOff_Clicked(object sender, RoutedEventArgs args)
    {
        Thread ledOffThread = new(new ThreadStart(Gpio.LedOff));
        ledOffThread.Start();

        //GpioOut!.LedOff();

        btnLedOn.IsEnabled = true;
        btnLedOff.IsEnabled = false;
        tbInfo.Text = "LED on Pin J11-8 is off";
    }

    void BtnLedBlink_Clicked(object sender, RoutedEventArgs args)
    {
        Thread ledBlinkThread = new(new ThreadStart(Gpio.LedBlink));
        ledBlinkThread.Start();

        //GpioOut!.LedBlink();

        tbInfo.Text = "LED on Pin J11-8 is blinking";
    }

    void BtnStartInput_Clicked(object sender, RoutedEventArgs args)
    {
        Thread inputThread = new(new ThreadStart(Gpio.ReadGpioInput));
        inputThread.Start();

        //GpioOut!.ReadGpioInput();

        btnStartInput.IsEnabled = false;
        btnStopInput.IsEnabled = true;
        tbInfo.Text = "Waiting for Hardware-Button click";
    }

    void BtnStopInput_Clicked(object sender, RoutedEventArgs args)
    {
        Thread inputStopThread = new(new ThreadStart(Gpio.StopGpioInput));
        inputStopThread.Start();

        //GpioOut!.StopGpioInput();

        btnStartInput.IsEnabled = true;
        btnStopInput.IsEnabled = false;
        tbInfo.Text = "Hardware-Button deactivated";
    }
}

//TODO: Abfrage, welcher GPIO genutzt wird - Gpio an Test-Klasse geben, Info über Anschlusspins anzeigen