using Avalonia.Controls;
using System;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace IoTLib_Test.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /* Add IP Address to Header */

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
}