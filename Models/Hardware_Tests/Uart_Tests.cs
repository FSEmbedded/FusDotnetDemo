using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Analog;
using System.IO.Ports;
using Iot.Device.Board;
using System.Device.Gpio;
using Iot.Device.GoPiGo3.Sensors;

namespace IoTLib_Test.Models.Hardware_Tests;

internal class Uart_Tests
{
    //TODO

    // UART_D_TXD - J11-13
    // UART_D_RXD - J11-15

    // UART_A_RTS - J11-36
    // UART_A_CTS - J11-38
    // SP3232ECY

    // /dev/ttyS0
    // /dev/ttyS1
    // /dev/ttyS2
    // /dev/ttyS3

    public void UartRW()
    {
        using Board b = Board.Create();
        
    }
    

}
