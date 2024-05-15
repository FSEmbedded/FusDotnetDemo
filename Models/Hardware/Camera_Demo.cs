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
using Iot.Device.Media;


namespace FusDotnetDemo.Models.Hardware;

internal class Camera_Demo
{
    private readonly VideoConnectionSettings settings;
    private readonly VideoDevice videoDevice;

    public Camera_Demo(int _busid, uint _width, uint _height)
    {
        try
        {
            /* Create new object VideoDevice.
             * Set VideoPixelFormat to RGB24 (other settings may work, but must be tested) */
            settings = new(busId: _busid, captureSize: (_width, _height), VideoPixelFormat.RGB24);
            videoDevice = VideoDevice.Create(settings);
        }
        catch (Exception ex)
        {
            throw new("Exception: " + ex.Message);
        }
    }

    public bool CaptureCam(string imgFile)
    {
        /* Capture static image */
        videoDevice.Capture(imgFile);

        FileInfo imageSize = new(imgFile);


        if (imageSize.Length == 0)
            return false;

        return true;
    }

    public static List<string> GetAvailableCameras()
    {
        List<string> cameras = [];

        /* This command first filters the output of ls to only include lines starting with 'l' (indicating symbolic links),
         * then uses awk to print the last field of each line */
        string argument = $"-c \"ls -l /dev/v4l/by-id/ | grep '^l' | awk '{{print $NF}}'\"";

        ProcessStartInfo startInfo = new()
        {
            FileName = "/bin/bash",
            Arguments = argument,
            RedirectStandardOutput = true,
        };

        using Process process = Process.Start(startInfo)!;

        while (!process.StandardOutput.EndOfStream)
        {
            /* Add the values read to the list */
            string line = process.StandardOutput.ReadLine()!;
            if (!string.IsNullOrEmpty(line))
            {
                cameras.Add(line);
            }
        }
        return cameras;
    }
}
