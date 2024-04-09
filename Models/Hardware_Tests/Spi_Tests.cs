using System;
using System.Device.Spi;

namespace IoTLib_Test.Models.Hardware_Tests
{
    internal class Spi_Tests
    {
        private readonly SpiConnectionSettings spiConnectionSettings;
        private readonly SpiDevice spiDevice;
        private readonly byte register;

        public Spi_Tests(int spidev, byte _register)
        {
            try
            {
                spiConnectionSettings = new(spidev, 0);
                spiDevice = SpiDevice.Create(spiConnectionSettings);
                register = _register;
            }
            catch (Exception ex)
            {
                throw new($"SPI Device was not created: {ex.Message}");
            }
        }

        public (byte, byte) StartSpiRWTest(byte valueWrite1, byte valueWrite2)
        {
            /* Send reset command to empty all registers */
            byte[] reset = [0xc0];
            spiDevice!.Write(reset);

            /* Write valueWrite1 to register1, valueWrite2 will be written to the next register */
            byte[] writecmd = [0x2, register, valueWrite1, valueWrite2];
            spiDevice.Write(writecmd);

            /* Read data from the registers that were written */
            byte valueRead1 = SpiRead(spiDevice, register);
            byte valueRead2 = SpiRead(spiDevice, Convert.ToByte(register + 1));

            return (valueRead1, valueRead2);
        }

        private byte SpiRead(SpiDevice spiDevice, byte address)
        {
            const byte dontCare = 0x00;
            ReadOnlySpan<byte> writeBuffer =
            [
                (byte)0x3,
                address,
                dontCare
            ];
            Span<byte> readBuffer = stackalloc byte[3];
            spiDevice.TransferFullDuplex(writeBuffer, readBuffer);
            return readBuffer[2];
        }
    }
}
