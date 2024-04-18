using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;

namespace IoTLib_Test.Models.Hardware_Tests;

internal class Uart_Tests
{
    private readonly SerialPort serialPort;
    private string valueRead = "";

    public Uart_Tests(string port, int baudrate, int dataBit, double stopBit, string parity, string handshake)
    {
        try
        {
            /* Create a new SerialPort object */
            serialPort = new SerialPort()
            {
                /* Change settings to selected values from UI */
                PortName = port,
                BaudRate = baudrate,
                DataBits = dataBit,
                StopBits = ConvertStopBit(stopBit),
                Parity = ConvertParity(parity),
                Handshake = ConvertHandshake(handshake),
            };
        }
        catch (Exception ex)
        {
            throw new("Exception: " + ex.Message);
        }
    }

    public static List<string> GetAvailableSerialPorts()
    {
        /* Collect Port values in a list. */
        List<string> ports = SerialPort.GetPortNames().ToList();

        return ports;
    }

    public string Read()
    {
        /* Set the read timeout */
        serialPort.ReadTimeout = 500;
        /* Open port for reading */
        serialPort.Open();
        /* Read message on selected port */
        try
        {
            valueRead = serialPort.ReadLine();
        }
        catch (TimeoutException) { }

        /* Close port */
        serialPort.Close();

        return valueRead;
    }

    public void Write(string message)
    {
        /* Set the write timeout */
        serialPort.WriteTimeout = 500;
        /* Open port for writing */
        serialPort.Open();
        /* Write message to selected port */
        try
        {
            serialPort.WriteLine(message);
        }
        catch (TimeoutException) { }

        /* Close port */
        serialPort.Close();
    }

    private static Parity ConvertParity(string parityString)
    {
        Parity parity;

        switch (parityString)
        {
            case "None":
                parity = Parity.None;
                break;
            case "Odd":
                parity = Parity.Odd;
                break;
            case "Even":
                parity = Parity.Even;
                break;
            case "Mark":
                parity = Parity.Mark;
                break;
            case "Space":
                parity = Parity.Space;
                break;
            default:
                parity = Parity.None;
                break;
        }

        return parity;
    }

    private static StopBits ConvertStopBit(double stopBitDouble)
    {
        StopBits stopBits;

        switch (stopBitDouble)
        {
            case 0:
                stopBits = StopBits.None;
                break;
            case 1:
                stopBits = StopBits.One;
                break;
            case 1.5:
                stopBits = StopBits.OnePointFive;
                break;
            case 2:
                stopBits = StopBits.Two;
                break;
            default:
                stopBits = StopBits.One;
                break;
        }

        return stopBits;
    }

    private static Handshake ConvertHandshake(string handshakeString)
    {
        Handshake handshake;

        switch (handshakeString)
        {
            case "None":
                handshake = Handshake.None;
                break;
            case "XOnXOff":
                handshake = Handshake.XOnXOff;
                break;
            case "RequestToSend":
                handshake = Handshake.RequestToSend;
                break;
            case "RequestToSendXOnXOff":
                handshake = Handshake.RequestToSendXOnXOff;
                break;
            default:
                handshake = Handshake.None;
                break;
        }

        return handshake;
    }
}
