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
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FusDotnetDemo.Models.Tools;

public class DefaultBoardValues
{
    /* Path to JSON-file containing default values */
    private static readonly string boardvaluesPath = AppDomain.CurrentDomain.BaseDirectory + "boardvalues.json";

    #region Values
    public static string BoardType { get; private set; } = string.Empty;
    /* Audio Values */
    public static string LinuxPlaybackDevice { get; private set; } = string.Empty;
    public static string LinuxRecordingDevice { get; private set; } = string.Empty;
    public static uint RecDuration { get; private set; } = 5;
    public static string RecFileCont { get; private set; } = string.Empty;
    public static string RecFileDur { get; private set; } = string.Empty;
    public static string InputSignal { get; private set; } = "LINE_IN";
    /* Camera Values */
    public static string ImgFile { get; private set; } = string.Empty;
    public static uint Width { get; private set; } = 1920;
    public static uint Height { get; private set; } = 1080;
    /* CAN Values */
    public static string CanDevNo { get; private set; } = string.Empty;
    public static uint CanIdWrite { get; private set; } = 0;
    public static string Bitrate { get; private set; } = "1000000";
    public static byte[] ValueSend { get; private set; } = [1, 2, 3, 40, 50, 60, 70, 80];
    /* GPIO Values */
    public static int GpioNoLed { get; private set; } = 0;
    //TODO: umbenennen
    public static int GpioNoInputLed { get; private set; } = 0;
    public static int GpioNoInput { get; private set; } = 0;
    /* I2C Values */
    public static int BusId { get; private set; } = 0;
    public static int DevAddrRW { get; private set; } = 0;
    public static int DevAddrLed { get; private set; } = 0;
    public static int DevAddrPwm { get; private set; } = 0;
    public static int DevAddrAdc { get; private set; } = 0;
    public static byte ValueWrite1 { get; private set; } = 0xAA;
    public static byte ValueWrite2 { get; private set; } = 0x55;
    public static byte I2cRegister1 { get; private set; } = 0;
    public static byte I2cRegister2 { get; private set; } = 0;
    /* LED Values */
    public static string LedName { get; private set; } = string.Empty;
    /* PWM Values */
    public static int GpioNoPwm { get; private set; } = 0;
    public static int PwmDuration { get; private set; } = 0;
    public static double PwmVoltageValue { get; private set; } = 0;
    /* SPI Values */
    public static int SpiDev { get; private set; } = 0;
    public static byte SpiRegister1 { get; private set; } = 0;
    public static byte SpiRegister2 { get; private set; } = 0;
    public static byte SpiValueWrite1 { get; private set; } = 0;
    public static byte SpiValueWrite2 { get; private set; } = 0;
    /* UART Values */
    public static string UartPortSender { get; private set; } = string.Empty;
    public static string UartPortReceiver { get; private set; } = string.Empty;
    public static string TestMessage { get; private set; } = "TestMessage1234567890";
    public static int Baudrate { get; private set; } = 115200;
    public static int DataBit { get; private set; } = 8;
    public static double StopBit { get; private set; } = 1;
    public static string Parity { get; private set; } = "None";
    public static string Handshake { get; private set; } = "None";
    public static List<int> Baudrates { get; private set; } = new([110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 230400, 460800, 921600]);
    public static List<int> DataBits { get; private set; } = new([7, 8]);
    public static List<double> StopBits { get; private set; } = new([0, 1, 1.5, 2]);
    public static List<string> Parities { get; private set; } = new(["None", "Odd", "Even", "Mark", "Space"]);
    public static List<string> Handshakes { get; private set; } = new(["None", "XOnXOff", "RequestToSend", "RequestToSendXOnXOff"]);
    #endregion

    #region Functions
    public static void GetDefaultValues()
    {
        GetBoardType();

        ReadValuesFromJson();
    }

    private static void GetBoardType()
    {
        string argument = $"-c \"cat /sys/bdinfo/platform\"";

        ProcessStartInfo startInfo = new()
        {
            FileName = "/bin/bash",
            Arguments = argument,
            RedirectStandardOutput = true,
        };

        using Process process = Process.Start(startInfo)!;
        string line = process.StandardOutput.ReadLine()!;
        if (!string.IsNullOrEmpty(line))
        {
            BoardType = line;
        }
    }

    private static void ReadValuesFromJson()
    {
        if (!File.Exists(boardvaluesPath))
        {
            throw new FileNotFoundException("The JSON file with board values was not found.");
        }

        string jsonContent = File.ReadAllText(boardvaluesPath);
        dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);

        // Determine the board type dynamically or use a default board type if needed
        string boardType = BoardType.ToLower();
        if (string.IsNullOrEmpty(boardType) || jsonData.boards[boardType] == null)
        {
            throw new Exception("Board type is not specified or not found in the JSON.");
        }

        // Read values for the specific board type
        var boardData = jsonData.boards[boardType].interfaces;

        LinuxPlaybackDevice = boardData.audio.LinuxPlaybackDevice ?? LinuxPlaybackDevice;
        LinuxRecordingDevice = boardData.audio.LinuxRecordingDevice ?? LinuxRecordingDevice;

        CanDevNo = boardData.can.CanDevNo ?? CanDevNo;
        CanIdWrite = ConvertHexStringToUInt32((string)boardData.can.CanIdWrite);

        GpioNoLed = boardData.gpio.GpioNoLed ?? GpioNoLed;
        GpioNoInputLed = boardData.gpio.GpioNoInputLed ?? GpioNoInputLed;
        GpioNoInput = boardData.gpio.GpioNoInput ?? GpioNoInput;

        BusId = ConvertHexStringToInt((string)boardData.i2c.BusId);

        LedName = boardData.led.LedName ?? LedName;

        GpioNoPwm = boardData.pwm.GpioNoPwm ?? GpioNoPwm;

        SpiDev = ConvertHexStringToInt((string)boardData.spi.SpiDev);
        SpiRegister1 = ConvertHexStringToByte((string)boardData.spi.Register1);
        SpiRegister2 = ConvertHexStringToByte((string)boardData.spi.Register2);

        UartPortSender = boardData.uart.UartPortSender ?? UartPortSender;
        UartPortReceiver = boardData.uart.UartPortReceiver ?? UartPortReceiver;

        // Read values from "CommonDefaults"
        var commonDefaults = jsonData.CommonDefaults.interfaces;

        RecDuration = commonDefaults.audio.RecDuration ?? RecDuration;
        RecFileCont = commonDefaults.audio.RecFileCont ?? RecFileCont;
        RecFileDur = commonDefaults.audio.RecFileDur ?? RecFileDur;
        InputSignal = commonDefaults.audio.InputSignal ?? InputSignal;

        ImgFile = commonDefaults.camera.ImgFile ?? ImgFile;
        Width = commonDefaults.camera.Width ?? Width;
        Height = commonDefaults.camera.Height ?? Height;

        Bitrate = commonDefaults.can.Bitrate ?? Bitrate;
        ValueSend = ((JArray)commonDefaults.can.ValueSend).ToObject<byte[]>() ?? ValueSend;

        DevAddrRW = ConvertHexStringToInt((string)commonDefaults.i2c.DevAddrRW);
        DevAddrLed = ConvertHexStringToInt((string)commonDefaults.i2c.DevAddrLed);
        DevAddrPwm = ConvertHexStringToInt((string)commonDefaults.i2c.DevAddrPwm);
        DevAddrAdc = ConvertHexStringToInt((string)commonDefaults.i2c.DevAddrAdc);
        ValueWrite1 = ConvertHexStringToByte((string)commonDefaults.i2c.ValueWrite1);
        ValueWrite2 = ConvertHexStringToByte((string)commonDefaults.i2c.ValueWrite2);
        I2cRegister1 = ConvertHexStringToByte((string)commonDefaults.i2c.Register1);
        I2cRegister2 = ConvertHexStringToByte((string)commonDefaults.i2c.Register2);

        PwmDuration = commonDefaults.pwm.Duration ?? PwmDuration;
        PwmVoltageValue = commonDefaults.pwm.VoltageValue ?? PwmVoltageValue;

        SpiValueWrite1 = ConvertHexStringToByte((string)commonDefaults.spi.ValueWrite1);
        SpiValueWrite2 = ConvertHexStringToByte((string)commonDefaults.spi.ValueWrite2);

        TestMessage = commonDefaults.uart.TestMessage ?? TestMessage;
        Baudrate = commonDefaults.uart.Baudrate ?? Baudrate;
        DataBit = commonDefaults.uart.DataBit ?? DataBit;
        StopBit = commonDefaults.uart.StopBit ?? StopBit;
        Parity = commonDefaults.uart.Parity ?? Parity;
        Handshake = commonDefaults.uart.Handshake ?? Handshake;

        Baudrates = ((JArray)commonDefaults.uart.Baudrates).ToObject<List<int>>() ?? Baudrates;
        DataBits = ((JArray)commonDefaults.uart.DataBits).ToObject<List<int>>() ?? DataBits;
        StopBits = ((JArray)commonDefaults.uart.StopBits).ToObject<List<double>>() ?? StopBits;
        Parities = ((JArray)commonDefaults.uart.Parities).ToObject<List<string>>() ?? Parities;
        Handshakes = ((JArray)commonDefaults.uart.Handshakes).ToObject<List<string>>() ?? Handshakes;
    }

    private static uint ConvertHexStringToUInt32(string hexString)
    {
        if (string.IsNullOrWhiteSpace(hexString)) return 0;
        if (hexString.StartsWith("0x")) hexString = hexString.Substring(2);
        return uint.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
    }

    private static int ConvertHexStringToInt(string hexString)
    {
        if (string.IsNullOrWhiteSpace(hexString)) return 0;
        if (hexString.StartsWith("0x")) hexString = hexString.Substring(2);
        return int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
    }

    private static byte ConvertHexStringToByte(string hexString)
    {
        if (string.IsNullOrWhiteSpace(hexString)) return 0;
        if (hexString.StartsWith("0x")) hexString = hexString.Substring(2);
        return byte.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
    }
    #endregion
}

//TODO: ReadValuesFromJson -> optimieren
//TODO: Kommentare einfügen
//TODO: private string -> string.Empty ändern bei Common Values