/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Bruegel                              *
*                                                       *
*    This file is part of FusDotnetDemo.                *
*                                                       *
*********************************************************/

using System.Collections.Generic;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FusDotnetDemo.Models.Tools;
using FusDotnetDemo.Models.Hardware;

namespace FusDotnetDemo.Views;

public partial class UserControl_Led : UserControl
{
    /* LED functions are in a separate class */
    private readonly Led_Demo Led;

    private bool ledBlinkIsActive = false;

    /* Default values from boardvalues.json */
    private string LedName = DefaultValues.LedName;

    public UserControl_Led()
    {
        InitializeComponent();
        AddButtonHandlers();
        FillTextBlockWithText();
        SetupComboBox();

        tbLedName.IsReadOnly = true;
        ActivateButtonLed(false);
        /* Create new object Led_Tests */
        Led = new Led_Demo();
    }

    private void BtnLedName_Clicked(object sender, RoutedEventArgs args)
    {
        /* Empty ComboBox */
        cbLedNames.Items.Clear();

        List<string> LedNames = Led_Demo.GetAllLeds();
        
        if (LedNames.Count == 0)
        {
            txInfoLedName.Text = "No LEDs found!";
            return;
        }
        else
            txInfoLedName.Text = "Select LED from dropdown to continue.";

        /* Add all names to the ComboBox */
        foreach (string name in LedNames)
        {
            cbLedNames.Items.Add(name);
        }

        /* Select LedName in ComboBox */
        if (!string.IsNullOrEmpty(LedName) && LedNames.Contains(LedName))
        {
            cbLedNames.SelectedItem = LedName;
        }
        else
            cbLedNames.SelectedIndex = 0;        
    }

    private void CbLedNames_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbLedNames.SelectedItem != null && !string.IsNullOrEmpty(cbLedNames.SelectedItem.ToString()))
        {
            /* Set ledName */
            LedName = cbLedNames.SelectedItem.ToString()!;
            tbLedName.Text = LedName;
            /* Close dropdown */
            cbLedNames.IsDropDownOpen = false;

            ActivateButtonLed(true);
        }
        else
            ActivateButtonLed(false);
    }

    private void BtnLed_Clicked(object sender, RoutedEventArgs args)
    {
        if (!ledBlinkIsActive)
        {
            if (!string.IsNullOrEmpty(LedName))
            {
                /* Create new thread, let LED blink */
                Thread ledBlinkThread = new(() => Led.StartLedBlink(LedName));
                ledBlinkThread.Start();
                ledBlinkIsActive = true;
                /* Change UI */
                btnLed.Content = "Stop LED";
                btnLed.Background = Brushes.Red;
                txInfoLed.Text = $"LED {LedName} blinks";
            }
            else
                txInfoLed.Text = "Select LED from Dropdown";
        }
        else
        {
            /* Create new thread, stop LED blink */
            Thread stopBlinkThread = new(Led.StopLedBlink);
            stopBlinkThread.Start();
            ledBlinkIsActive = false;
            /* Change UI */
            btnLed.Content = "Blink LED";
            btnLed.Background = Brushes.LightGreen;
            txInfoLed.Text = $"LED {LedName} is off";
        }
    }

    private void ActivateButtonLed(bool activate)
    {
        if (activate)
        {
            btnLed.IsEnabled = true;
            txDescLed.Text = "Selected LED will blink until stopped.";
        }
        else
        {
            btnLed.IsEnabled = false;
            txDescLed.Text = "Select LED first";
        }
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnLedName.AddHandler(Button.ClickEvent, BtnLedName_Clicked!);
        btnLed.AddHandler(Button.ClickEvent, BtnLed_Clicked!);
    }

    private void FillTextBlockWithText()
    {
        txDescLedName.Text = "Find all available LEDs on your Board.";
        txDescLed.Text = "Select LED first";
        txInfoLedName.Text = "";
        txInfoLed.Text = "";
    }

    private void SetupComboBox()
    {
        cbLedNames.AddHandler(ComboBox.SelectionChangedEvent, CbLedNames_SelectionChanged!);
    }
}
