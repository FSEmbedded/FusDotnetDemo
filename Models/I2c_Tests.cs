using System;
using System.Collections.Generic;
using System.Threading;
using System.Device.I2c;

namespace IoTLib_Test.Models
{
    internal class I2c_Tests
    {
        private bool runLedTest;
        private readonly I2cDevice i2cDevice;

        private readonly byte busId;
        private readonly byte pwmOn = 0x0;
        private readonly byte pwmOff = 0x1;

        public I2c_Tests(int _busId, int _devAddr)
        {
            busId = (byte)_busId;

            /* Create I2C Device */
            I2cConnectionSettings i2cSettings = new(_busId, _devAddr);
            try
            {
                i2cDevice = I2cDevice.Create(i2cSettings);
                /* Test if i2cDevice was created succesfully */
                i2cDevice.WriteByte(0x00);
            }
            catch (Exception ex)
            {
                throw new("Exception: " + ex.Message);
            }
        }

        #region ExtensionLED
        public void WriteValuesLed()
        {
            runLedTest = true;
            int sleep = 200;

            /* Set I2C Device to read mode */
            byte[] config = [0x06, 0x00, 0x00];

            if (i2cDevice != null)
            {
                i2cDevice.Write(config);

                /* Values to send to I2C Extension Board */
                List<byte[]> bytes = GetLedValues();

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

        public void StopLedLoop()
        {
            runLedTest = false;
        }

        private static List<byte[]> GetLedValues()
        {
            /* Values to send to I2C Extension Board */
            byte[] value01 = [0x02, 0x00, 0x00]; // All Off
            byte[] value02 = [0x02, 0x01, 0x00]; // LED 1
            byte[] value03 = [0x02, 0x02, 0x00]; // LED 2
            byte[] value04 = [0x02, 0x04, 0x00]; // LED 3
            byte[] value05 = [0x02, 0x08, 0x00]; // LED 4
            byte[] value06 = [0x02, 0x10, 0x00]; // LED 5
            byte[] value07 = [0x02, 0x20, 0x00]; // LED 6
            byte[] value08 = [0x02, 0x40, 0x00]; // LED 7
            byte[] value09 = [0x02, 0x80, 0x00]; // LED 8
            byte[] value10 = [0x03, 0x01, 0x00]; // LED 9
            byte[] value11 = [0x03, 0x02, 0x00]; // LED 10
            byte[] value12 = [0x03, 0x04, 0x00]; // LED 11
            byte[] value13 = [0x03, 0x08, 0x00]; // LED 12
            byte[] value14 = [0x03, 0x10, 0x00]; // LED 13
            byte[] value15 = [0x03, 0x20, 0x00]; // LED 14
            byte[] value16 = [0x03, 0x40, 0x00]; // LED 15
            byte[] value17 = [0x03, 0x80, 0x00]; // LED 16
            byte[] value18 = [0x02, 0xff, 0xff]; // All On

            List<byte[]> bytes = [value01, value02, value03,
                                    value04, value05, value06,
                                    value07, value08, value09,
                                    value10, value11, value12,
                                    value13, value14, value15,
                                    value16, value17, value18];

            return bytes;
        }
        #endregion
        #region ReadWrite
        public bool WriteValueToRegister(int _register, int _valueWrite)
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

        public byte ReadValueFromRegister(int _register)
        {
            byte register = Convert.ToByte(_register);

            /* Set address to register first */
            i2cDevice.WriteByte(register);
            /* Read data from last used register */
            byte valueRead = i2cDevice.ReadByte();

            return valueRead;
        }
        #endregion
        #region PWM
        public bool WritePwm(bool toggleOn)
        {
            /* Write value to PWM device */
            if (!toggleOn)
                i2cDevice!.Write([busId, pwmOn]);
            else
                i2cDevice!.Write([busId, pwmOff]);

            return true;
        }

        public byte ReadADC()
        {
            /* Set address to register first */
            i2cDevice.WriteByte(0x00);
            /* Read value from last used register of ADC device */
            byte valueRead = i2cDevice.ReadByte();

            return valueRead;
        }
        #endregion
    }
}
