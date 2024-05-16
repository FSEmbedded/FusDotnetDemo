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

    /* Default values from boardvalues.json */
    /* IDs and addresses */
    private int BusIdRW = DefaultValues.I2cBusIdRW;
    private int BusIdLed = DefaultValues.I2cBusIdLed;
    private int BusIdPwm = DefaultValues.I2cBusIdPwm;
    private int BusIdAdc = DefaultValues.I2cBusIdAdc;
    private int DeviceAddrRW = DefaultValues.I2cDeviceAddrRW;
    private int DeviceAddrLed = DefaultValues.I2cDeviceAddrLed;
    private int DeviceAddrPwm = DefaultValues.I2cDeviceAddrPwm;
    private int DeviceAddrAdc = DefaultValues.I2cDeviceAddrAdc;
    /* Values to write to I2C device */
    private byte ValueWrite1 = DefaultValues.I2cValueWrite1;
    private byte ValueWrite2 = DefaultValues.I2cValueWrite2;
    private byte Register1 = DefaultValues.I2cRegister1;
    private byte Register2 = DefaultValues.I2cRegister2;

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
            I2c = new I2c_Demo(BusIdRW, DeviceAddrRW);
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
           I2c!.WriteValueToRegister(Register1, ValueWrite1);
           I2c!.WriteValueToRegister(Register2, ValueWrite2);
        }
        catch (Exception ex)
        {
            txInfoWrite.Text = ex.Message;
            txInfoWrite.Foreground = Brushes.Red;
            txInfoRead.Text = "";
            return;
        }

        txInfoWrite.Text = $"Values 0x{ValueWrite1:X} & 0x{ValueWrite2:X} sent to I�C Device";
        txInfoWrite.Foreground = Brushes.Blue;

        /* Read values from I2C Device */
        byte valueRead1 = I2c.ReadValueFromRegister(Register1);
        byte valueRead2 = I2c.ReadValueFromRegister(Register2);

        /* Check if values read and write are equal */
        if (valueRead1 == ValueWrite1 && valueRead2 == ValueWrite2)
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
                I2c = new I2c_Demo(BusIdLed, DeviceAddrLed);
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
            I2cPwm = new I2c_Demo(BusIdPwm, DeviceAddrPwm);
            /* Create new object I2c_Tests for ADC device */
            I2cAdc = new I2c_Demo(BusIdAdc, DeviceAddrAdc);
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
                    BusIdRW = Helper.ConvertHexStringToInt(tbBusIdRW.Text, BusIdRW);
                else
                    tbBusIdRW.Text = BusIdRW.ToString("X");
                if (!string.IsNullOrEmpty(tbDevAddrRW.Text))
                    DeviceAddrRW = Helper.ConvertHexStringToInt(tbDevAddrRW.Text, DeviceAddrRW);
                else
                    tbDevAddrRW.Text = DeviceAddrRW.ToString("X");
                if (!string.IsNullOrEmpty(tbValue1.Text))
                    ValueWrite1 = Helper.ConvertHexStringToByte(tbValue1.Text, ValueWrite1);
                else
                    tbValue1.Text = ValueWrite1.ToString("X");
                if (!string.IsNullOrEmpty(tbValue2.Text))
                    ValueWrite2 = Helper.ConvertHexStringToByte(tbValue2.Text, ValueWrite2);
                else
                    tbValue2.Text = ValueWrite2.ToString("X");
                if (!string.IsNullOrEmpty(tbReg1.Text))
                    Register1 = Helper.ConvertHexStringToByte(tbReg1.Text, Register1);
                else
                    tbReg1.Text = Register1.ToString("X");
                if (!string.IsNullOrEmpty(tbReg2.Text))
                    Register2 = Helper.ConvertHexStringToByte(tbReg2.Text, Register2);
                else
                    tbReg2.Text = Register2.ToString("X");
                break;
            /* BtnI2cLed_Clicked() */
            case 1:
                if (!string.IsNullOrEmpty(tbBusIdLed.Text))
                    BusIdLed = Helper.ConvertHexStringToInt(tbBusIdLed.Text, BusIdLed);
                else
                    tbBusIdLed.Text = BusIdLed.ToString("X");
                if (!string.IsNullOrEmpty(tbDevAddrLed.Text))
                    DeviceAddrLed = Helper.ConvertHexStringToInt(tbDevAddrLed.Text, DeviceAddrLed);
                else
                    tbDevAddrLed.Text = DeviceAddrLed.ToString("X");
                break;
            /* BtnI2cPwm_Clicked() */
            case 2:
                if (!string.IsNullOrEmpty(tbBusIdPwm.Text))
                    BusIdPwm = Helper.ConvertHexStringToInt(tbBusIdPwm.Text, BusIdPwm);
                else
                    tbBusIdPwm.Text = BusIdPwm.ToString("X");
                if (!string.IsNullOrEmpty(tbDevAddrPwm.Text))
                    DeviceAddrPwm = Helper.ConvertHexStringToInt(tbDevAddrPwm.Text, DeviceAddrPwm);
                else
                    tbDevAddrPwm.Text = DeviceAddrPwm.ToString("X");
                if (!string.IsNullOrEmpty(tbBusIdAdc.Text))
                    BusIdAdc = Helper.ConvertHexStringToInt(tbBusIdAdc.Text, BusIdAdc);
                else
                    tbBusIdAdc.Text = BusIdAdc.ToString("X");
                if (!string.IsNullOrEmpty(tbDevAddrAdc.Text))
                    DeviceAddrAdc = Helper.ConvertHexStringToInt(tbDevAddrAdc.Text, DeviceAddrAdc);
                else
                    tbDevAddrAdc.Text = DeviceAddrAdc.ToString("X");
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
        tbBusIdRW.Text = BusIdRW.ToString("X");
        tbDevAddrRW.Text = DeviceAddrRW.ToString("X");
        tbValue1.Text = ValueWrite1.ToString("X");
        tbValue2.Text = ValueWrite2.ToString("X");
        tbReg1.Text = Register1.ToString("X");
        tbReg2.Text = Register2.ToString("X");
        tbBusIdLed.Text = BusIdLed.ToString("X");
        tbDevAddrLed.Text = DeviceAddrLed.ToString("X");
        tbBusIdPwm.Text = BusIdPwm.ToString("X");
        tbDevAddrPwm.Text = DeviceAddrPwm.ToString("X");
        tbBusIdAdc.Text = BusIdAdc.ToString("X");
        tbDevAddrAdc.Text = DeviceAddrAdc.ToString("X");
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
