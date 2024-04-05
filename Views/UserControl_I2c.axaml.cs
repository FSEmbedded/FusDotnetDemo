using System;
using System.Globalization;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models;

namespace IoTLib_Test.Views;

public partial class UserControl_I2c : UserControl
{
    /* I²C functions are in separate class */
    I2c_Tests? I2c;
    I2c_Tests? I2cPwm;
    I2c_Tests? I2cAdc;
    /* Standard IDs and addresses */
    private int busIdLed = 0x5;
    private int devAddrLed = 0x23;
    private int busIdStorage = 0x5;
    private int devAddrStorage = 0x23;
    private int busIdPwm = 0x5;
    private int devAddrPwm = 0x63;
    private int busIdAdc = 0x5;
    private int devAddrAdc = 0x4b;
    /* Values to write on I2C storage */
    private int valueWrite1 = 0xAA;
    private int valueWrite2 = 0x55;
    private int register1 = 0x02;
    private int register2 = 0x03;

    private bool ledThreadStarted = false;

    public UserControl_I2c()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
    }

    private void BtnI2cLed_Clicked(object sender, RoutedEventArgs args)
    {
        if (!ledThreadStarted)
        {
            /* Convert  values from UI to hex */
            busIdLed = Helper.ConvertHexStringToHexInt(tbBusIdLed.Text, busIdLed);
            devAddrLed = Helper.ConvertHexStringToHexInt(tbDevAddrLed.Text, devAddrLed);
            
            try
            {
                /* Create new I2c_Tests */
                I2c = new(busIdLed, devAddrLed);
            }
            catch (Exception ex)
            {
                /* Show exception */
                txInfoLed.Text = ex.Message;
                txInfoLed.Foreground = Brushes.Red;
                return;
            }

            /* Create new Thread, start I2C Test */
            Thread I2cLedThread = new(() => I2c!.WriteValuesLed());
            I2cLedThread.Start();
            ledThreadStarted = true;
            /* Change UI */
            btnI2cLed.Content = "Stop LED Test";
            btnI2cLed.Background = Brushes.Red;
            txInfoLed.Text = "LEDs on I²C-Extension Board are blinking";
            txInfoLed.Foreground = Brushes.Blue;
        }
        else
        {
            /* Stop the I2C Thread from running */
            I2c!.StopLedLoop();
            ledThreadStarted = false;
            /* Change UI */
            btnI2cLed.Content = "Start LED Test";
            btnI2cLed.Background = Brushes.LightGreen;
            txInfoLed.Text = "LEDs on I²C-Extension Board stopped blinking";
            txInfoLed.Foreground = Brushes.Blue;
        }
    }

    private void BtnI2cRW_Clicked(object sender, RoutedEventArgs args)
    {
        /* Convert  values from UI to hex */
        busIdStorage = Helper.ConvertHexStringToHexInt(tbBusIdStorage.Text, busIdStorage);
        devAddrStorage = Helper.ConvertHexStringToHexInt(tbDevAddrStorage.Text, devAddrStorage);
        valueWrite1 = Helper.ConvertHexStringToHexInt(tbValue1.Text, valueWrite1);
        valueWrite2 = Helper.ConvertHexStringToHexInt(tbValue2.Text, valueWrite2);
        register1 = Helper.ConvertHexStringToHexInt(tbReg1.Text, register1);
        register2 = Helper.ConvertHexStringToHexInt(tbReg2.Text, register2);

        try
        {
            /* Create new I2c_Tests */
            I2c = new(busIdStorage, devAddrStorage);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txInfoWrite.Text = ex.Message;
            txInfoWrite.Foreground = Brushes.Red;
            txInfoRead.Text = "";
            return;
        }

        bool writeSuccess;
        try
        {
            writeSuccess = I2c!.WriteValueToRegister(register1, valueWrite1);
            writeSuccess = I2c!.WriteValueToRegister(register2, valueWrite2);
        }
        catch (Exception ex)
        {
            txInfoWrite.Text = ex.Message;
            txInfoWrite.Foreground = Brushes.Red;
            txInfoRead.Text = "";
            return;
        }

        /* Send values to I2C Device */
        if (writeSuccess)
        {
            txInfoWrite.Text = $"Values 0x{valueWrite1:X} & 0x{valueWrite2:X} sent to I²C Device";
            txInfoWrite.Foreground = Brushes.Blue;

            /* Read values from I2C Device */
            byte valueRead1 = I2c.ReadValueFromRegister(register1);
            byte valueRead2 = I2c.ReadValueFromRegister(register2);

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

    private void BtnI2cPwm_Clicked(object sender, RoutedEventArgs args)
    {
        /* Convert  values from UI to hex */
        busIdPwm = Helper.ConvertHexStringToHexInt(tbBusIdPwm.Text, busIdPwm);
        devAddrPwm = Helper.ConvertHexStringToHexInt(tbDevAddrPwm.Text, devAddrPwm);
        busIdAdc = Helper.ConvertHexStringToHexInt(tbBusIdAdc.Text, busIdAdc);
        devAddrAdc = Helper.ConvertHexStringToHexInt(tbDevAddrAdc.Text, devAddrAdc);

        try
        {
            /* Create new I2c_Tests */
            I2cPwm = new(busIdPwm, devAddrPwm);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txPwmSend.Text = ex.Message;
            txPwmSend.Foreground = Brushes.Red;
            return;
        }
        try
        {
            /* Create new I2c_Tests */
            I2cAdc = new(busIdAdc, devAddrAdc);
        }
        catch (Exception ex)
        {
            /* Show exception */
            txPwmRead.Text = ex.Message;
            txPwmRead.Foreground = Brushes.Red;
            return;
        }

        byte returnValue;
        int counter = 0;
        bool toggleOn = true;

        while(counter < 6)
        {
            I2cPwm!.WritePwm(toggleOn);
            toggleOn = !toggleOn;
            returnValue = I2cAdc!.ReadADC();
            /* calculate voltage from return value */
            double voltage = (double)returnValue / (double)0xff * 100;
            /* When LED is on, returnValue should be 0 */
            if (!toggleOn && returnValue != 0)
            {
                txPwmRead.Foreground = Brushes.Red;
                txPwmRead.Text += $"LED is off, Voltage should be 0V. ADC detected {voltage:F2}V\r\n";
            }
            else if (!toggleOn && returnValue == 0)
            {
                txPwmRead.Foreground = Brushes.Green;
                txPwmRead.Text += $"LED is on, ADC detected: {voltage:F2}V\r\n";
            }
            else
            {
                
                txPwmRead.Foreground = Brushes.Green;
                txPwmRead.Text += $"LED is off, ADC detected: {voltage:F2}V\r\n";
            }
            counter++;
            Thread.Sleep(1000);
        }
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnI2cLed.AddHandler(Button.ClickEvent, BtnI2cLed_Clicked!);
        btnI2cRW.AddHandler(Button.ClickEvent, BtnI2cRW_Clicked!);
        btnI2cPwm.AddHandler(Button.ClickEvent, BtnI2cPwm_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes*/
        tbBusIdLed.Text = busIdLed.ToString("X");
        tbDevAddrLed.Text = devAddrLed.ToString("X");
        tbBusIdStorage.Text = busIdStorage.ToString("X");
        tbDevAddrStorage.Text = devAddrStorage.ToString("X");
        tbValue1.Text = valueWrite1.ToString("X");
        tbValue2.Text = valueWrite2.ToString("X");
        tbReg1.Text = register1.ToString("X");
        tbReg2.Text = register2.ToString("X");
        tbBusIdPwm.Text = busIdPwm.ToString("X");
        tbDevAddrPwm.Text = devAddrPwm.ToString("X");
        tbBusIdAdc.Text = busIdAdc.ToString("X");
        tbDevAddrAdc.Text = devAddrAdc.ToString("X");
    }

    private void AddTextBoxHandlers()
    {
        /* Add handler to only allow hex value inputs */
        tbBusIdLed.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbDevAddrLed.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbBusIdStorage.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbDevAddrStorage.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbValue1.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbValue2.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbReg1.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbReg2.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbBusIdPwm.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbDevAddrPwm.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbBusIdAdc.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
        tbDevAddrAdc.AddHandler(KeyDownEvent, InputControl.TextBox_HexInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        /* Description Text */
        txDescLed.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16";
        txDescStorage.Text = "Connect BBDSI with I²C Extension Board: I2C_A_SCL = J11-16 -> J1-11, I2C_A_SDA = J11-17 -> J1-10, GND = J11-37 -> J1-16";
        txDescPwm.Text = "Connect I²C Extension Board Pins: J2-17 -> J2-27; Set S2-3 to ON";

        txInfoLed.Text = "";
        txInfoWrite.Text = "";
        txInfoRead.Text = "";
        txPwmSend.Text = "";
        txPwmRead.Text = "";
    }
}
