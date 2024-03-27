using System;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models;

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
    private int devAddrAdc = 0x63;
    /* Values to write on I2C storage */
    private int valueWrite1 = 0xAA;
    private int valueWrite2 = 0x55;
    private byte register1 = 0x02;
    private byte register2 = 0x03;

    public UserControl_I2c()
    {
        InitializeComponent();

        /* button bindings */
        btnI2cLed.AddHandler(Button.ClickEvent, BtnI2cLed_Clicked!);
        btnI2cRead.AddHandler(Button.ClickEvent, BtnI2cRead_Clicked!);
        btnI2cPwm.AddHandler(Button.ClickEvent, BtnI2cPwm_Clicked!);

        /* Write standard values in textboxes*/
        tbBusIdLed.Text = busIdLed.ToString("X");
        tbDevAddrLed.Text = devAddrLed.ToString("X");
        tbBusIdStorage.Text = busIdStorage.ToString("X");
        tbDevAddrStorage.Text = devAddrStorage.ToString("X");
        tbValue1.Text = valueWrite1.ToString("X");
        tbValue2.Text = valueWrite2.ToString("X");
        tbReg1.Text = register1.ToString("X");
        tbReg2.Text = register1.ToString("X");
        tbBusIdPwm.Text = busIdPwm.ToString("X");
        tbDevAddrPwm.Text = devAddrPwm.ToString("X");
        tbBusIdAdc.Text = busIdAdc.ToString("X");
        tbDevAddrAdc.Text = devAddrAdc.ToString("X");

        /* Handler to only allow hex value inputs */
        tbBusIdLed.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbDevAddrLed.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbBusIdStorage.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbDevAddrStorage.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbValue1.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbValue2.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbReg1.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbReg2.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbBusIdPwm.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbDevAddrPwm.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbBusIdAdc.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);
        tbDevAddrAdc.AddHandler(KeyDownEvent, TextBox_KeyDown!, RoutingStrategies.Tunnel);


        /* Description Text */
        txDescLed.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16";
        txDescStorage.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16";
        txDescPwm.Text = "Connect I²C Extension Board Pins: J2-17 -> J2-27; Set S2-3 to ON";

        txInfoLed.Text = "";
        txInfoWrite.Text = "";
        txInfoRead.Text = "";
        txPwmSend.Text = "";
        txPwmRead.Text = "";

        I2c = new I2c_Tests();
    }

    void BtnI2cLed_Clicked(object sender, RoutedEventArgs args)
    {
        //TODO: Werte aus UI übernehmen
        if (!ledThreadStarted)
        {
            /* Create new Thread, start I2C Test */
            Thread I2cLedThread = new(() => I2c.WriteLedValues(busIdLed, devAddrLed));
            I2cLedThread.Start();
            ledThreadStarted = true;
            /* Change UI */
            btnI2cLed.Content = "Stop LED Test";
            btnI2cLed.Background = Brushes.Red;
            txInfoLed.Text = "LEDs on I²C-Extension Board are blinking";
        }
        else
        {
            /* Stop the I2C Thread from running */
            I2c!.StopLed();
            ledThreadStarted = false;
            /* Change UI */
            btnI2cLed.Content = "Start LED Test";
            btnI2cLed.Background = Brushes.LightGreen;
            txInfoLed.Text = "LEDs on I²C-Extension Board are blinking";
        }
    }

    void BtnI2cRead_Clicked(object sender, RoutedEventArgs args)
    {
        //TODO: Werte aus UI übernehmen

        /* Send values to I2C Device */
        if (I2c.WriteData(busIdStorage, devAddrStorage, register1, register2, valueWrite1, valueWrite2))
        {
            txInfoWrite.Text = $"Values 0x{valueWrite1:X} & 0x{valueWrite2:X} sent to I²C Device";
            txInfoWrite.Foreground = Brushes.Blue;

            /* Read values from I2C Device */
            byte valueRead1 = I2c.ReadData(busIdStorage, devAddrStorage, register1);
            byte valueRead2 = I2c.ReadData(busIdStorage, devAddrStorage, register2);

            /* Check if values read and write are equal */
            if (valueRead1 == valueWrite1 && valueRead2 == valueWrite2)
            {
                txInfoRead.Text = $"Value 0x{valueRead1:X} & 0x{valueRead2:X} read from I²C Device";
                txInfoRead.Foreground = Brushes.Green;
            }
            else
            {
                txInfoRead.Text = $"Values 0x{valueRead1:X} & 0x{valueRead2:X} don't match the values written to I²C Device";
                txInfoRead.Foreground = Brushes.Red;
            }
        }
    }

    void BtnI2cPwm_Clicked(object sender, RoutedEventArgs args)
    {
        //TODO: Werte aus UI übernehmen

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

    //TODO: Handler anpassen -> HexValues, buchstabe a-f
    void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        /* Check if the pressed key is a control character (like Backspace) or a digit */
        if (!char.IsControl(Convert.ToChar(e.KeySymbol!)) && !char.IsDigit(Convert.ToChar(e.KeySymbol!)))
        {
            /* If it's not, prevent the character from being entered */
            e.Handled = true;
        }
    }
}
