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
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FusDotnetDemo.Models.Tools;
using FusDotnetDemo.Models.Hardware;

namespace FusDotnetDemo.Views;

public partial class UserControl_Uart : UserControl
{
    /* UART functions are in a separate class */
    Uart_Demo? UartSender;
    Uart_Demo? UartReceiver;

    /* Standard values */
    private string portSender = DefaultBoardValues.UartPortSender;
    private string portReceiver = DefaultBoardValues.UartPortReceiver;
    private string valueWrite = DefaultBoardValues.TestMessage;
    private int baudrate = DefaultBoardValues.Baudrate;
    private int dataBit = DefaultBoardValues.DataBit;
    private double stopBit = DefaultBoardValues.StopBit;
    private string parity = DefaultBoardValues.Parity;
    private string handshake = DefaultBoardValues.Handshake;

    private bool senderSet = false;
    private bool receiverSet = false;
    private string valueRead = "";

    /* Values to fill in ComboBoxes */
    private readonly List<int> baudrates = DefaultBoardValues.Baudrates;
    private readonly List<int> dataBits = DefaultBoardValues.DataBits;
    private readonly List<double> stopBits = DefaultBoardValues.StopBits;
    private readonly List<string> parities = DefaultBoardValues.Parities;
    private readonly List<string> handshakes = DefaultBoardValues.Handshakes;

    public UserControl_Uart()
    {
        InitializeComponent();
        AddButtonHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
        SetupComboBox();
        ActivateButtonUart();
    }

    private void BtnGetSerialPorts_Clicked(object sender, RoutedEventArgs args)
    {
        /* Find all available Serial Ports */
        List<string> ports = Uart_Demo.GetAvailableSerialPorts();

        /* Add all available ports to the ComboBoxes */
        cbUartSender.ItemsSource = ports;
        cbUartReceiver.ItemsSource = ports;

        /* Select port for Sender and Receiver in ComboBox */
        if (!string.IsNullOrEmpty(portSender) && ports.Contains(portSender))
            cbUartSender.SelectedItem = portSender;
        else
            cbUartSender.SelectedIndex = 0;
        
        if (!string.IsNullOrEmpty(portReceiver) && ports.Contains(portReceiver))
            cbUartReceiver.SelectedItem = portReceiver;
        else
            cbUartReceiver.SelectedIndex = 0;

        txInfoSerialPorts.Text = "Select Sender and Receiver Port from the dropdowns to continue.";
    }

    private void BtnUartRW_Clicked(object sender, RoutedEventArgs args)
    {
        /* Get message to write */
        GetValuesFromTextBox();

        try
        {
            /* Create new objects Uart_Tests for read and write */
            UartSender = new Uart_Demo(portSender, baudrate, dataBit, stopBit, parity, handshake);
            UartReceiver = new Uart_Demo(portReceiver, baudrate, dataBit, stopBit, parity, handshake);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoUart.Text = ex.Message;
            txInfoUart.Foreground = Brushes.Red;
            return;
        }

        /* Create new UART read thread*/
        Thread uartReadThread = new(() => { valueRead = UartReceiver.Read(); });
        /* Create new UART write thread*/
        Thread uartWriteThread = new(() => UartSender.Write(valueWrite));
        
        /* Start threads */
        uartReadThread.Start();
        uartWriteThread.Start();

        /* Get value for messageRead */
        uartReadThread.Join();

        /* Show results in UI */
        txUartSend.Text = $"Message sent on port {portSender}: {valueWrite}";
        txUartReceive.Text = $"Message read on port {portReceiver}: {valueRead}";

        if (valueWrite == valueRead)
        {
            txInfoUart.Text = "Values sent and read are equal";
            txInfoUart.Foreground = Brushes.Green;
        }
        else
        {
            txInfoUart.Text = "Values sent and read are different";
            txInfoUart.Foreground = Brushes.Red;
        }
    }

    private void ActivateButtonUart()
    {
        if (senderSet && receiverSet)
        {
            btnUartRW.IsEnabled = true;
            txDescUart.Text = "Send values from UART Sender to Receiver. Sender TX and Receiver RX must be connected!";
        }
        else
        {
            btnUartRW.IsEnabled = false;
            txDescUart.Text = "Select Sender and Receiver Ports first";
        }
    }

    private void CbUartSender_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbUartSender.SelectedItem != null && !string.IsNullOrEmpty(cbUartSender.SelectedItem.ToString()))
        {
            /* Set Sender Port */
            portSender = cbUartSender.SelectedItem.ToString()!;
            /* Close dropdown */
            cbUartSender.IsDropDownOpen = false;
            senderSet = true;
            ActivateButtonUart();
        }
        else
        {
            senderSet = false;
            ActivateButtonUart();
        }
    }

    private void CbUartReceiver_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbUartReceiver.SelectedItem != null && !string.IsNullOrEmpty(cbUartReceiver.SelectedItem.ToString()))
        {
            /* Set Receiver Port */
            portReceiver = cbUartReceiver.SelectedItem.ToString()!;
            /* Close dropdown */
            cbUartReceiver.IsDropDownOpen = false;
            receiverSet = true;
            ActivateButtonUart();
        }
        else
        {
            receiverSet = false;
            ActivateButtonUart();
        }
    }

    private void CbDataBit_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbDataBit.SelectedItem != null)
        {
            /* Set DataBit */
            dataBit = dataBits[cbDataBit.SelectedIndex];
            /* Close dropdown */
            cbDataBit.IsDropDownOpen = false;
        }
    }

    private void CbStopBit_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbStopBit.SelectedItem != null)
        {
            /* Set StopBit */
            stopBit = stopBits[cbStopBit.SelectedIndex];
            /* Close dropdown */
            cbStopBit.IsDropDownOpen = false;
        }
    }

    private void CbBaudrate_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbBaudrate.SelectedItem != null)
        {
            /* Set Baudrate */
            baudrate = baudrates[cbBaudrate.SelectedIndex];
            /* Close dropdown */
            cbBaudrate.IsDropDownOpen = false;
        }
    }

    private void CbParity_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbParity.SelectedItem != null && !string.IsNullOrEmpty(cbParity.SelectedItem.ToString()))
        {
            /* Set ledName */
            parity = parities[cbParity.SelectedIndex];
            /* Close dropdown */
            cbParity.IsDropDownOpen = false;
        }
    }

    private void CbHandshake_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbHandshake.SelectedItem != null && !string.IsNullOrEmpty(cbHandshake.SelectedItem.ToString()))
        {
            /* Set ledName */
            handshake = handshakes[cbHandshake.SelectedIndex];
            /* Close dropdown */
            cbHandshake.IsDropDownOpen = false;
        }
    }

    private void GetValuesFromTextBox()
    {
        if (!string.IsNullOrEmpty(tbMessage.Text))
            valueWrite = tbMessage.Text;
        else
            tbMessage.Text = valueWrite;
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnGetSerialPorts.AddHandler(Button.ClickEvent, BtnGetSerialPorts_Clicked!);
        btnUartRW.AddHandler(Button.ClickEvent, BtnUartRW_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes*/
        tbMessage.Text = valueWrite;
    }

    private void FillTextBlockWithText()
    {
        /* Description Text */
        txDescSerialPorts.Text = "Get all available Serial Ports.";
        txInfoSerialPorts.Text = "";
        txDescUart.Text = "Select Sender and Receiver Ports first";
        txUartSend.Text = "";
        txUartReceive.Text = "";
        txInfoUart.Text = "";
    }

    private void SetupComboBox()
    {
        /* Sender & Receiver */
        cbUartSender.AddHandler(ComboBox.SelectionChangedEvent, CbUartSender_SelectionChanged!);
        cbUartReceiver.AddHandler(ComboBox.SelectionChangedEvent, CbUartReceiver_SelectionChanged!);
        /* Data Bit */
        cbDataBit.ItemsSource = dataBits;
        cbDataBit.SelectedItem = dataBit;
        cbDataBit.AddHandler(ComboBox.SelectionChangedEvent, CbDataBit_SelectionChanged!);
        /* Stop Bit */
        cbStopBit.ItemsSource = stopBits;
        cbStopBit.SelectedItem = stopBit;
        cbStopBit.AddHandler(ComboBox.SelectionChangedEvent, CbStopBit_SelectionChanged!);
        /* Baudrate */
        cbBaudrate.ItemsSource = baudrates;
        cbBaudrate.SelectedItem = baudrate;
        cbBaudrate.AddHandler(ComboBox.SelectionChangedEvent, CbBaudrate_SelectionChanged!);
        /* Parity */
        cbParity.ItemsSource = parities;
        cbParity.SelectedItem = parity;
        cbParity.AddHandler(ComboBox.SelectionChangedEvent, CbParity_SelectionChanged!);
        /* Handshake */
        cbHandshake.ItemsSource = handshakes;
        cbHandshake.SelectedItem = handshake;
        cbHandshake.AddHandler(ComboBox.SelectionChangedEvent, CbHandshake_SelectionChanged!);
    }
}
