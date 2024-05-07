/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Br�gel                               *
*                                                       *
*    This file is part of dotnetIoT_Demo.               *
*                                                       *
*********************************************************/

using System;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using dotnetIot_Demo.Models.Tools;
using dotnetIot_Demo.Models.Hardware;

namespace dotnetIot_Demo.Views;

public partial class UserControl_Can : UserControl
{
    /* CAN functions are in a separate class */
    private Can_Demo? Can;
    /* Standard values */
    private string canDevNo = "0"; //can0
    private string canDevice = "can0";
    private string bitrate = "1000000";
    private byte[] valueSend = [1, 2, 3, 40, 50, 60, 70, 80];
    private uint canIdWrite = 0x1a;
    /* values that are read */
    private byte[] valueRead = [];
    private uint canIdRead;
    
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
            Can = new Can_Demo(canDevice, bitrate, canIdWrite);
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
        valueRead = [];
        canIdRead = new();

        /* take values from TextBoxes, insert them into byte array */
        valueSend = ValuesToByteArray();

        try
        {
            /* Run Read/Write test */
            (valueRead, canIdRead) = Can!.StartRWTest(valueSend);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoCanRW.Text = ex.Message;
            txInfoCanRW.Foreground = Brushes.Red;
            return;
        }

        /* Convert the byte array values to strings to display in UI */
        txCanWrite.Text = "CAN Write - " + CreateResultString(canIdWrite, valueSend);
        txCanRead.Text = "CAN Read - " + CreateResultString(canIdRead, valueRead);

        /* Compare CanIds and byte arrays - CanId must be different, but byte array the same */
        if (canIdRead != canIdWrite && Helper.ByteArraysEqual(valueRead, valueSend))
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
            canDevNo = tbCanDev.Text;
        else
            tbCanDev.Text = canDevNo;
        
        if (!string.IsNullOrEmpty(tbBitrate.Text))
            bitrate = tbBitrate.Text;
        else
            tbBitrate.Text = bitrate;
        
        if (!string.IsNullOrEmpty(tbCanId.Text))
            canIdWrite = Helper.ConvertStringToUInt(tbCanId.Text);
        else
            tbCanId.Text = canIdWrite.ToString("X");

        canDevice = $"can{canDevNo}";
    }

    private void ActivateButtonCanRW(bool activate)
    {
        if (activate)
        {
            btnCanRW.IsEnabled = true;
            txDescCanRW.Text = "External CAN device must echo the received message!\r\n" +
                "Use this command in Linux on your external device:\r\n" +
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
            if (tbCanDev.Text == canDevNo.ToString())
                ActivateButtonCanRW(true);
            else
                ActivateButtonCanRW(false);
        }
        else if (sender == tbCanId)
        {
            if (tbCanId.Text == canIdWrite.ToString("X"))
                ActivateButtonCanRW(true);
            else
                ActivateButtonCanRW(false);
        }
        else if (sender == tbBitrate)
        {
            if (tbBitrate.Text == bitrate.ToString())
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
        tbCanDev.Text = canDevNo.ToString();
        tbCanId.Text = canIdWrite.ToString("X");
        tbBitrate.Text = bitrate.ToString();
        tbVal0.Text = valueSend[0].ToString();
        tbVal1.Text = valueSend[1].ToString();
        tbVal2.Text = valueSend[2].ToString();
        tbVal3.Text = valueSend[3].ToString();
        tbVal4.Text = valueSend[4].ToString();
        tbVal5.Text = valueSend[5].ToString();
        tbVal6.Text = valueSend[6].ToString();
        tbVal7.Text = valueSend[7].ToString();
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
