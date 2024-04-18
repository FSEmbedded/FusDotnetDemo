using System;
using System.Threading;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models.Tools;
using IoTLib_Test.Models.Hardware_Tests;

namespace IoTLib_Test.Views;

public partial class UserControl_Uart : UserControl
{
    /* UART functions are in a separate class */
    Uart_Tests? UartSender;
    Uart_Tests? UartReceiver;

    /* Standard values */
    private string portSender = "/dev/ttymxc1";
    private string portReceiver = "/dev/ttymxc2";
    private string valueWrite = "TestMessage1234567890";
    private int baudrate = 115200;
    private int dataBit = 8;
    private double stopBit = 1;
    private string parity = "None";
    private string handshake = "None";

    private bool senderSet = false;
    private bool receiverSet = false;
    private string valueRead = "";
    
    /* Values to fill in ComboBoxes */
    private readonly List<int> baudrates = new([110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 230400, 460800, 921600]);
    private readonly List<int> dataBits = new([7, 8]);
    private readonly List<double> stopBits = new([0, 1, 1.5, 2]);
    private readonly List<string> parities = new(["None", "Odd", "Even", "Mark", "Space"]);
    private readonly List<string> handshakes = new(["None", "XOnXOff", "RequestToSend", "RequestToSendXOnXOff"]);

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
        /* Empty ComboBox */
        cbUartSender.Items.Clear();
        cbUartReceiver.Items.Clear();

        /* Find all available Serial Ports */
        List<string> ports = Uart_Tests.GetAvailableSerialPorts();

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
            UartSender = new Uart_Tests(portSender, baudrate, dataBit, stopBit, parity, handshake);
            UartReceiver = new Uart_Tests(portReceiver, baudrate, dataBit, stopBit, parity, handshake);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoUart.Text = ex.Message;
            txInfoUart.Foreground = Brushes.Red;
            txInfoUart.Text = "";
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
        txUartSend.Text = $"Message sent: {valueWrite}";
        txUartReceive.Text = $"Message read: {valueRead}";

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

    public void GetValuesFromTextBox()
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
