/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Bruegel                              *
*                                                       *
*    This file is part of FusDotnetDemo.                *
*                                                       *
*********************************************************/

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FusDotnetDemo.Models.Tools;
using FusDotnetDemo.Models.Hardware;

namespace FusDotnetDemo.Views;

public partial class UserControl_Spi : UserControl
{
    /* SPI functions are in a separate class */
    private Spi_Demo? Spi;
    
    /* Values read from SPI device */
    private byte valueRead1;
    private byte valueRead2;

    /* Default values from boardvalues.json */
    private int SpiDevice = DefaultValues.SpiDevice;
    private byte Register1 = DefaultValues.SpiRegister1;
    private byte Register2 = DefaultValues.SpiRegister2;
    /* Values to write */
    private byte ValueWrite1 = DefaultValues.SpiValueWrite1;
    private byte ValueWrite2 = DefaultValues.SpiValueWrite2;

    public UserControl_Spi()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
    }

    private void BtnSpi_Clicked(object sender, RoutedEventArgs args)
    {
        /* Empty TextBlock */
        txInfoSpi.Text = "";
        txInfoSpiWrite.Text = "";
        txInfoSpiRead.Text = "";

        /* Convert values from UI to hex */
        GetValuesFromTextBox();

        try
        {
            /* Create new object Spi_Tests */
            Spi = new Spi_Demo(SpiDevice);
        }
        catch (Exception ex)
        {
            txInfoSpi.Text = ex.Message;
            txInfoSpi.Foreground = Brushes.Red;
            return;
        }

        /* SpiStart writes the values valueWrite1/2 to the defined registers on SPI device, 
         * returns the values that it reads in these registers */
        valueRead1 = Spi.StartSpiRWTest(Register1, ValueWrite1);
        valueRead2 = Spi.StartSpiRWTest(Register2, ValueWrite2);

        /* Write values into TextBlock */
        FillInfoTextBlock();

        /* Compare return values with values written */
        if (valueRead1 == ValueWrite1 && valueRead2 == ValueWrite2)
        {
            txInfoSpi.Text = $"SPI Test Success!\r\nRead and Write values are the same";
            txInfoSpi.Foreground = Brushes.Green;
        }
        else
        {
            txInfoSpi.Text = "SPI Test Failed!\r\nRead and Write values are different";
            txInfoSpi.Foreground = Brushes.Red;
        }
    }

    private void GetValuesFromTextBox()
    {
        /* Convert values from UI to hex */
        if (!string.IsNullOrEmpty(tbSpiDev.Text))
            SpiDevice = Helper.ConvertHexStringToInt(tbSpiDev.Text, SpiDevice);
        else
            tbSpiDev.Text = SpiDevice.ToString("X");
        try
        {
            if(!string.IsNullOrEmpty(tbRegister.Text))
                Register1 = Helper.ConvertHexStringToByte(tbRegister.Text, Register1);
            else
                tbRegister.Text = Register1.ToString("X");
            if (!string.IsNullOrEmpty(tbValue1.Text))
                ValueWrite1 = Helper.ConvertHexStringToByte(tbValue1.Text, ValueWrite1);
            else
                tbValue1.Text = ValueWrite1.ToString("X");
            if (!string.IsNullOrEmpty(tbValue2.Text))
                ValueWrite2 = Helper.ConvertHexStringToByte(tbValue2.Text, ValueWrite2);
            else
                tbValue2.Text = ValueWrite2.ToString("X");

            /* register2 is register1 + 1 */
            Register2 = Convert.ToByte(Register1 + 1);
        }
        catch (Exception ex)
        {
            txInfoSpi.Text = ex.Message;
            txInfoSpi.Foreground = Brushes.Red;
            return;
        }
    }

    private void FillInfoTextBlock()
    {
        txInfoSpiWrite.Text = $"SPI Write\r\n" +
                              $"SPI Device: 0x{SpiDevice:X}\r\n" +
                              $"Register 1: 0x{Register1:X}\r\n" +
                              $"Value 1: 0x{ValueWrite1:X}\r\n" +
                              $"Register 2: 0x{Register2:X}\r\n" +
                              $"Value2: 0x{ValueWrite2:X}";

        txInfoSpiRead.Text = $"SPI Read\r\n" +
                             $"SPI Device: 0x{SpiDevice:X}\r\n" +
                             $"Register 1: 0x{Register1:X}\r\n" +
                             $"Value 1: 0x{valueRead1:X}\r\n" +
                             $"Register 2: 0x{Register2:X}\r\n" +
                             $"Value2: 0x{valueRead2:X}";
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnSpi.AddHandler(Button.ClickEvent, BtnSpi_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes*/
        tbSpiDev.Text = SpiDevice.ToString("X");
        tbRegister.Text = Register1.ToString("X");
        tbValue1.Text = ValueWrite1.ToString("X");
        tbValue2.Text = ValueWrite2.ToString("X");
    }

    private void AddTextBoxHandlers()
    {
        /* Add handler to only allow decimal value inputs */
        tbSpiDev.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbRegister.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbValue1.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbValue2.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        /* Description Text */
        txDescSpi.Text = "Writes values to the SPI device, then reads values from the same device.";
        txInfoSpiWrite.Text = "";
        txInfoSpiRead.Text = "";
        txInfoSpi.Text = "";
    }
}
