using System;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models;
using UnitsNet;

namespace IoTLib_Test.Views;

public partial class UserControl_Led : UserControl
{
    public UserControl_Led()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
    }

    private void BtnLed_Clicked(object sender, RoutedEventArgs args)
    {
        txInfoLed.Text = "";

        //TODO
        Led_Tests Led = new();
        Led.StartLedTest();

        txInfoLed.Text = "Is LED on?";
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnLed.AddHandler(Button.ClickEvent, BtnLed_Clicked!);
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
        txDescLed.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16\r\n" +
            "Driver for PCA9532 must be enabled";
        txInfoLed.Text = "";
    }
}
