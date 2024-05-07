/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Brügel                               *
*                                                       *
*    This file is part of dotnetIoT_Demo.               *
*                                                       *
*********************************************************/

using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Iot.Device.SocketCan;

namespace dotnetIot_Demo.Models.Hardware;

internal class Can_Demo
{
    private readonly string canDev;
    private readonly string bitrate;
    private readonly CanId canIdWrite;
    private CanId canIdRead;
    /* Value that is read will be stored in thise byte array */
    private byte[] valueRead = [];

    private bool rwTestIsRunning; // Is used to stop while-loop
    private readonly int maxReadCount = 10; // counter for repeating Read task

    public Can_Demo(string _candev, string _bitrate, uint _canIdWriteValue)
    {
        canDev = _candev; // eg. "can0"
        bitrate = _bitrate;
        /* Set standard identifier for canIdWrite */
        canIdWrite = new() { Standard = _canIdWriteValue };

        /* Check if CAN device is up */
        if (!IsCanDevUp())
        {
            /* Activate canDev */
            ActivateCanDev();
            /* Throw exception if canDev couldn't be activated */
            if (!IsCanDevUp())
            {
                throw new Exception($"Could not activate CAN device {canDev}");
            }
        }

        try
        {
            /* Start write processes to check if receiving device is working */
            CanWriteFrame([0]);
        }
        catch (Exception ex)
        {
            throw new Exception($"Receiving device is not working as expected: {ex.Message}");
        }
    }

    public (byte[], uint) StartRWTest(byte[] valueWrite)
    {
        /* Reset values */
        rwTestIsRunning = true;
        int readCount = 0;

        /* Start write thread. RepeatWriteFrame will call CanWriteFrame() in a loop */
        Thread writeThread = new(() => RepeatWriteFrame(valueWrite));
        writeThread.Start();

        while (rwTestIsRunning && readCount < maxReadCount)
        {
            /* Let CanRead run as Task, otherwise it won't timeout if it there is nothing to read */
            var readTask = Task.Run(() => CanReadFrame(valueWrite));

            if (readTask.Wait(TimeSpan.FromSeconds(1)))
            {
                /* CanRead returns true if anything was read */
                if (readTask.Result == true)
                {
                    /* Compare CAN IDs */
                    if (canIdRead.Value != canIdWrite.Value)
                    {
                        /* Stop RW Test if IDs are different -> echo from other device was received */
                        rwTestIsRunning = false;
                    }
                }
            }
            readCount++;
        }
        rwTestIsRunning = false;
        return (valueRead, canIdRead.Value);
    }

    #region RW_Test
    public void CanWriteFrame(byte[] valueWrite)
    {
        /* using declaration ensures hardware resources will be released properly */
        using CanRaw can = new(canDev!);
        Span<byte> bytes = new(valueWrite);
        
        try
        {
            /* Write value */
            can.WriteFrame(bytes, canIdWrite);
        }
        catch (Exception ex)
        {
            rwTestIsRunning = false;
            throw new Exception("CAN Write Exception: " + ex.Message);
        }
    }

    public bool CanReadFrame(byte[] valueWrite)
    {
        /* using declaration ensures hardware resources will be released properly */
        using CanRaw can = new(canDev!);
        /* buffer needs to be the same length as valueSend */
        byte[] buffer = new byte[valueWrite.Length];

        try
        {
            if (can.TryReadFrame(buffer, out int frameLength, out canIdRead))
            {
                Span<byte> bytesRead = new(buffer, 0, frameLength);
                valueRead = bytesRead.ToArray();
                return true;
            }
        }
        catch (Exception ex)
        {
            rwTestIsRunning = false;
            throw new Exception("CAN Read Exception: " + ex.Message);
        }
        return false;
    }

    public void RepeatWriteFrame(byte[] valueWrite)
    {
        /* Repeat until rwTestIsRunning == false is set by another function */
        while (rwTestIsRunning)
        {
            try
            {
                /* Write data */
                CanWriteFrame(valueWrite);
            }
            catch
            {
                rwTestIsRunning = false;
            }
            Thread.Sleep(1000);
        }
    }
    #endregion
    #region ActivateCanDev
    public bool IsCanDevUp()
    {
        /* Check if CAN device is up */
        /* Run shell command */
        string argument = $"-c \"ip link show {canDev} | grep -q 'state UP'\"";
        ProcessStartInfo startInfo = new()
        {
            FileName = "/bin/bash",
            Arguments = argument,
        };

        using Process process = Process.Start(startInfo)!;
        process.WaitForExit();
        /* ExitCode is 0 if canDev is up */
        if (process.ExitCode == 0)
        {
            return true;
        }
        return false;
    }

    public void ActivateCanDev()
    {
        /* Activate canDev and setup bitrate */
        string argument = $"-c \"ip link set {canDev} up type can bitrate {bitrate} && ifconfig {canDev} up\"";

        ProcessStartInfo startInfo = new()
        {
            FileName = "/bin/bash",
            Arguments = argument,
        };

        using Process process = Process.Start(startInfo)!;
        process.WaitForExit();
    }
    #endregion
}
