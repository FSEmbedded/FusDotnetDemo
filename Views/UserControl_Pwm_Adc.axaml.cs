using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_Pwm_Adc : UserControl
{
    /* PWM and ADC functions are in separate class */
    private readonly Pwm_Tests Pwm;
    private readonly Adc_Tests Adc;
    /* GPIO Pin # */
    int gpioNo = 10; // GPIO1_IO10
    int bank;
    int pin;

    public UserControl_Pwm_Adc()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();

        /* Convert GPIO Pin number to gpio bank and pin */
        bank = Helper.GetGpioBank(gpioNo);
        pin = Helper.GetGpioPin(gpioNo);

        Pwm = new Pwm_Tests();
        Adc = new Adc_Tests();
    }

    private void BtnPwm_Clicked(object sender, RoutedEventArgs args)
    {
        /* Convert GPIO Pin # to gpio bank and pin */
        gpioNo = Convert.ToInt32(tbPwmPin.Text);
        bank = Helper.GetGpioBank(gpioNo);
        pin = Helper.GetGpioPin(gpioNo);

        Pwm.PwmSet(bank, pin);
        //TODO: optimieren
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnPwm.AddHandler(Button.ClickEvent, BtnPwm_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard GPIO pins in textboxes */
        tbPwmPin.Text = Convert.ToString(gpioNo);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbPwmPin.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        txDescPwm.Text = "Connect voltmeter to J11-34 and GND"; //TODO
        txInfoPwm.Text = "";
    }

}
//TODO: ADC hier einfügen!
// PWM PWM3 GPIO1_IO10 IO 10 J11-34