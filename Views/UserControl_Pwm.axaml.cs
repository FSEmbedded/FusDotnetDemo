using System;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_Pwm : UserControl
{
    /* PWM functions are in separate class */
    private readonly Pwm_Tests Pwm;

    /* GPIO Pin # */
    int gpioNo = 10; // GPIO1_IO10
    int bank;
    int pin;

    public UserControl_Pwm()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();

        /* Convert GPIO Pin number to gpio bank and pin */
        bank = PinConverter.GetGpioBank(gpioNo);
        pin = PinConverter.GetGpioPin(gpioNo);

        Pwm = new Pwm_Tests();
    }

    void BtnPwm_Clicked(object sender, RoutedEventArgs args)
    {
        /* Convert GPIO Pin # to gpio bank and pin */
        gpioNo = Convert.ToInt32(tbPwmPin.Text);
        bank = PinConverter.GetGpioBank(gpioNo);
        pin = PinConverter.GetGpioPin(gpioNo);

        Pwm.PwmSet(bank, pin);
        //TODO: optimieren
    }

    void AddButtonHandlers()
    {
        /* Button bindings */
        btnPwm.AddHandler(Button.ClickEvent, BtnPwm_Clicked!);
    }

    void WriteStandardValuesInTextBox()
    {
        /* Write standard GPIO pins in textboxes */
        tbPwmPin.Text = Convert.ToString(gpioNo);
    }

    void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbPwmPin.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    void FillTextBlockWithText()
    {
        txDescPwm.Text = "Connect voltmeter to J11-34 and GND"; //TODO
        txInfoPwm.Text = "";
    }

}
//TODO: ADC hier einfügen!?
// PWM PWM3 GPIO1_IO10 IO 10 J11-34