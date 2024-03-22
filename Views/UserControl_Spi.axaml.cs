using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models;
using System;
using System.Threading;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace IoTLib_Test.Views;

public partial class UserControl_Spi : UserControl
{
    /* SPI functions are in separate class */
    private readonly Spi_Tests spi;
    public int spidev = 0x1;
    byte register = 0x2b;

    public UserControl_Spi()
    {
        InitializeComponent();

        /* button bindings */
        btnSpi.AddHandler(Button.ClickEvent, btnSpi_Clicked!);
        /* Description Text */
        tbDesc.Text = "Connect BBDSI with SPI: " +
            "SCLK: ADP-2 -> J11-3; " +
            "MOSI: ADP-3 -> J11-6; " +
            "MISO: ADP-4 -> J11-5; " +
            "CS: ADP-6 -> J11-4; " +
            "RESET: ADP-8 -> J11-39; " +
            "GND: ADP-16 -> J11-42; " +
            "+3V3: ADP-26 -> J11-1";

        spi = new Spi_Tests();
    }

    void btnSpi_Clicked(object sender, RoutedEventArgs args)
    {
        if(!spi.SpiStart(spidev, register))
        {
            tbSpiInfo.Text = "Data sent and data read are different";
            tbSpiInfo.Foreground = Brushes.Red;
        }
        else
        {
            tbSpiInfo.Text = "Data sent and data read are equal";
            tbSpiInfo.Foreground = Brushes.Green;
        }
    }
}