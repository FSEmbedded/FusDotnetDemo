using Avalonia.Controls;
using Avalonia.Interactivity;
using IoTLib_Test.Models;
using System;
using UnitsNet;

namespace IoTLib_Test.Views;

public partial class UserControl_Adc : UserControl
{
    public UserControl_Adc()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
    }

    private void BtnAdc_Clicked(object sender, RoutedEventArgs args)
    {
        txInfoAdc.Text = "";

        //TODO
        Adc_Tests Adc = new();
        Adc.StartAdcTest();

        txInfoAdc.Text = "ADC active?";
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnAdc.AddHandler(Button.ClickEvent, BtnAdc_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard GPIO pins in textboxes */

        //TODO
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */

        //TODO
    }

    private void FillTextBlockWithText()
    {
        txDescAdc.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16\r\n" +
            "Driver for ADS7828 must be enabled\r\n" +
            "Connect BBDSI J11-8 with I²C Extension Board J2-17; Set S2-3 to ON";
        txInfoAdc.Text = "";
    }
}
