using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using IoTLib_Test.Models;
using UnitsNet;

namespace IoTLib_Test.Views;

public partial class UserControl_Pwm : UserControl
{
    /* PWM functions are in separate class */
    private readonly Pwm_Tests Pwm;
    /* GPIO Pin # */
    private int gpioNoPwm = 1; // GPIO_J1_54
    private int pinPwm;

    private int duration = 10;

    public UserControl_Pwm()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();

        Pwm = new Pwm_Tests();
    }

    private void BtnPwm_Clicked(object sender, RoutedEventArgs args)
    {
        /* Clear UI */
        txInfoPwm.Text = "";

        /* Convert GPIO Pin # to gpio pin */
        gpioNoPwm = Convert.ToInt32(tbPwmPin.Text);
        pinPwm = Helper.GetGpioPin(gpioNoPwm);

        /* Get Time Span of dimming process */
        duration = Convert.ToInt32(tbTimeSpan.Text);
        /* PwmDimLed takes 100 steps, sleep is in ms: (duration * 1000)ms / 100 */
        int sleep = duration * 10;

        Pwm.PwmDimLed(pinPwm, sleep);
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnPwm.AddHandler(Button.ClickEvent, BtnPwm_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard GPIO pins in textboxes */
        tbPwmPin.Text = Convert.ToString(gpioNoPwm);
        tbTimeSpan.Text = Convert.ToString(duration);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbPwmPin.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbTimeSpan.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        txDescPwm.Text = "Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11\r\n" +
            "LED brightness will be increased by changing PWM values."; // GPIO_J1_54
        txInfoPwm.Text = "";
    }

}
//TODO: ADC hier einfügen!?
