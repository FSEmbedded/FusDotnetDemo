using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using IoTLib_Test.Models.Hardware_Tests;

namespace IoTLib_Test.Views;

public partial class UserControl_Led : UserControl
{
    /* LED functions are in a separate class */
    private readonly Led_Tests Led;
    private string ledName = "";

    public UserControl_Led()
    {
        InitializeComponent();
        AddButtonHandlers();
        FillTextBlockWithText();
        SetupComboBox();

        tbLedName.IsReadOnly = true;
        btnLed.IsEnabled = false;
        /* Create new object Led_Tests */
        Led = new Led_Tests();
    }

    private void BtnLedName_Clicked(object sender, RoutedEventArgs args)
    {
        /* Empty list */
        cbLedNames.Items.Clear();

        List<string> ledNames = Led_Tests.GetAllLeds();
        /* Add all names to the ComboBox */
        foreach (string name in ledNames)
        {
            cbLedNames.Items.Add(name);
        }

        /* Select ledName in ComboBox */
        if (!string.IsNullOrEmpty(ledName) && ledNames.Contains(ledName))
        {
            cbLedNames.SelectedItem = ledName;
        }

        txInfoLedName.Text = "Select LED name from dropdown";
    }

    private void CbLedNames_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbLedNames.SelectedItem != null && !string.IsNullOrEmpty(cbLedNames.SelectedItem.ToString()))
        {
            /* Set ledName */
            ledName = cbLedNames.SelectedItem.ToString()!;
            tbLedName.Text = ledName;
            /* Close dropdown */
            cbLedNames.IsDropDownOpen = false;

            btnLed.IsEnabled = true;
        }
        else
            btnLed.IsEnabled = false;
    }

    private void BtnLed_Clicked(object sender, RoutedEventArgs args)
    {
        if (!string.IsNullOrEmpty(ledName))
        {
            /* Let LED blink */
            Led.StartLedTest(ledName);

            txInfoLed.Text = $"LED {ledName} blinks";
        }
        else
            txInfoLed.Text = "Select LED from Dropdown";
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnLedName.AddHandler(Button.ClickEvent, BtnLedName_Clicked!);
        btnLed.AddHandler(Button.ClickEvent, BtnLed_Clicked!);
    }

    private void FillTextBlockWithText()
    {
        //TODO: Desc Text verbessern: Pins in Doku/Readme, LED keyboard wird auch erkannt

        txDescLedName.Text = "Find all available LEDs on your Board. Us the desired LED name for the LED Test.";
        txDescLed.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16\r\n" +
            "Default LED name for extension board: pca:red:power\r\n" +
            "Driver for PCA9532 must be enabled";
        txInfoLedName.Text = "";
        txInfoLed.Text = "";
    }

    private void SetupComboBox()
    {
        cbLedNames.AddHandler(ComboBox.SelectionChangedEvent, CbLedNames_SelectionChanged!);
    }
}
