using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models.Tools;
using IoTLib_Test.Models.Hardware_Tests;

namespace IoTLib_Test.Views;

public partial class UserControl_Pwm : UserControl
{
    /* PWM functions are in a separate class */
    private Pwm_Tests? PwmTS;
    private Pwm_Tests? PwmV;

    /* GPIO Pin # */
    private int gpioNoPwm = 1; // GPIO_J1_54
    private int pinPwmTS;
    private int pinPwmV;

    private int duration = 10;
    private double voltageValue = 0.5;
    private bool sliderIsActive = false;

    public UserControl_Pwm()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
        SetupSlider();
    }

    private void BtnPwmTS_Clicked(object sender, RoutedEventArgs args)
    {
        /* Clear UI */
        txInfoPwmTS.Text = "";
        txInfoPwmTS.Foreground = Brushes.Blue;

        /* Get GPIO Pin # and  Time Span of dimming process */
        GetValuesFromTextBox(0);

        /* Convert GPIO Pin # to gpio pin */
        pinPwmTS = Helper.GetGpioPin(gpioNoPwm);

        /* PwmDimLed takes 100 steps, sleep is in ms: (duration * 1000)ms / 100 */
        int sleep = duration * 10;

        try
        {
            /* Create new object Pwm_Tests */
            PwmTS = new Pwm_Tests(pinPwmTS);
        }
        catch (Exception ex)
        {
            txInfoPwmTS.Text = ex.Message;
            txInfoPwmTS.Foreground = Brushes.Red;
            return;
        }
        /* Start dimming */
        PwmTS.PwmDimTimespan(sleep);
    }

    private void BtnPwmV_Clicked(object sender, RoutedEventArgs args)
    {
        /* Clear UI */
        txInfoPwmV.Text = "";
        txInfoPwmV.Foreground = Brushes.Blue;

        if (!sliderIsActive)
        {
            /* Get GPIO Pin # from UI */
            GetValuesFromTextBox(1);

            /* Convert GPIO Pin # to gpio pin */
            pinPwmV = Helper.GetGpioPin(gpioNoPwm);
            voltageValue = slVoltage.Value;

            try
            {
                /* Create new object Pwm_Tests */
                PwmV = new Pwm_Tests(pinPwmV);
            }
            catch (Exception ex)
            {
                txInfoPwmV.Text = ex.Message;
                txInfoPwmV.Foreground = Brushes.Red;
                return;
            }

            /* Create new thread, light up LED */
            Thread pwmDimValueThread = new(() => PwmV.PwmDimValue(voltageValue));
            pwmDimValueThread.Start();
            sliderIsActive = true;
            /* Change UI */
            btnPwmV.Content = "Deactivate Slider";
            btnPwmV.Background = Brushes.Red;
            txInfoPwmV.Text = "Move slider to change brightness";
        }
        else
        {
            /* Create new thread, turn off LED */
            Thread stopPwmDimValueThread = new(PwmV!.StopPwmDimValue);
            stopPwmDimValueThread.Start();
            sliderIsActive = false;
            /* Change UI */
            btnPwmV.Content = "Activate Slider";
            btnPwmV.Background = Brushes.LightGreen;
            txInfoPwmV.Text = "Slider is deactivated";
        }
    }

    private void SlVoltage_OnPointerMoved(object sender, RoutedEventArgs args)
    {
        if (sliderIsActive)
        {
            /* Set voltage when slider is moved */
            voltageValue = slVoltage.Value;
            PwmV!.SetVoltageValue(voltageValue);
        }
    }

    public void GetValuesFromTextBox(int callerId)
    {
        switch (callerId)
        {
            /* BtnPwmTS_Clicked() */
            case 0:
                if (!string.IsNullOrEmpty(tbPwmPinTS.Text))
                    gpioNoPwm = Convert.ToInt32(tbPwmPinTS.Text);
                else
                    tbPwmPinTS.Text = gpioNoPwm.ToString();
                if (!string.IsNullOrEmpty(tbTimeSpan.Text))
                    duration = Convert.ToInt32(tbTimeSpan.Text);
                else
                    tbTimeSpan.Text = duration.ToString();
                break;
            /* BtnPwmV_Clicked() */
            case 1:
                if (!string.IsNullOrEmpty(tbPwmPinV.Text))
                    gpioNoPwm = Convert.ToInt32(tbPwmPinV.Text);
                else
                    tbPwmPinV.Text = gpioNoPwm.ToString();
                break;
        }
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnPwmTS.AddHandler(Button.ClickEvent, BtnPwmTS_Clicked!);
        btnPwmV.AddHandler(Button.ClickEvent, BtnPwmV_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard GPIO pins in textboxes */
        tbPwmPinTS.Text = Convert.ToString(gpioNoPwm);
        tbTimeSpan.Text = Convert.ToString(duration);
        tbPwmPinV.Text = Convert.ToString(gpioNoPwm);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbPwmPinTS.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbTimeSpan.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbPwmPinV.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        //TODO: Desc Text verbessern: Pins in Doku/Readme

        txDescPwmTS.Text = "Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11\r\n" +
            "LED brightness will be increased by changing PWM values"; // GPIO_J1_54
        txDescPwmV.Text = "Connect LED to PcoreBBDSI Rev1.40 - J11-8 / J11-11\r\n" +
            "LED brightness is changed by moving the slider"; // GPIO_J1_54
        txInfoPwmTS.Text = "";
        txInfoPwmV.Text = "";
    }

    private void SetupSlider()
    {
        /* Set values for slider, add handler for movement */
        slVoltage.Minimum = 0;
        slVoltage.Maximum = 1;
        slVoltage.Value = voltageValue;
        slVoltage.AddHandler(PointerMovedEvent, SlVoltage_OnPointerMoved!);
    }
}
