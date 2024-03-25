using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Threading;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_Can : UserControl
{
    /* CAN functions are in separate class */
    private readonly Can_Tests Can;
    private bool canThreadStarted = false;


    public UserControl_Can()
    {
        InitializeComponent();

        /* button bindings */
        btnCan.AddHandler(Button.ClickEvent, btnCan_Clicked!);
        tbDesc.Text = "Connect CAN Interface\r\n" +
            "Run following commands in Linux to activate CAN:\r\n" +
            "ip link set can0 up type can bitrate 1000000\r\n" +
            "ifconfig can0 up"; //TODO

        Can = new Can_Tests();
    }

    void btnCan_Clicked(object sender, RoutedEventArgs args)
    {
        if (!canThreadStarted)
        {
            /* Create new Thread, start CAN Test */
            Thread CanLedThread = new Thread(new ThreadStart(Can.CanStart));
            CanLedThread.Start();
            canThreadStarted = true;
            /* Change UI */
            btnCan.Content = "Stop CAN";
            btnCan.Background = Brushes.Red;
            tbCanInfo.Text = "CAN Test running";
        }
        else
        {
            /* Stop the CAN Thread from running */
            Can!.CanStop();
            canThreadStarted = false;
            /* Change UI */
            btnCan.Content = "Start CAN";
            btnCan.Background = Brushes.LightGreen;
            tbCanInfo.Text = "CAN Test stopped";
        }
    }
}