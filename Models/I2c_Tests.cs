using System;
using System.Collections.Generic;
using System.Threading;
using System.Device.I2c;

namespace IoTLib_Test.Models
{
    internal class I2c_Tests
    {
        bool runTest;
        private readonly List<byte[]> bytes;
        private readonly int sleep = 200;

        byte value1;
        byte value2;
        byte[]? dataToSend;

        public I2c_Tests()
        {
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

            bytes = [data01, data02, data03,
                    data04, data05, data06,
                    data07, data08, data09,
                    data10, data11, data12,
                    data13, data14, data15,
                    data16, data17, data18];
        }

        #region I2CWrite
        public void WriteLedValues(int _busId, int _deviceAddr)
        {
            runTest = true;

            /* Create I2C Device */
            int busId = _busId;
            int deviceAddr = _deviceAddr;
            I2cConnectionSettings i2cSettings = new(busId, deviceAddr);
            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);

            /* Set I2C Device to read mode */
            byte[] config = [0x06,  0x00,  0x00];
            i2cDevice.Write(config);

            /* Run LED lights until StopI2C() is called */
            while (runTest)
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
            runTest = false;
        }
        #endregion
        #region I2CRead
        public bool WriteData(int _busId, int _deviceAddr, int _value1, int _value2)
        {
            /* Create I2C Device */
            int busId = _busId;
            int deviceAddr = _deviceAddr;
            I2cConnectionSettings i2cSettings = new(busId, deviceAddr);
            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);

            /* Set I2C Device to read mode */
            byte[] config = [0x06, 0x00, 0x00];
            i2cDevice.Write(config);

            /* Write data */
            value1 = Convert.ToByte(_value1);
            value2 = Convert.ToByte(_value2);

            dataToSend = [0x02, value1];
            i2cDevice.Write(dataToSend);

            dataToSend = [0x03, value2];
            i2cDevice.Write(dataToSend);

            return true;
        }
        public bool ReadData(int _busId, int _deviceAddr)
        {

            byte dataRead;

            /* Create I2C Device */
            int busId = _busId;
            int deviceAddr = _deviceAddr;
            I2cConnectionSettings i2cSettings = new(busId, deviceAddr);
            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);

            /* Read data */
            //i2cDevice.WriteByte(0x03);
            dataRead = i2cDevice.ReadByte(); //TODO: Read von definierter Adresse
            if (dataRead != value1)
                return false;

            //i2cDevice.WriteByte(0x02);
            dataRead = i2cDevice.ReadByte();
            if (dataRead != value2)
                return false;

            return true; //TODO: return int gelesener  wert?!
        }
        #endregion
        #region PWM

        I2cDevice? i2cDevicePWM; //= I2cDevice.Create(i2cSettings);
        public bool SetPwm(int _busId, int _deviceAddr)
        {

            /* Create I2C Device */
            int busId = _busId;
            int deviceAddr = _deviceAddr;
            I2cConnectionSettings i2cSettings = new(busId, deviceAddr);
            i2cDevicePWM = I2cDevice.Create(i2cSettings);

            return true;
        }

        public void TogglePWM()
        {
            int counter = 0;

            /* Write data */
            while (counter <= 10)
            {
                /* set values */
                i2cDevicePWM!.Write([0x05, 0x0]);
                Thread.Sleep(1000);
                i2cDevicePWM!.Write([0x05, 0x1]);
                Thread.Sleep(1000);

                counter++;
            }
        }
        public double ReadADC()
        {
            int value = 0;

            //Liest Spannung: 100% = 3,3V
            //12bit -> 100% = 4095 / 0xFFF / 0b1111 1111 1111

            return value;
        }
        #endregion
    }
}
