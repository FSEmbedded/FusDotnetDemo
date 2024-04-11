using System;
using System.Device.Spi;

namespace IoTLib_Test.Models.Hardware_Tests;

internal class Spi_Tests
{
    private readonly SpiConnectionSettings spiConnectionSettings;
    private readonly SpiDevice spiDevice;

    public Spi_Tests(int spidev)
    {
        try
        {
            /* Create SPI device */
            spiConnectionSettings = new(spidev, 0);
            spiDevice = SpiDevice.Create(spiConnectionSettings);
        }
        catch (Exception ex)
        {
            throw new($"SPI Device was not created: {ex.Message}");
        }
    }

    public byte StartSpiRWTest(byte register, byte valueWrite)
    {
        /* Send reset command to empty all registers */
        byte[] reset = [0xc0];
        spiDevice!.Write(reset);

        /* Write valueWrite1 to register1, valueWrite2 will be written to the next register */
        byte[] writecmd = [0x2, register, valueWrite];
        spiDevice.Write(writecmd);

        /* Read data from the registers that were written */
        byte valueRead = SpiRead(spiDevice, register);

        return valueRead;
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
