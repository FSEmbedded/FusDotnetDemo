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
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FusDotnetDemo.Models.Tools;
using FusDotnetDemo.Models.Hardware;
using Iot.Device.Ssd13xx.Commands;

namespace FusDotnetDemo.Views;

public partial class UserControl_Can : UserControl
{
    /* CAN functions are in a separate class */
    private Can_Demo? Can;
    
    private string CanDevice = string.Empty;

    /* values that are read */
    private byte[] ValuesRead = [];
    private uint CanIdRead;

    /* Default values from boardvalues.json */
    private string CanDeviceNo = DefaultValues.CanDeviceNo;
    private uint CanIdWrite = DefaultValues.CanIdWrite;
    private string Bitrate = DefaultValues.CanBitrate;
    private byte[] ValuesSend = DefaultValues.CanValuesSend;

    public UserControl_Can()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
        /* Button will be enabled after CAN is activated */
        ActivateButtonCanRW(false);
    }

    private void BtnCanAct_Clicked(object sender, RoutedEventArgs args)
    {
        /* Read values from TextBoxes */
        GetValuesFromTextBox();

        try
        {
            /* Create new object Can_Tests
             * Will activate CAN device in constructor
             * Writes to external CAN device to check if connection is established */
            Can = new Can_Demo(CanDevice, Bitrate, CanIdWrite);
            txInfoCanAct.Text = "CAN device is active and connection to receiving device is validated";
            txInfoCanAct.Foreground = Brushes.Green;
            ActivateButtonCanRW(true);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoCanAct.Text = ex.Message;
            txInfoCanAct.Foreground = Brushes.Red;
            return;
        }
    }

    private void BtnCanRW_Clicked(object sender, RoutedEventArgs args)
    {
        /* Reset values */
        ValuesRead = [];
        CanIdRead = new();

        /* take values from TextBoxes, insert them into byte array */
        ValuesSend = ValuesToByteArray();

        try
        {
            /* Run Read/Write test */
            (ValuesRead, CanIdRead) = Can!.StartRWTest(ValuesSend);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoCanRW.Text = ex.Message;
            txInfoCanRW.Foreground = Brushes.Red;
            return;
        }

        /* Convert the byte array values to strings to display in UI */
        txCanWrite.Text = "CAN Write - " + CreateResultString(CanIdWrite, ValuesSend);
        txCanRead.Text = "CAN Read - " + CreateResultString(CanIdRead, ValuesRead);

        /* Compare CanIds and byte arrays - CanId must be different, but byte array the same */
        if (CanIdRead != CanIdWrite && Helper.ByteArraysEqual(ValuesRead, ValuesSend))
        {
            txInfoCanRW.Text = $"CAN Test Success!\r\nCAN ID differs while values are the same";
            txInfoCanRW.Foreground = Brushes.Green;
        }
        else
        {
            txInfoCanRW.Text = "CAN Test Failed\r\n" +
                "Receiving device must echo the values written to it, but with a different CAN ID!";
            txInfoCanRW.Foreground = Brushes.Red;
        }
    }

    private static string CreateResultString(uint id, byte[] bytes)
    {
        /* Convert the byte array values to strings, to display in UI */
        string result = $"CAN ID: {id:X} - " +
            $"Values: ";

        for(int i = 0; i < bytes.Length; i++)
        {
            result += $"0x{bytes[i]:X} ";
        }

        return result;
    }

    private byte[] ValuesToByteArray()
    {
        /* Get values from TextBoxes, add them to  byte array */
        byte[] bytes =
        [
            Helper.ConvertStringToByte(tbVal0.Text),
            Helper.ConvertStringToByte(tbVal1.Text),
            Helper.ConvertStringToByte(tbVal2.Text),
            Helper.ConvertStringToByte(tbVal3.Text),
            Helper.ConvertStringToByte(tbVal4.Text),
            Helper.ConvertStringToByte(tbVal5.Text),
            Helper.ConvertStringToByte(tbVal6.Text),
            Helper.ConvertStringToByte(tbVal7.Text),
        ];
        return bytes;
    }

    private void GetValuesFromTextBox()
    {
        /* Get CAN Device and Bitrate from TextBoxes */
        if (!string.IsNullOrEmpty(tbCanDev.Text))
            CanDeviceNo = tbCanDev.Text;
        else
            tbCanDev.Text = CanDeviceNo;
        
        if (!string.IsNullOrEmpty(tbBitrate.Text))
            Bitrate = tbBitrate.Text;
        else
            tbBitrate.Text = Bitrate;
        
        if (!string.IsNullOrEmpty(tbCanId.Text))
            CanIdWrite = Helper.ConvertStringToUInt(tbCanId.Text);
        else
            tbCanId.Text = CanIdWrite.ToString("X");

        CanDevice = $"can{CanDeviceNo}";
    }

    private void ActivateButtonCanRW(bool activate)
    {
        if (activate)
        {
            btnCanRW.IsEnabled = true;
            txDescCanRW.Text = "External CAN device must echo the received message!\r\n" +
                "Activate CAN on second device:\r\n" +
                "ip link set can0 up type can bitrate 1000000 && ifconfig can0 up\r\n" +
                "Use this command to return the message:\r\n" +
                "STRING=$(candump can0 -L -n1 | cut -d '#' -f2) && cansend can0 01b#${STRING}";
        }
        else
        {
            btnCanRW.IsEnabled = false;
            txDescCanRW.Text = "Activate CAN device first!";
        }
    }

    private void TextBoxValuesChanged(object sender, KeyEventArgs e)
    {
        /* (De)Activate btnCanRW if values for CAN device changed.
         * Button will be activated again if right value is entered 
         * or activation with new value is run again */
        if (sender == tbCanDev)
        {
            if (tbCanDev.Text == CanDeviceNo.ToString())
                ActivateButtonCanRW(true);
            else
                ActivateButtonCanRW(false);
        }
        else if (sender == tbCanId)
        {
            if (tbCanId.Text == CanIdWrite.ToString("X"))
                ActivateButtonCanRW(true);
            else
                ActivateButtonCanRW(false);
        }
        else if (sender == tbBitrate)
        {
            if (tbBitrate.Text == Bitrate.ToString())
                ActivateButtonCanRW(true);
            else
                ActivateButtonCanRW(false);
        }
        e.Handled = true;
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnCanAct.AddHandler(Button.ClickEvent, BtnCanAct_Clicked!);
        btnCanRW.AddHandler(Button.ClickEvent, BtnCanRW_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes*/
        tbCanDev.Text = CanDeviceNo.ToString();
        tbCanId.Text = CanIdWrite.ToString("X");
        tbBitrate.Text = Bitrate.ToString();
        tbVal0.Text = ValuesSend[0].ToString();
        tbVal1.Text = ValuesSend[1].ToString();
        tbVal2.Text = ValuesSend[2].ToString();
        tbVal3.Text = ValuesSend[3].ToString();
        tbVal4.Text = ValuesSend[4].ToString();
        tbVal5.Text = ValuesSend[5].ToString();
        tbVal6.Text = ValuesSend[6].ToString();
        tbVal7.Text = ValuesSend[7].ToString();
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbCanDev.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbCanId.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbBitrate.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbVal0.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal1.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal2.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal3.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal4.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal5.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal6.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal7.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        /* Additional handlers, will disable btnCanRW */
        tbCanDev.AddHandler(KeyUpEvent, TextBoxValuesChanged!, RoutingStrategies.Tunnel);
        tbCanId.AddHandler(KeyUpEvent, TextBoxValuesChanged!, RoutingStrategies.Tunnel);
        tbBitrate.AddHandler(KeyUpEvent, TextBoxValuesChanged!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        /* Description Text */
        txDescCanAct.Text = "Activate the selected CAN device on your board and validate the connection to the external CAN device.";
        txInfoCanAct.Text = "";
        txDescCanRW.Text = "Activate CAN device first!";
        txCanWrite.Text = "";
        txCanRead.Text = "";
        txInfoCanRW.Text = "";
    }
}
