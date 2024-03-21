using Avalonia.Controls;
using Avalonia.Interactivity;
using IoTLib_Test.Models;
using System.Device.Gpio;

namespace IoTLib_Test.Views;

public partial class UserControl_Gpio : UserControl
{
    /* GPIO functions are in separate class */
    private readonly Gpio_Tests GpioOut;

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

        GpioOut = new Gpio_Tests();
    }

    void BtnLedOn_Clicked(object sender, RoutedEventArgs args)
    {
        GpioOut!.LedOn();

        btnLedOn.IsEnabled = false;
        btnLedOff.IsEnabled = true;
        tbInfo.Text = "LED on Pin J11-8 is on";
    }

    void BtnLedOff_Clicked(object sender, RoutedEventArgs args)
    {
        GpioOut!.LedOff();

        btnLedOn.IsEnabled = true;
        btnLedOff.IsEnabled = false;
        tbInfo.Text = "LED on Pin J11-8 is off";
    }

    void BtnLedBlink_Clicked(object sender, RoutedEventArgs args)
    {
        GpioOut!.LedBlink();
        tbInfo.Text = "LED on Pin J11-8 is blinking";
    }

    void BtnStartInput_Clicked(object sender, RoutedEventArgs args)
    {
        GpioOut!.ReadGpioInput();

        btnStartInput.IsEnabled = false;
        btnStopInput.IsEnabled = true;
        btnControl.IsVisible = true; //TODO: Grün, wenn Hardware Button geklickt, rot wenn released
        tbInfo.Text = "Waiting for Hardware-Button click";
    }

    void BtnStopInput_Clicked(object sender, RoutedEventArgs args)
    {
        GpioOut!.StopGpioInput();

        btnStartInput.IsEnabled = true;
        btnStopInput.IsEnabled = false;
        btnControl.IsVisible = false;
        tbInfo.Text = "Hardware-Button deactivated";
    }
}

//TODO: Abfrage, welcher GPIO genutzt wird - Gpio an Test-Klasse geben, Info über Anschlusspins anzeigen