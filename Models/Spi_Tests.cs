using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Spi;
using System.IO.Pipelines;
using System.Device;
using Iot.Device.Mcp25xxx;
using Iot.Device.Mcp25xxx.Models;
using Iot.Device.Mcp25xxx.Register;
using Iot.Device.Mcp25xxx.Register.AcceptanceFilter;
using Iot.Device.Mcp25xxx.Register.BitTimeConfiguration;
using Iot.Device.Mcp25xxx.Register.CanControl;
using Iot.Device.Mcp25xxx.Tests.Register.CanControl;
using Iot.Device.Mcp25xxx.Register.ErrorDetection;
using Iot.Device.Mcp25xxx.Register.Interrupt;
using Iot.Device.Mcp25xxx.Register.MessageReceive;
using Iot.Device.Mcp25xxx.Register.MessageTransmit;
using System.Collections.Concurrent;

namespace IoTLib_Test.Models
{
    internal class Spi_Tests
    {
        public bool SpiStart(int spidev, byte register)
        {
            SpiConnectionSettings spiConnectionSettings = new(spidev, 0);
            SpiDevice spiDevice = SpiDevice.Create(spiConnectionSettings);

            /* Send reset command to empty all registers */
            byte[] reset = [0xc0];
            spiDevice.Write(reset);

            /* Data to send */
            byte dataSend1 = 0x5;
            byte dataSend2 = 0x6;

            /* Write dataSend1 to register1, dataSend2 will be written to the next register */
            byte[] writecmd = [0x2, register, dataSend1, dataSend2];
            spiDevice.Write(writecmd);

            /* Read data from the registers that were written */
            byte dataRead1 = SpiRead(spiDevice, register);
            byte dataRead2 = SpiRead(spiDevice, Convert.ToByte(register + 1));

            /* Compare return values with data sent */
            if(dataRead1 == dataSend1 && dataRead2 == dataSend2)
            {
                return true;
            }
            return false;
        }

        public byte SpiRead(SpiDevice spiDevice, byte address)
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

//TODO: Tests überarbeiten

/* Read values from all adresses */
//Array addresses = Enum.GetValues(typeof(Address));
//Mcp2515 mcp25xxx = new Mcp2515(spiDevice);
//foreach (Address address in addresses)
//{
//    byte addressData = mcp25xxx.Read(address);
//    Console.WriteLine($"0x{(byte)address:X2} - {address,-10}: 0x{addressData:X2}");
//}
