using System;
using System.Collections.Generic;
using System.Threading;
using System.Device.I2c;

namespace IoTLib_Test.Models
{
    internal class I2c_Tests
    {
        bool runLedTest;
        byte valueWrite1;
        byte valueWrite2;
        byte register1 = 0x02;
        byte register2 = 0x03;

        #region ExtensionLED
        public void WriteLedValues(int busId, int deviceAddr)
        {
            runLedTest = true;
            int sleep = 200;

            /* Create I2C Device */
            I2cConnectionSettings i2cSettings = new(busId, deviceAddr);
            using var i2cDevice = I2cDevice.Create(i2cSettings);

            /* Set I2C Device to read mode */
            byte[] config = [0x06,  0x00,  0x00];
            i2cDevice.Write(config);

            /* Values to send to I2C Extension Board */
            byte[] data01 = [0x02, 0x00, 0x00]; // All Off
            byte[] data02 = [0x02, 0x01, 0x00]; // LED 1
            byte[] data03 = [0x02, 0x02, 0x00]; // LED 2
            byte[] data04 = [0x02, 0x04, 0x00]; // LED 3
            byte[] data05 = [0x02, 0x08, 0x00]; // LED 4
            byte[] data06 = [0x02, 0x10, 0x00]; // LED 5
            byte[] data07 = [0x02, 0x20, 0x00]; // LED 6
            byte[] data08 = [0x02, 0x40, 0x00]; // LED 7
            byte[] data09 = [0x02, 0x80, 0x00]; // LED 8
            byte[] data10 = [0x03, 0x01, 0x00]; // LED 9
            byte[] data11 = [0x03, 0x02, 0x00]; // LED 10
            byte[] data12 = [0x03, 0x04, 0x00]; // LED 11
            byte[] data13 = [0x03, 0x08, 0x00]; // LED 12
            byte[] data14 = [0x03, 0x10, 0x00]; // LED 13
            byte[] data15 = [0x03, 0x20, 0x00]; // LED 14
            byte[] data16 = [0x03, 0x40, 0x00]; // LED 15
            byte[] data17 = [0x03, 0x80, 0x00]; // LED 16
            byte[] data18 = [0x02, 0xff, 0xff]; // All On

            List<byte[]> bytes = [data01, data02, data03,
                                data04, data05, data06,
                                data07, data08, data09,
                                data10, data11, data12,
                                data13, data14, data15,
                                data16, data17, data18];

            /* Run LED lights until StopI2C() is called */
            while (runLedTest)
            {
                foreach (byte[] data in bytes)
                {
                    i2cDevice.Write(data);
                    Thread.Sleep(sleep);
                }
            }
        }

        public void StopLed()
        {
            runLedTest = false;
        }
        #endregion
        #region ReadWrite
        public bool WriteData(int busId, int devAddr, byte _register1, byte _register2, int _valueWrite1, int _valueWrite2)
        {
            byte[] valuesToWrite;
            register1 = _register1;
            register2 = _register2;

            /* Create I2C Device */
            I2cConnectionSettings i2cSettings = new(busId, devAddr);
            using var i2cDevice = I2cDevice.Create(i2cSettings);

            /* Set I2C Device to read mode */
            byte[] config = [0x06, 0x00, 0x00];
            i2cDevice.Write(config);

            /* Write data */
            valueWrite1 = Convert.ToByte(_valueWrite1);
            valueWrite2 = Convert.ToByte(_valueWrite2);

            valuesToWrite = [register1, valueWrite1];
            i2cDevice.Write(valuesToWrite);

            valuesToWrite = [register2, valueWrite2];
            i2cDevice.Write(valuesToWrite);

            return true;
        }
        public byte ReadData(int busId, int devAddr, byte register)
        {
            /* Create I2C Device */
            I2cConnectionSettings i2cSettings = new(busId, devAddr);
            using var i2cDevice = I2cDevice.Create(i2cSettings);

            /* Set address to register first */
            i2cDevice.WriteByte(register);
            /* Read data from set register */
            byte valueRead = i2cDevice.ReadByte();

            return valueRead;
        }
        #endregion
        #region PWM
        public bool SetPwm(int busId, int devAddr, bool toggleOn)
        {
            /* Create I2C Device */
            I2cConnectionSettings i2cSettings = new(busId, devAddr);
            using var i2cDevicePWM = I2cDevice.Create(i2cSettings);

            if (!toggleOn)
                i2cDevicePWM!.Write([0x05, 0x0]);
            else
                i2cDevicePWM!.Write([0x05, 0x1]);

            return true;
        }

        public double ReadADC(int busId, int devAddr)
        {
            int value = 0;
            byte dataRead;

            //TODO: Spannung messen, J2-17 -> ADS7828 CH0
            /* Create I2C Device */
            I2cConnectionSettings i2cSettings = new(busId, devAddr);
            using var i2cDeviceADC = I2cDevice.Create(i2cSettings);

            // i2cget -y 5 0x63 0x00
            dataRead = i2cDeviceADC.ReadByte();


            //Liest Spannung: 100% = 3,3V
            //12bit -> 100% = 4095 / 0xFFF / 0b1111 1111 1111
            
            return value;
        }
        #endregion
    }
}
