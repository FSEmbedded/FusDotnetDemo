using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Threading;
using IoTLib_Test.Models;
using System;

namespace IoTLib_Test.Views;

public partial class UserControl_I2c : UserControl
{
    /* I²C functions are in separate class */
    private readonly I2c_Tests I2c;
    private bool ledThreadStarted = false;
    /* Standard IDs and addresses */
    private int busIdLed = 0x5;
    private int devAddrLed = 0x23;
    private int busIdStorage = 0x5;
    private int devAddrStorage = 0x23;
    private int busIdPwm = 0x5;
    private int devAddrPwm = 0x63;
    private int busIdAdc = 0x5;
    private int devAddrAdc = 0x63; //TODO
    /* Values to write on I2C storage */
    private int valueWrite1 = 0xAA;
    private int valueWrite2 = 0x55;

    public UserControl_I2c()
    {
        InitializeComponent();

        /* button bindings */
        btnI2cWrite.AddHandler(Button.ClickEvent, BtnI2cWrite_Clicked!);
        btnI2cRead.AddHandler(Button.ClickEvent, BtnI2cRead_Clicked!);
        btnPwm.AddHandler(Button.ClickEvent, BtnPwm_Clicked!);

        /* Write standard values in textboxes*/
        //TODO: Werte als hex anzeigen!
        tbBusIdLed.Text = Convert.ToString(busIdLed);
        tbDevAddrLed.Text = Convert.ToString(devAddrLed);
        tbBusIdStorage.Text = Convert.ToString(busIdStorage);
        tbDevAddrStorage.Text = Convert.ToString(devAddrStorage);
        tbValue1.Text = Convert.ToString(valueWrite1);
        tbValue2.Text = Convert.ToString(valueWrite2);
        tbBusIdPwm.Text = Convert.ToString(busIdPwm);
        tbDevAddrPwm.Text = Convert.ToString(devAddrPwm);
        tbBusIdAdc.Text = Convert.ToString(busIdAdc);
        tbDevAddrAdc.Text = Convert.ToString(devAddrAdc);

        /* Description Text */
        txDescLed.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16";
        txDescStorage.Text = ""; //TODO
        txDescPwm.Text = "Connect I²C Extension Board Pins: J2-17 -> J2-27; Set S2-3 to ON";

        txInfoLed.Text = "";
        txInfoWrite.Text = "";
        txInfoRead.Text = "";
        txPwmSend.Text = "";
        txPwmRead.Text = "";

        I2c = new I2c_Tests();
    }

    void BtnI2cWrite_Clicked(object sender, RoutedEventArgs args)
    {
        if (!ledThreadStarted)
        {
            /* Create new Thread, start I2C Test */
            Thread I2cLedThread = new(() => I2c.WriteLedValues(busIdLed, devAddrLed));
            I2cLedThread.Start();
            ledThreadStarted = true;
            /* Change UI */
            btnI2cWrite.Content = "Stop I²C";
            btnI2cWrite.Background = Brushes.Red;
            txInfoLed.Text = "LEDs on I²C-Extension Board are blinking";
        }
        else
        {
            /* Stop the I2C Thread from running */
            I2c!.StopLed();
            ledThreadStarted = false;
            /* Change UI */
            btnI2cWrite.Content = "Start I²C";
            btnI2cWrite.Background = Brushes.LightGreen;
            txInfoLed.Text = "LEDs on I²C-Extension Board are blinking";
        }
    }

    void BtnI2cRead_Clicked(object sender, RoutedEventArgs args)
    {
        /* Send data to I2C Device */
        if(I2c.WriteData(busIdLed, devAddrStorage, valueWrite1, valueWrite2))
        {
            txInfoWrite.Text = $"Value {valueWrite1} sent to I²C Device";
            txInfoWrite.Foreground = Brushes.Blue;

            /* Read data from I2C Device, true if values match the data sent */
            if (I2c.ReadData(busIdLed, devAddrStorage))
            {
                txInfoRead.Text = $"Value {valueWrite1} read from I²C Device";
                txInfoRead.Foreground = Brushes.Green;
            }
            else
            {
                txInfoRead.Text = $"Value {valueWrite1} doesn't match the value read from I²C Device";
                txInfoRead.Foreground = Brushes.Red;
            }
        }
    }

    void BtnPwm_Clicked(object sender, RoutedEventArgs args)
    {
        double returnVoltage;
        int counter = 0;
        bool toggleOn = true;

        txPwmSend.Text = $"PWM will toggle. See LED";

        while(counter <= 9)
        {
            I2c.SetPwm(busIdLed, devAddrPwm, toggleOn);
            //TODO: Read auswerten
            returnVoltage = I2c.ReadADC(busIdLed, devAddrAdc);
            txPwmRead.Text = $"ADC received {returnVoltage} V";
            Thread.Sleep(1000);

            toggleOn = !toggleOn;
            counter++;
        }

        //if( returnVoltage == voltage1)
        //{

        //}
    }
}
