using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models;
using System.Threading;

namespace IoTLib_Test.Views;

public partial class UserControl_I2c : UserControl
{
    /* I²C functions are in separate class */
    private readonly I2c_Tests i2c;

    public int busId = 0x5;
    public int deviceAddr = 0x23;
    private bool I2cThreadStarted = false;

    public UserControl_I2c()
    {
        InitializeComponent();

        /* button bindings */
        btnI2c.AddHandler(Button.ClickEvent, BtnI2c_Clicked!);

        i2c = new I2c_Tests();
    }

    void BtnI2c_Clicked(object sender, RoutedEventArgs args)
    {
        if (!I2cThreadStarted)
        {
            /* Create new Thread, start I2C Test */
            Thread I2cThread = new(() => i2c.StartI2cWrite(busId, deviceAddr));
            I2cThread.Start();
            I2cThreadStarted = true;
            /* Change button */
            btnI2c.Content = "Stop I²C";
            btnI2c.Background = Brushes.Red;
            tbInfo.Text = "LEDs on I²C-Extension Board are blinking";
        }
        else
        {
            /* Stop the I2C Thread from running */
            i2c!.StopI2cWrite();
            I2cThreadStarted = false;
            /* Change button */
            btnI2c.Content = "Start I²C";
            btnI2c.Background = Brushes.LightGreen;
            tbInfo.Text = "LEDs on I²C-Extension Board are blinking";
        }
    }
}

//TODO: Read-Write auf I2C
//TODO: Eingabefeld für busId und deviceAddr

#region I2C

// I2C_A_SCL - J11-16 -> GPIO3_IO28 - 92
// I2C_A_SDA - J11-17 -> GPIO3_IO29 - 93
// GND - J11-37
// Adresse LED: 0x23 // 0x46 - new NI2CFile.NI2C_MSG_HEADER(0x46, 0, 3)
// I2C-Extension-Board: SCL - J1-11; SDA - J1-10; GND - J1-16
#endregion
