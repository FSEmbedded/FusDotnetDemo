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
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FusDotnetDemo.Models.Tools;
using FusDotnetDemo.Models.Hardware;

namespace FusDotnetDemo.Views;

public partial class UserControl_I2c : UserControl
{
    /* I2C functions are in a separate class */
    I2c_Demo? I2c;
    I2c_Demo? I2cPwm;
    I2c_Demo? I2cAdc;
    /* Standard IDs and addresses */
    private int busIdRW = DefaultBoardValues.BusId;
    private int devAddrRW = DefaultBoardValues.DevAddrRW;
    private int busIdLed = DefaultBoardValues.BusId;
    private int devAddrLed = DefaultBoardValues.DevAddrLed;
    private int busIdPwm = DefaultBoardValues.BusId;
    private int devAddrPwm = DefaultBoardValues.DevAddrPwm;
    private int busIdAdc = DefaultBoardValues.BusId;
    private int devAddrAdc = DefaultBoardValues.DevAddrAdc;
    /* Values to write to I2C device */
    private byte valueWrite1 = DefaultBoardValues.ValueWrite1;
    private byte valueWrite2 = DefaultBoardValues.ValueWrite2;
    private byte register1 = DefaultBoardValues.I2cRegister1;
    private byte register2 = DefaultBoardValues.I2cRegister2;

    private bool ledThreadStarted = false;

    public UserControl_I2c()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
    }

    private void BtnI2cRW_Clicked(object sender, RoutedEventArgs args)
    {
        /* Get values from UI */
        GetValuesFromTextBox(0);

        try
        {
            /* Create new object I2c_Tests */
            I2c = new I2c_Demo(busIdRW, devAddrRW);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoWrite.Text = ex.Message;
            txInfoWrite.Foreground = Brushes.Red;
            txInfoRead.Text = "";
            return;
        }

        try
        {
            /* Write values to I2C Device */
           I2c!.WriteValueToRegister(register1, valueWrite1);
           I2c!.WriteValueToRegister(register2, valueWrite2);
        }
        catch (Exception ex)
        {
            txInfoWrite.Text = ex.Message;
            txInfoWrite.Foreground = Brushes.Red;
            txInfoRead.Text = "";
            return;
        }

        txInfoWrite.Text = $"Values 0x{valueWrite1:X} & 0x{valueWrite2:X} sent to I�C Device";
        txInfoWrite.Foreground = Brushes.Blue;

        /* Read values from I2C Device */
        byte valueRead1 = I2c.ReadValueFromRegister(register1);
        byte valueRead2 = I2c.ReadValueFromRegister(register2);

        /* Check if values read and write are equal */
        if (valueRead1 == valueWrite1 && valueRead2 == valueWrite2)
        {
            txInfoRead.Text = $"Values 0x{valueRead1:X} & 0x{valueRead2:X} read from I�C Device";
            txInfoRead.Foreground = Brushes.Green;
        }
        else
        {
            txInfoRead.Text = $"Values 0x{valueRead1:X} & 0x{valueRead2:X} don't match the values written to I�C Device";
            txInfoRead.Foreground = Brushes.Red;
        }
    }

    private void BtnI2cLed_Clicked(object sender, RoutedEventArgs args)
    {
        if (!ledThreadStarted)
        {
            /* Get values from UI */
            GetValuesFromTextBox(1);

            try
            {
                /* Create new object I2c_Tests */
                I2c = new I2c_Demo(busIdLed, devAddrLed);
            }
            catch (Exception ex)
            {
                /* Show exception */
                txInfoLed.Text = ex.Message;
                txInfoLed.Foreground = Brushes.Red;
                return;
            }

            /* Create new Thread, start I2C Test */
            Thread I2cLedThread = new(() => I2c!.WriteValuesLed());
            I2cLedThread.Start();
            ledThreadStarted = true;
            /* Change UI */
            btnI2cLed.Content = "Stop LED Test";
            btnI2cLed.Background = Brushes.Red;
            txInfoLed.Text = "LEDs on I�C-Extension Board are blinking";
            txInfoLed.Foreground = Brushes.Blue;
        }
        else
        {
            /* Stop the I2C Thread from running */
            I2c!.StopLedLoop();
            ledThreadStarted = false;
            /* Change UI */
            btnI2cLed.Content = "Start LED Test";
            btnI2cLed.Background = Brushes.LightGreen;
            txInfoLed.Text = "LEDs on I�C-Extension Board stopped blinking";
            txInfoLed.Foreground = Brushes.Blue;
        }
    }

    private void BtnI2cPwm_Clicked(object sender, RoutedEventArgs args)
    {
        /* Empty TextBlock */
        txInfoPwm.Text = "";
        txInfoPwm.Foreground = Brushes.Blue;

        /* Get values from UI */
        GetValuesFromTextBox(2);

        try
        {
            /* Create new object I2c_Tests for PWM device */
            I2cPwm = new I2c_Demo(busIdPwm, devAddrPwm);
            /* Create new object I2c_Tests for ADC device */
            I2cAdc = new I2c_Demo(busIdAdc, devAddrAdc);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoPwm.Text = ex.Message;
            txInfoPwm.Foreground = Brushes.Red;
            return;
        }

        int counter = 0;
        bool toggleOn = false;

        while (counter < 6)
        {
            /* Toggle PWM */
            I2cPwm!.PwmSwitchOnOff(toggleOn);
            toggleOn = !toggleOn;

            /* Read ADC */
            byte adcValue = I2cAdc!.ReadADC();

            /* Calculate voltage from adcvalue */
            byte maxValue = 0xFF; /* Maximum value of the ADC */
            double maxVoltage = 3.3; /* Maximum voltage of the PWM */
            double voltage = ((double)adcValue / (double)maxValue) * maxVoltage;

            /* Write results in UI */
            UpdateUI(voltage, toggleOn);
            
            counter++;
            Thread.Sleep(1000);
        }
    }

    public void UpdateUI(double voltage, bool toggleOn)
    {
        /* If PWM switched on the OnBoard-LED, returnValue should be 0 */
        if (toggleOn)
        {
            txInfoPwm.Text += $"PWM on - ADC: {voltage:F4} V\r\n";
        }
        else if (!toggleOn)
        {
            txInfoPwm.Text += $"PWM off - ADC: {voltage:F4} V\r\n";
        }
    }

    private void GetValuesFromTextBox(int callerId)
    {
        switch (callerId)
        {
            /* BtnI2cRW_Clicked() */
            case 0:
                if (!string.IsNullOrEmpty(tbBusIdRW.Text))
                    busIdRW = Helper.ConvertHexStringToInt(tbBusIdRW.Text, busIdRW);
                else
                    tbBusIdRW.Text = busIdRW.ToString("X");
                if (!string.IsNullOrEmpty(tbDevAddrRW.Text))
                    devAddrRW = Helper.ConvertHexStringToInt(tbDevAddrRW.Text, devAddrRW);
                else
                    tbDevAddrRW.Text = devAddrRW.ToString("X");
                if (!string.IsNullOrEmpty(tbValue1.Text))
                    valueWrite1 = Helper.ConvertHexStringToByte(tbValue1.Text, valueWrite1);
                else
                    tbValue1.Text = valueWrite1.ToString("X");
                if (!string.IsNullOrEmpty(tbValue2.Text))
                    valueWrite2 = Helper.ConvertHexStringToByte(tbValue2.Text, valueWrite2);
                else
                    tbValue2.Text = valueWrite2.ToString("X");
                if (!string.IsNullOrEmpty(tbReg1.Text))
                    register1 = Helper.ConvertHexStringToByte(tbReg1.Text, register1);
                else
                    tbReg1.Text = register1.ToString("X");
                if (!string.IsNullOrEmpty(tbReg2.Text))
                    register2 = Helper.ConvertHexStringToByte(tbReg2.Text, register2);
                else
                    tbReg2.Text = register2.ToString("X");
                break;
            /* BtnI2cLed_Clicked() */
            case 1:
                if (!string.IsNullOrEmpty(tbBusIdLed.Text))
                    busIdLed = Helper.ConvertHexStringToInt(tbBusIdLed.Text, busIdLed);
                else
                    tbBusIdLed.Text = busIdLed.ToString("X");
                if (!string.IsNullOrEmpty(tbDevAddrLed.Text))
                    devAddrLed = Helper.ConvertHexStringToInt(tbDevAddrLed.Text, devAddrLed);
                else
                    tbDevAddrLed.Text = devAddrLed.ToString("X");
                break;
            /* BtnI2cPwm_Clicked() */
            case 2:
                if (!string.IsNullOrEmpty(tbBusIdPwm.Text))
                    busIdPwm = Helper.ConvertHexStringToInt(tbBusIdPwm.Text, busIdPwm);
                else
                    tbBusIdPwm.Text = busIdPwm.ToString("X");
                if (!string.IsNullOrEmpty(tbDevAddrPwm.Text))
                    devAddrPwm = Helper.ConvertHexStringToInt(tbDevAddrPwm.Text, devAddrPwm);
                else
                    tbDevAddrPwm.Text = devAddrPwm.ToString("X");
                if (!string.IsNullOrEmpty(tbBusIdAdc.Text))
                    busIdAdc = Helper.ConvertHexStringToInt(tbBusIdAdc.Text, busIdAdc);
                else
                    tbBusIdAdc.Text = busIdAdc.ToString("X");
                if (!string.IsNullOrEmpty(tbDevAddrAdc.Text))
                    devAddrAdc = Helper.ConvertHexStringToInt(tbDevAddrAdc.Text, devAddrAdc);
                else
                    tbDevAddrAdc.Text = devAddrAdc.ToString("X");
                break;
        }
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnI2cRW.AddHandler(Button.ClickEvent, BtnI2cRW_Clicked!);
        btnI2cLed.AddHandler(Button.ClickEvent, BtnI2cLed_Clicked!);
        btnI2cPwm.AddHandler(Button.ClickEvent, BtnI2cPwm_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes*/
        tbBusIdRW.Text = busIdRW.ToString("X");
        tbDevAddrRW.Text = devAddrRW.ToString("X");
        tbValue1.Text = valueWrite1.ToString("X");
        tbValue2.Text = valueWrite2.ToString("X");
        tbReg1.Text = register1.ToString("X");
        tbReg2.Text = register2.ToString("X");
        tbBusIdLed.Text = busIdLed.ToString("X");
        tbDevAddrLed.Text = devAddrLed.ToString("X");
        tbBusIdPwm.Text = busIdPwm.ToString("X");
        tbDevAddrPwm.Text = devAddrPwm.ToString("X");
        tbBusIdAdc.Text = busIdAdc.ToString("X");
        tbDevAddrAdc.Text = devAddrAdc.ToString("X");
    }

    private void AddTextBoxHandlers()
    {
        /* Add handler to only allow hex value inputs */
        tbBusIdRW.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbDevAddrRW.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbValue1.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbValue2.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbReg1.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbReg2.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbBusIdLed.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbDevAddrLed.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbBusIdPwm.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbDevAddrPwm.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbBusIdAdc.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbDevAddrAdc.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        /* Description Text */
        txDescRW.Text = "Write the values to the I²C-Device at the defined Bus-ID and address.";
        txInfoWrite.Text = "";
        txInfoRead.Text = "";
        txDescLed.Text = "Writes multiple values in a loop to the I²C-Device. On the I²C Extension Board you will get an LED chaser.";
        txInfoLed.Text = "";
        txDescPwm.Text = "Toggles the PWM multiple times and reads the values measured by the ADC.";
        txInfoPwm.Text = "";
    }
}
