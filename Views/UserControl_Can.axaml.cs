using System;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_Can : UserControl
{
    /* CAN functions are in separate class */
    private readonly Can_Tests Can;
    /* Standard values */
    private string canDevice = "0"; //can0
    private string bitrate = "1000000";

    public UserControl_Can()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();

        /* Create new Can_Tests */
        Can = new Can_Tests();
    }

    void btnCan_Clicked(object sender, RoutedEventArgs args)
    {
        /* Convert GPIO Pin # to gpio bank and pin */
        if(tbCanDev.Text != "" && tbCanDev.Text != null)
            canDevice = tbCanDev.Text;
        if (tbBitrate.Text != "" && tbBitrate.Text != null)
            bitrate = tbBitrate.Text;

        bool canRWTestSuccess;
        try
        {
            /* Run Read / Write test */
            canRWTestSuccess = Can.StartCanRWTest(canDevice, bitrate);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoCan.Text = ex.Message;
            txInfoCan.Foreground = Brushes.Red;
            return;
        }

        if (canRWTestSuccess)
        {
            txInfoCan.Text = "CAN Test Success: value send and value read are equal, CAN ID of returned message is different from sent message"; //TODO: Meldung verbessern
            txInfoCan.Foreground = Brushes.Green;
        }
        else
        {
            txInfoCan.Text = "CAN Test Failed\r\n" +
                "Is receiving device connected and CAN activated?";
            txInfoCan.Foreground = Brushes.Red;
        }
    }

    void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        /* Check if the pressed key is a control character (like Backspace) or a digit */
        if (!char.IsControl(Convert.ToChar(e.KeySymbol!)) && !char.IsDigit(Convert.ToChar(e.KeySymbol!)))
        {
            /* If it's not, prevent the character from being entered */
            e.Handled = true;
        }
    }

    void AddButtonHandlers()
    {
        /* Button bindings */
        btnCan.AddHandler(Button.ClickEvent, btnCan_Clicked!);
    }

    void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes*/
        tbCanDev.Text = Convert.ToString(canDevice);
        tbBitrate.Text = Convert.ToString(bitrate);
    }

    void AddTextBoxHandlers()
    {
        /* Handler to only allow number inputs */
        tbCanDev.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbBitrate.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
    }

    void FillTextBlockWithText()
    {
        /* Description Text */
        txDescCan.Text = "Connect second board, CAN_L - CAN_L & CAN_H - CAN_H\r\n" +
            "On second device , run following comand under Linux to activate can0:\r\n" +
            "ip link set can0 up type can bitrate 1000000 && ifconfig can0 up\r\n" +
            "Run this command while CAN test is running to return the received value:\r\n" +
            "STRING=$(candump can0 -L -n1 | cut -d '#' -f2) && cansend can0 01b#${STRING}";

        txInfoCan.Text = "";
    }
}
