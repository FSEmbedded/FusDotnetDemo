using System;
using System.Collections.Generic;
using System.Threading;
using System.Device.I2c;

namespace IoTLib_Test.Models
{
    internal class I2c_Tests
    {
        bool runLedTest;
        readonly I2cDevice i2cDevice;

        public I2c_Tests(int busId, int devAddr)
        {
            /* Create I2C Device */
            I2cConnectionSettings i2cSettings = new(busId, devAddr);
            try
            {
                i2cDevice = I2cDevice.Create(i2cSettings);
                /* Test if i2cDevice was created succesfully */
                i2cDevice.WriteByte(0x00);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
        }

        #region ExtensionLED
        public void WriteLedValues()
        {
            runLedTest = true;
            int sleep = 200;

            /* Set I2C Device to read mode */
            byte[] config = [0x06, 0x00, 0x00];

            if (i2cDevice != null)
            {
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
            else
                runLedTest = false;
        }

        public void StopLed()
        {
            runLedTest = false;
        }
        #endregion
        #region ReadWrite
        public bool WriteData(int _register, int _valueWrite)
        {
            try
            {
                byte[] valuesToWrite;
                byte register = Convert.ToByte(_register);

                /* Set I2C Device to read mode */
                byte[] config = [0x06, 0x00, 0x00];
                i2cDevice.Write(config);

                /* Write data */
                byte valueWrite1 = Convert.ToByte(_valueWrite);

                valuesToWrite = [register, valueWrite1];
                i2cDevice.Write(valuesToWrite);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
        }
        public byte ReadData(int _register)
        {
            byte register = Convert.ToByte(_register);

            /* Set address to register first */
            i2cDevice.WriteByte(register);
            /* Read data from set register */
            byte valueRead = i2cDevice.ReadByte();

            return valueRead;
        }
        #endregion
        #region PWM
        public bool SetPwm(bool toggleOn)
        {
            if (!toggleOn)
                i2cDevice!.Write([0x05, 0x0]); //TODO: values als Variable
            else
                i2cDevice!.Write([0x05, 0x1]); //TODO: values als Variable

            return true;
        }

        public double ReadADC()
        {
            int value = 0;
            byte dataRead;

            //TODO: Spannung messen, J2-17 -> ADS7828 CH0
            
            // i2cget -y 5 0x63 0x00
            dataRead = i2cDevice.ReadByte();


            //Liest Spannung: 100% = 3,3V
            //12bit -> 100% = 4095 / 0xFFF / 0b1111 1111 1111
            
            return value;
        }
        #endregion
    }
}
