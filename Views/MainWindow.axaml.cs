﻿/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Brügel                               *
*                                                       *
*    This file is part of dotnetIoT_Demo.               *
*                                                       *
*********************************************************/

using Avalonia.Controls;
using System;
using System.Net;

namespace dotnetIot_Demo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        /* Add Infos to Header */
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