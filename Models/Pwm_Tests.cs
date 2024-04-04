using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.Pwm;
using Iot.Device.Board;
using Iot.Device.Adc;

namespace IoTLib_Test.Models
{
    internal class Pwm_Tests
    {
        //TODO: PWM Tests
        // PWM PWM3 GPIO1_IO10 IO 10 J11-34

        /* GPIO_LED Pin Number*/
        int bank;
        int pin;
        GpioDriver? drvGpio;

        //int chip;
        //int channel;


        public void PwmSet(int _bank, int _pin)
        {
            bank = _bank;
            pin = _pin;

            drvGpio = new LibGpiodDriver(bank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpio);
            controller.OpenPin(pin, PinMode.Output);


            controller.Write(pin, PinValue.High);
            Thread.Sleep(1000);
            controller.Write(pin, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(pin, PinValue.High);
            Thread.Sleep(1000);
            controller.Write(pin, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(pin, PinValue.High);
            Thread.Sleep(1000);
            controller.Write(pin, PinValue.Low);
            Thread.Sleep(1000);
            controller.Write(pin, PinValue.High);
            Thread.Sleep(1000);
            controller.Write(pin, PinValue.Low);

            //TODO: optimieren
        }

        //public void PwmSet(int _chip, int _channel)
        //{
        //    chip = _chip;
        //    channel = _channel;

        //    using Board board = Board.Create();
        //    board.CreatePwmChannel(chip, 0, 400, 50, channel, PinNumberingScheme.Logical);

        //    //using PwmChannel pwmChannel = PwmChannel.Create(chip, channel);
        //    //pwmChannel.Start();
        //}
    }
}
//TODO