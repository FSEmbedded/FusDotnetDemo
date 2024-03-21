using System.Collections.Generic;
using System.Device.I2c;
using System.Threading;

namespace IoTLib_Test.Models
{
    internal class I2c_Tests
    {
        bool runTest;

        public void StartI2cWrite(int _busId, int _deviceAddr)
        {
            runTest = true;
            int sleep = 200;

            /* Create I2C Device */
            int busId = _busId;
            int deviceAddr = _deviceAddr;
            I2cConnectionSettings i2cSettings = new(busId, deviceAddr);
            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);

            /* Set I2C Device to read mode */
            byte[] config = [0x06,  0x00,  0x00];
            i2cDevice.Write(config);

            /* Values to send to I2C Device */
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

            List<byte[]> bytes = [data01, data02, 
                                data03, data04, 
                                data05, data06, 
                                data07, data08, 
                                data09, data10, 
                                data11, data12, 
                                data13, data14, 
                                data15, data16, 
                                data17, data18];

            /* Runs until StopI2C() is called */
            while (runTest)
            {
                foreach (byte[] data in bytes)
                {
                    i2cDevice.Write(data);
                    Thread.Sleep(sleep);
                }
            }
        }

        public void StopI2cWrite()
        {
            runTest = false;
        }
    }
}
