using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Threading;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_I2c : UserControl
{
    /* I²C functions are in separate class */
    private readonly I2c_Tests I2c;
    private bool ledThreadStarted = false;

    //TODO: Eingabe in UI
    public int busId = 0x5;
    public int deviceAddrLed = 0x23;
    public int deviceAddrStorage = 0x23;
    public int deviceAddrPwm = 0x63;
    public int deviceAddrAdc = 0x63; //TODO
    public int value1 = 0xAA;
    public int value2 = 0x55;

    public UserControl_I2c()
    {
        InitializeComponent();

        /* button bindings */
        btnI2cWrite.AddHandler(Button.ClickEvent, BtnI2cWrite_Clicked!);
        btnI2cRead.AddHandler(Button.ClickEvent, BtnI2cRead_Clicked!);
        btnPwm.AddHandler(Button.ClickEvent, BtnPwm_Clicked!);
        /* Description Text */
        tbDesc.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16";
        tbPwmDesc.Text = "Connect I²C Extension Board Pins: J2-17 -> J2-27; Set S2-3 to ON";

        I2c = new I2c_Tests();
    }

    void BtnI2cWrite_Clicked(object sender, RoutedEventArgs args)
    {
        if (!ledThreadStarted)
        {
            /* Create new Thread, start I2C Test */
            Thread I2cLedThread = new(() => I2c.WriteLedValues(busId, deviceAddrLed));
            I2cLedThread.Start();
            ledThreadStarted = true;
            /* Change UI */
            btnI2cWrite.Content = "Stop I²C";
            btnI2cWrite.Background = Brushes.Red;
            tbLedInfo.Text = "LEDs on I²C-Extension Board are blinking";
        }
        else
        {
            /* Stop the I2C Thread from running */
            I2c!.StopLed();
            ledThreadStarted = false;
            /* Change UI */
            btnI2cWrite.Content = "Start I²C";
            btnI2cWrite.Background = Brushes.LightGreen;
            tbLedInfo.Text = "LEDs on I²C-Extension Board are blinking";
        }
    }

    void BtnI2cRead_Clicked(object sender, RoutedEventArgs args)
    {
        /* Send data to I2C Device */
        if(I2c.WriteData(busId, deviceAddrStorage, value1, value2))
        {
            tbI2cReadInfoSend.Text = $"Value {value1} sent to I²C Device";
            tbI2cReadInfoSend.Foreground = Brushes.Blue;

            /* Read data from I2C Device, true if values match the data sent */
            if (I2c.ReadData(busId, deviceAddrStorage))
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
        double returnVoltage;
        int counter = 0;
        bool toggleOn = true;

        tbPwmSend.Text = $"PWM will toggle. See LED";

        while(counter <= 9)
        {
            I2c.SetPwm(busId, deviceAddrPwm, toggleOn);
            //TODO: Read auswerten
            returnVoltage = I2c.ReadADC(busId, deviceAddrAdc);
            tbPwmRead.Text = $"ADC received {returnVoltage} V";
            Thread.Sleep(1000);

            toggleOn = !toggleOn;
            counter++;
        }

        //if( returnVoltage == voltage1)
        //{

        //}
    }
}

//TODO: Eingabefeld für busId und deviceAddr