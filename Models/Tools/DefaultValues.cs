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

public class DefaultValues
{
    /* Path to JSON-file containing default values */
    private static readonly string boardvaluesPath = AppDomain.CurrentDomain.BaseDirectory + "boardvalues.json";

    #region Values
    public static string BoardType { get; private set; } = string.Empty;

    /* Audio Values */
    public static string AudioPlaybackDevice { get; private set; } = string.Empty; // board specific
    public static string AudioRecordingDevice { get; private set; } = string.Empty; // board specific
    public static string AudioFilePathRecordingContinuous { get; private set; } = "rec_continuous.wav"; // common default
    public static string AudioFilePathRecordingTimed { get; private set; } = "rec_fixedduration.wav"; // common default
    public static string AudioInputSignal { get; private set; } = "LINE_IN"; // common default
    public static uint AudioRecordingTime { get; private set; } = 5; // common default
    /* Camera Values */
    public static string CameraFilePathImage { get; private set; } = "img_camtest.png"; // common default
    public static uint CameraImageHeigth { get; private set; } = 1080; // common default
    public static uint CameraImageWidth { get; private set; } = 1920; // common default
    /* CAN Values */
    public static string CanDeviceNo { get; private set; } = string.Empty; // board specific
    public static uint CanIdWrite { get; private set; } = 0; // board specific
    public static string CanBitrate { get; private set; } = "1000000"; // common default
    public static byte[] CanValuesSend { get; private set; } = [1, 2, 3, 40, 50, 60, 70, 80]; // common default
    /* GPIO Values */
    public static int GpioNoInputButton { get; private set; } = 0; // board specific
    public static int GpioNoOutputButton { get; private set; } = 0; // board specific
    public static int GpioNoOutputLed { get; private set; } = 0; // board specific
    /* I2C Values */
    public static int I2cBusIdAdc { get; private set; } = 0; // board specific
    public static int I2cBusIdLed { get; private set; } = 0; // board specific
    public static int I2cBusIdPwm { get; private set; } = 0; // board specific
    public static int I2cBusIdRW { get; private set; } = 0; // board specific
    public static int I2cDeviceAddrAdc { get; private set; } = 0x4B; // common default
    public static int I2cDeviceAddrLed { get; private set; } = 0x23; // common default
    public static int I2cDeviceAddrPwm { get; private set; } = 0x63; // common default
    public static int I2cDeviceAddrRW { get; private set; } = 0x23; // common default
    public static byte I2cRegister1 { get; private set; } = 0x02; // common default
    public static byte I2cRegister2 { get; private set; } = 0x03; // common default
    public static byte I2cValueWrite1 { get; private set; } = 0xAA; // common default
    public static byte I2cValueWrite2 { get; private set; } = 0x55; // common default
    /* LED Values */
    public static string LedName { get; private set; } = string.Empty; // board specific
    /* PWM Values */
    public static int PwmGpioNo { get; private set; } = 0; // board specific
    public static int PwmDuration { get; private set; } = 10; // common default
    public static double PwmVoltageValue { get; private set; } = 0.5; // common default
    /* SPI Values */
    public static byte SpiRegister1 { get; private set; } = 0; // board specific
    public static byte SpiRegister2 { get; private set; } = 0; // board specific
    public static int SpiDevice { get; private set; } = 0; // board specific
    public static byte SpiValueWrite1 { get; private set; } = 0x5; // common default
    public static byte SpiValueWrite2 { get; private set; } = 0x6; // common default
    /* UART Values */
    public static string UartPortReceiver { get; private set; } = string.Empty; // board specific
    public static string UartPortSender { get; private set; } = string.Empty; // board specific
    public static int UartBaudrate { get; private set; } = 115200; // common default
    public static List<int> UartBaudrates { get; private set; } = new([110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 230400, 460800, 921600]); // common default
    public static int UartDataBit { get; private set; } = 8; // common default
    public static List<int> UartDataBits { get; private set; } = new([7, 8]); // common default
    public static string UartHandshake { get; private set; } = "None"; // common default
    public static List<string> UartHandshakes { get; private set; } = new(["None", "XOnXOff", "RequestToSend", "RequestToSendXOnXOff"]); // common default
    public static List<string> UartParities { get; private set; } = new(["None", "Odd", "Even", "Mark", "Space"]); // common default
    public static string UartParity { get; private set; } = "None"; // common default
    public static double UartStopBit { get; private set; } = 1; // common default
    public static List<double> UartStopBits { get; private set; } = new([0, 1, 1.5, 2]); // common default
    public static string UartValueWrite { get; private set; } = "TestMessage1234567890"; // common default
    #endregion

    #region Functions
    public static void GetDefaultValues()
    {
        /* Find BoardType first */
        GetBoardType();
        /* BoardType is needed to find the relevant info in json file */
        ReadValuesFromJson();
    }

    private static void GetBoardType()
    {
        /* Get info for platform / boardtype */
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
            /* Store platform as BoardType, eg. picocoremx8mpr2, picocoremx6sx */
            BoardType = line.ToLower();
        }
    }

    private static void ReadValuesFromJson()
    {
        /* If boardvalues.json is not available you can still enter your values in the UI */
        if (!File.Exists(boardvaluesPath))
            return;

        string jsonString = File.ReadAllText(boardvaluesPath);
        dynamic? jsonData = JsonConvert.DeserializeObject(jsonString);

        if (jsonData == null)
            return;

        /* Get board specific values */
        if (!string.IsNullOrEmpty(BoardType) || jsonData.boards[BoardType] != null)
        {
            /* Read values for the specific board type */
            var boardData = jsonData.boards[BoardType].interfaces;
            /* Audio Values */
            AudioPlaybackDevice = boardData.audio.PlaybackDevice ?? AudioPlaybackDevice;
            AudioRecordingDevice = boardData.audio.RecordingDevice ?? AudioRecordingDevice;
            /* CAN Values */
            CanDeviceNo = boardData.can.CanDeviceNo ?? CanDeviceNo;
            CanIdWrite = ConvertHexStringToUInt32((string)boardData.can.CanIdWrite);
            /* GPIO Values */
            GpioNoOutputLed = boardData.gpio.GpioNoOutputLed ?? GpioNoOutputLed;
            GpioNoOutputButton = boardData.gpio.GpioNoOutputButton ?? GpioNoOutputButton;
            GpioNoInputButton = boardData.gpio.GpioNoInputButton ?? GpioNoInputButton;
            /* I2C Values */
            I2cBusIdRW = ConvertHexStringToInt((string)boardData.i2c.I2cBusIdRW);
            I2cBusIdLed = ConvertHexStringToInt((string)boardData.i2c.I2cBusIdLed);
            I2cBusIdPwm = ConvertHexStringToInt((string)boardData.i2c.I2cBusIdPwm);
            I2cBusIdAdc = ConvertHexStringToInt((string)boardData.i2c.BusId);
            /* LED Values */
            LedName = boardData.led.LedName ?? LedName;
            /* PWM Values */
            PwmGpioNo = boardData.pwm.GpioNoPwm ?? PwmGpioNo;
            /* SPI Values */
            SpiDevice = ConvertHexStringToInt((string)boardData.spi.SpiDevice);
            SpiRegister1 = ConvertHexStringToByte((string)boardData.spi.Register1);
            SpiRegister2 = ConvertHexStringToByte((string)boardData.spi.Register2);
            /* UART Values */
            UartPortSender = boardData.uart.PortSender ?? UartPortSender;
            UartPortReceiver = boardData.uart.PortReceiver ?? UartPortReceiver;
        }
        
        /* Check if json contains "CommonDefaults" */
        if(jsonData.CommonDefaults != null)
        {
            /* Read values from "CommonDefaults" */
            var commonDefaults = jsonData.CommonDefaults.interfaces;
            /* Audio Values */
            AudioRecordingTime = commonDefaults.audio.RecordingTime ?? AudioRecordingTime;
            AudioFilePathRecordingContinuous = commonDefaults.audio.FilePathRecordingContinuous ?? AudioFilePathRecordingContinuous;
            AudioFilePathRecordingTimed = commonDefaults.audio.FilePathRecordingTimed ?? AudioFilePathRecordingTimed;
            AudioInputSignal = commonDefaults.audio.InputSignal ?? AudioInputSignal;
            /* Camera Values */
            CameraFilePathImage = commonDefaults.camera.FilePathImage ?? CameraFilePathImage;
            CameraImageWidth = commonDefaults.camera.ImageWidth ?? CameraImageWidth;
            CameraImageHeigth = commonDefaults.camera.ImageHeigth ?? CameraImageHeigth;
            /* CAN Values */
            CanBitrate = commonDefaults.can.Bitrate ?? CanBitrate;
            CanValuesSend = ((JArray)commonDefaults.can.ValuesSend).ToObject<byte[]>() ?? CanValuesSend;
            /* I2C Values */
            I2cDeviceAddrRW = ConvertHexStringToInt((string)commonDefaults.i2c.DeviceAddrRW);
            I2cDeviceAddrLed = ConvertHexStringToInt((string)commonDefaults.i2c.DeviceAddrLed);
            I2cDeviceAddrPwm = ConvertHexStringToInt((string)commonDefaults.i2c.DeviceAddrPwm);
            I2cDeviceAddrAdc = ConvertHexStringToInt((string)commonDefaults.i2c.DeviceAddrAdc);
            I2cValueWrite1 = ConvertHexStringToByte((string)commonDefaults.i2c.ValueWrite1);
            I2cValueWrite2 = ConvertHexStringToByte((string)commonDefaults.i2c.ValueWrite2);
            I2cRegister1 = ConvertHexStringToByte((string)commonDefaults.i2c.Register1);
            I2cRegister2 = ConvertHexStringToByte((string)commonDefaults.i2c.Register2);
            /* PWM Values */
            PwmDuration = commonDefaults.pwm.Duration ?? PwmDuration;
            PwmVoltageValue = commonDefaults.pwm.VoltageValue ?? PwmVoltageValue;
            /* SPI Values */
            SpiValueWrite1 = ConvertHexStringToByte((string)commonDefaults.spi.ValueWrite1);
            SpiValueWrite2 = ConvertHexStringToByte((string)commonDefaults.spi.ValueWrite2);
            /* UART Values */
            UartValueWrite = commonDefaults.uart.ValueWrite ?? UartValueWrite;
            UartBaudrate = commonDefaults.uart.Baudrate ?? UartBaudrate;
            UartDataBit = commonDefaults.uart.DataBit ?? UartDataBit;
            UartStopBit = commonDefaults.uart.StopBit ?? UartStopBit;
            UartParity = commonDefaults.uart.Parity ?? UartParity;
            UartHandshake = commonDefaults.uart.Handshake ?? UartHandshake;
            UartBaudrates = ((JArray)commonDefaults.uart.Baudrates).ToObject<List<int>>() ?? UartBaudrates;
            UartDataBits = ((JArray)commonDefaults.uart.DataBits).ToObject<List<int>>() ?? UartDataBits;
            UartStopBits = ((JArray)commonDefaults.uart.StopBits).ToObject<List<double>>() ?? UartStopBits;
            UartParities = ((JArray)commonDefaults.uart.Parities).ToObject<List<string>>() ?? UartParities;
            UartHandshakes = ((JArray)commonDefaults.uart.Handshakes).ToObject<List<string>>() ?? UartHandshakes;
        }
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
