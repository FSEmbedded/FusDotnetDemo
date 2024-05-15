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
using System.Collections.Generic;
using System.IO.Ports;

namespace FusDotnetDemo.Models.Hardware;

internal class Uart_Demo
{
    private readonly SerialPort serialPort;
    private string valueRead = "";

    public Uart_Demo(string port, int baudrate, int dataBit, double stopBit, string parity, string handshake)
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
        List<string> ports = [.. SerialPort.GetPortNames()];

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

    #region Converter
    private static Parity ConvertParity(string parityString)
    {
        var parity = parityString switch
        {
            "None" => Parity.None,
            "Odd" => Parity.Odd,
            "Even" => Parity.Even,
            "Mark" => Parity.Mark,
            "Space" => Parity.Space,
            _ => Parity.None,
        };
        return parity;
    }

    private static StopBits ConvertStopBit(double stopBitDouble)
    {
        var stopBits = stopBitDouble switch
        {
            0 => StopBits.None,
            1 => StopBits.One,
            1.5 => StopBits.OnePointFive,
            2 => StopBits.Two,
            _ => StopBits.One,
        };
        return stopBits;
    }

    private static Handshake ConvertHandshake(string handshakeString)
    {
        var handshake = handshakeString switch
        {
            "None" => Handshake.None,
            "XOnXOff" => Handshake.XOnXOff,
            "RequestToSend" => Handshake.RequestToSend,
            "RequestToSendXOnXOff" => Handshake.RequestToSendXOnXOff,
            _ => Handshake.None,
        };
        return handshake;
    }
    #endregion
}
