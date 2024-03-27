using Avalonia.Threading;
using System.Threading;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;

namespace IoTLib_Test.Models
{
    internal class Gpio_Tests
    {
        /* GPIO_LED Pin Number*/
        int ledBank;
        int ledPin;
        GpioDriver? drvGpioLed;

        /* GPIO_Input Pin Number */
        int inputBank;
        int inputPin;
        GpioDriver? drvGpioInput;
        GpioController? inputController;

        bool ledIsOn = false;

        public Gpio_Tests(int _ledBank, int _ledPin, int _inputBank, int _inputPin)
        {
            /* Set standard values */
            ledBank = _ledBank;
            ledPin = _ledPin;
            inputBank = _inputBank;
            inputPin = _inputPin;
        }

        #region GPIO_LED
        public void LedOn(int bank, int pin)
        {
            ledBank = bank;
            ledPin = pin;

            drvGpioLed = new LibGpiodDriver(ledBank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioLed);
            controller.OpenPin(ledPin, PinMode.Output);
            controller.Write(ledPin, PinValue.High);

            ledIsOn = true;
        }

        public void LedOff()
        {
            drvGpioLed = new LibGpiodDriver(ledBank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioLed);
            controller.OpenPin(ledPin, PinMode.Output);
            controller.Write(ledPin, PinValue.Low);

            ledIsOn = false;
        }

        public void LedBlink(int bank, int pin)
        {
            ledBank = bank;
            ledPin = pin;

            drvGpioLed = new LibGpiodDriver(ledBank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioLed);
            controller.OpenPin(ledPin, PinMode.Output);

            int blinkCount = 0;
            /* Blink 5 times */
            while (blinkCount < 10)
            {
                controller.Write(ledPin, ledIsOn ? PinValue.Low : PinValue.High);
                Thread.Sleep(500);
                ledIsOn = !ledIsOn;
                blinkCount++;
            }
        }
        #endregion
        #region GPIO_Input
        public void ReadGpioInput(int bank, int pin)
        {
            inputBank = bank;
            inputPin = pin;

            drvGpioInput = new LibGpiodDriver(inputBank);
            inputController = new GpioController(PinNumberingScheme.Logical, drvGpioInput);
            inputController.OpenPin(inputPin, PinMode.InputPullUp);

            /* Set event for hardware button clicked */
            inputController.RegisterCallbackForPinValueChangedEvent(
                inputPin,
                PinEventTypes.Falling,
                ButtonPress);
            /* Set event for hardware button released */
            inputController.RegisterCallbackForPinValueChangedEvent(
                inputPin,
                PinEventTypes.Rising,
                ButtonRelease);
        }

        public void StopGpioInput()
        {
            if (inputController != null)
            {
                inputController.ClosePin(inputPin);
                inputController.Dispose();
            }
        }

        async void ButtonPress(object sender, PinValueChangedEventArgs args)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                LedOn(ledBank, ledPin);

            });
        }

        async void ButtonRelease(object sender, PinValueChangedEventArgs args)
        {
            await Dispatcher.UIThread.InvokeAsync(LedOff);
        }
        #endregion
    }
}
