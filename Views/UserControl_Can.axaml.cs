using System;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using DynamicData;
using Iot.Device.SocketCan;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_Can : UserControl
{
    /* CAN functions are in separate class */
    private readonly Can_Tests Can;
    /* Standard values */
    private string canDevice = "0"; //can0
    private string bitrate = "1000000";
    private byte[] valueSend = [1, 2, 3, 40, 50, 60, 70, 80];
    /* value that is read will be stored in this byte array */
    private byte[] valueRead = [];

    public UserControl_Can()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();

        /* Create new Can_Tests */
        Can = new Can_Tests();
    }

    private void BtnCan_Clicked(object sender, RoutedEventArgs args)
    {
        /* Get CAN Device and Bitrate from TextBoxes */
        if(!string.IsNullOrEmpty(tbCanDev.Text))
            canDevice = tbCanDev.Text;
        if (!string.IsNullOrEmpty(tbBitrate.Text))
            bitrate = tbBitrate.Text;
        /* take values from TextBoxes, insert them into byte array */
        valueSend = ValuesToByteArray();

        CanId canIdRead;
        CanId canIdWrite = new()
        {
            Standard = 0x1A
        };

        try
        {
            /* Run Read/Write test */
            (valueRead, canIdRead) = Can.StartCanRWTest(canDevice, bitrate, canIdWrite, valueSend);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoCan.Text = ex.Message;
            txInfoCan.Foreground = Brushes.Red;
            return;
        }

        /* Compare CanIds and byte arrays - CanId must be different, but byte array the same */
        if (canIdRead.Value != canIdWrite.Value && Helper.ByteArraysEqual(valueRead, valueSend))
        {
            /* Convert the byte array values to strings, to display in UI */
            txCanWrite.Text = "CAN Write\r\n" + CreateResultString(canIdWrite, valueSend);
            txCanRead.Text = "CAN Read\r\n" + CreateResultString(canIdRead, valueRead);

            txInfoCan.Text = $"CAN Test Success!\r\nCAN ID differs while values are the same";
            txInfoCan.Foreground = Brushes.Green;
        }
        else
        {
            txCanWrite.Text = "";
            txCanRead.Text = "";

            txInfoCan.Text = "CAN Test Failed\r\n" +
                "Is receiving device connected and CAN activated?";
            txInfoCan.Foreground = Brushes.Red;
        }
    }

    private string CreateResultString(CanId id, byte[] bytes)
    {
        /* Convert the byte array values to strings, to display in UI */
        string result = $"CAN ID: {id.Value:X}\r\n" +
            $"Values:\r\n";

        for(int i = 0; i < bytes.Length; i++)
        {
            result += $"0x{bytes[i]:X} ";
            /* insert new line after half */
            if (i == (bytes.Length/2)-1)
                result += "\r\n";
        }

        return result;
    }
    private byte[] ValuesToByteArray()
    {
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

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnCan.AddHandler(Button.ClickEvent, BtnCan_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes*/
        tbCanDev.Text = Convert.ToString(canDevice);
        tbBitrate.Text = Convert.ToString(bitrate);
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
        tbBitrate.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbVal0.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal1.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal2.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal3.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal4.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal5.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal6.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbVal7.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        /* Description Text */
        txDescCan.Text = "Connect second board, CAN_L - CAN_L & CAN_H - CAN_H\r\n" +
            "On second device , run following comand under Linux to activate can0:\r\n" +
            "ip link set can0 up type can bitrate 1000000 && ifconfig can0 up\r\n" +
            "Run this command while CAN test is running to return the received value:\r\n" +
            "STRING=$(candump can0 -L -n1 | cut -d '#' -f2) && cansend can0 01b#${STRING}";

        txCanWrite.Text = "";
        txCanRead.Text = "";
        txInfoCan.Text = "";
    }
}
