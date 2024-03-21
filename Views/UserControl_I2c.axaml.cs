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
    public int deviceAddrLed = 0x23;
    public int deviceAddrStorage = 0x23;
    public int deviceAddrPwm = 0x63; //TODO
    public int value1 = 0xAA; //TODO: Eingabe in UI
    public int value2 = 0x55; //TODO: Eingabe in UI
    private bool I2cLedThreadStarted = false;

    public UserControl_I2c()
    {
        InitializeComponent();

        /* button bindings */
        btnI2cWrite.AddHandler(Button.ClickEvent, BtnI2c_Clicked!);
        btnI2cRead.AddHandler(Button.ClickEvent, BtnI2cRead_Clicked!);
        btnPwm.AddHandler(Button.ClickEvent, BtnPwm_Clicked!);
        /* Description Text */
        tbDesc.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16";
        tbPwmDesc.Text = "Connect I²C Extension Board Pins: J2-17 -> J2-27; Set S2-3 to ON";

        i2c = new I2c_Tests();
    }

    void BtnI2c_Clicked(object sender, RoutedEventArgs args)
    {
        if (!I2cLedThreadStarted)
        {
            /* Create new Thread, start I2C Test */
            Thread I2cLedThread = new(() => i2c.WriteLedValues(busId, deviceAddrLed));
            I2cLedThread.Start();
            I2cLedThreadStarted = true;
            /* Change button */
            btnI2cWrite.Content = "Stop I²C";
            btnI2cWrite.Background = Brushes.Red;
            tbLedInfo.Text = "LEDs on I²C-Extension Board are blinking";
        }
        else
        {
            /* Stop the I2C Thread from running */
            i2c!.StopLed();
            I2cLedThreadStarted = false;
            /* Change button */
            btnI2cWrite.Content = "Start I²C";
            btnI2cWrite.Background = Brushes.LightGreen;
            tbLedInfo.Text = "LEDs on I²C-Extension Board are blinking";
        }
    }

    void BtnI2cRead_Clicked(object sender, RoutedEventArgs args)
    {
        if(i2c.WriteData(busId, deviceAddrStorage, value1, value2))
        {
            tbI2cReadInfoSend.Text = $"Value {value1} sent to I²C Device";
            tbI2cReadInfoSend.Foreground = Brushes.Blue;


            if (i2c.ReadData(busId, deviceAddrStorage))
            {
                tbI2cReadInfoRead.Text = $"Value {value1} read from I²C Device";
                tbI2cReadInfoRead.Foreground = Brushes.Green;
            }
            else
            {
                tbI2cReadInfoRead.Text = $"Value {value1} doesn't match the value read from I²C Device";
                tbI2cReadInfoRead.Foreground = Brushes.Red;
            }
        }
    }
    void BtnPwm_Clicked(object sender, RoutedEventArgs args)
    {
        if(i2c.SetPwm(busId, deviceAddrPwm))
        {
            tbPwmSend.Text = $"PWM will toggle. See LED";

            Thread pwmThread = new(new ThreadStart(i2c.TogglePWM));
            pwmThread.Start();

            //TODO: Read auswerten
            double returnVoltage = i2c.ReadADC();

            tbPwmRead.Text = $"ADC received {returnVoltage} V";
            
            //if( returnVoltage == voltage1)
            //{

            //}
        }
    }
}

//TODO: Read-Write auf I2C
//TODO: Eingabefeld für busId und deviceAddr
