using Avalonia.Controls;
using System;
using System.Net;

namespace IoTLib_Test.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /* Add IP Address to Header */
            txIp.Text = GetIPAddress();
        }

        public static string GetIPAddress()
        {
            string ip = "";
            string Hostname = Environment.MachineName;
            IPHostEntry? Host = Dns.GetHostEntry(Hostname);
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
}