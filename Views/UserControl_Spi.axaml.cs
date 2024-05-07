/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Brügel                               *
*                                                       *
*    This file is part of dotnetIoT_Demo.               *
*                                                       *
*********************************************************/

using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using dotnetIot_Demo.Models.Tools;
using dotnetIot_Demo.Models.Hardware;

namespace dotnetIot_Demo.Views;

public partial class UserControl_Spi : UserControl
{
    /* SPI functions are in a separate class */
    private Spi_Demo? Spi;
    private int spidev = 0x1;
    private byte register1 = 0x2b;
    private byte register2 = 0x2c;
    /* Values to write */
    private byte valueWrite1 = 0x5;
    private byte valueWrite2 = 0x6;
    /* Values read from SPI device */
    private byte valueRead1;
    private byte valueRead2;

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
            Spi = new Spi_Demo(spidev);
        }
        catch (Exception ex)
        {
            txInfoSpi.Text = ex.Message;
            txInfoSpi.Foreground = Brushes.Red;
            return;
        }

        /* SpiStart writes the values valueWrite1/2 to the defined registers on SPI device, 
         * returns the values that it reads in these registers */
        valueRead1 = Spi.StartSpiRWTest(register1, valueWrite1);
        valueRead2 = Spi.StartSpiRWTest(register2, valueWrite2);

        /* Write values into TextBlock */
        FillInfoTextBlock();

        /* Compare return values with values written */
        if (valueRead1 == valueWrite1 && valueRead2 == valueWrite2)
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
            spidev = Helper.ConvertHexStringToInt(tbSpiDev.Text, spidev);
        else
            tbSpiDev.Text = spidev.ToString("X");
        try
        {
            if(!string.IsNullOrEmpty(tbRegister.Text))
                register1 = Helper.ConvertHexStringToByte(tbRegister.Text, register1);
            else
                tbRegister.Text = register1.ToString("X");
            if (!string.IsNullOrEmpty(tbValue1.Text))
                valueWrite1 = Helper.ConvertHexStringToByte(tbValue1.Text, valueWrite1);
            else
                tbValue1.Text = valueWrite1.ToString("X");
            if (!string.IsNullOrEmpty(tbValue2.Text))
                valueWrite2 = Helper.ConvertHexStringToByte(tbValue2.Text, valueWrite2);
            else
                tbValue2.Text = valueWrite2.ToString("X");

            /* register2 is register1 + 1 */
            register2 = Convert.ToByte(register1 + 1);
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
                              $"SPI Device: 0x{spidev:X}\r\n" +
                              $"Register 1: 0x{register1:X}\r\n" +
                              $"Value 1: 0x{valueWrite1:X}\r\n" +
                              $"Register 2: 0x{register2:X}\r\n" +
                              $"Value2: 0x{valueWrite2:X}";

        txInfoSpiRead.Text = $"SPI Read\r\n" +
                             $"SPI Device: 0x{spidev:X}\r\n" +
                             $"Register 1: 0x{register1:X}\r\n" +
                             $"Value 1: 0x{valueRead1:X}\r\n" +
                             $"Register 2: 0x{register2:X}\r\n" +
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
        tbSpiDev.Text = spidev.ToString("X");
        tbRegister.Text = register1.ToString("X");
        tbValue1.Text = valueWrite1.ToString("X");
        tbValue2.Text = valueWrite2.ToString("X");
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
