/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Brügel                               *
*                                                       *
*    This file is part of FusDotnetDemo.                *
*                                                       *
*********************************************************/

using Avalonia.Controls;
using System;
using System.Net;
using FusDotnetDemo.Models.Tools;

namespace FusDotnetDemo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        /* Get default values for board type first
         * Default values only work for Linux */
        if (OperatingSystem.IsLinux())
        {
            DefaultValues.GetDefaultValues();
        }

        InitializeComponent();
        
        /* Get version */
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        string? version = fvi.FileVersion;

        /* Add Infos to Header */
        if (!string.IsNullOrEmpty(version) )
            txVersion.Text = $"FusDotnetDemo V{version}";

        txHeader.Text = ".NET on Linux with F&S Boards";
        string archName = Environment.MachineName;
        txArchName.Text = archName;
        txIp.Text = GetIPAddress(archName);
    }

    public static string GetIPAddress(string archName)
    {
        string ip = "";
        IPHostEntry? Host = Dns.GetHostEntry(archName);
        foreach (IPAddress IP in Host.AddressList)
        {
            if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ip = "IP: " + Convert.ToString(IP);
            }
        }
        return ip;
    }
}