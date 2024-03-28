using System.Threading;
using Avalonia.Threading;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;

namespace IoTLib_Test.Models
{
    internal class Gpio_Tests
    {
        /* GPIO_LED Pin Number*/
        int bankLed;
        int pinLed;
        GpioDriver? drvGpioLed;

        /* GPIO_Input Pin Number */
        int bankButton;
        int pinButton;
        GpioDriver? drvGpioButton;
        GpioController? controllerButton;

        bool ledIsOn = false;

        public Gpio_Tests(int _bankLed, int _pinLed, int _bankButton, int _pinButton)
        {
            /* Set standard values */
            bankLed = _bankLed;
            pinLed = _pinLed;
            bankButton = _bankButton;
            pinButton = _pinButton;
        }

        #region GPIO_LED
        public void TurnOnLed(int bank, int pin)
        {
            bankLed = bank;
            pinLed = pin;

            drvGpioLed = new LibGpiodDriver(bankLed);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioLed);
            controller.OpenPin(pinLed, PinMode.Output);
            controller.Write(pinLed, PinValue.High);

            ledIsOn = true;
        }

        public void TurnOffLed()
        {
            drvGpioLed = new LibGpiodDriver(bankLed);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioLed);
            controller.OpenPin(pinLed, PinMode.Output);
            controller.Write(pinLed, PinValue.Low);

            ledIsOn = false;
        }

        public void BlinkLed(int bank, int pin)
        {
            bankLed = bank;
            pinLed = pin;

            drvGpioLed = new LibGpiodDriver(bankLed);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioLed);
            controller.OpenPin(pinLed, PinMode.Output);

            int blinkCount = 0;
            /* Blink 5 times */
            while (blinkCount < 10)
            {
                controller.Write(pinLed, ledIsOn ? PinValue.Low : PinValue.High);
                Thread.Sleep(500);
                ledIsOn = !ledIsOn;
                blinkCount++;
            }
        }
        #endregion
        #region GPIO_Input
        public void ActivateButtonInput(int bank, int pin)
        {
            bankButton = bank;
            pinButton = pin;

            drvGpioButton = new LibGpiodDriver(bankButton);
            controllerButton = new GpioController(PinNumberingScheme.Logical, drvGpioButton);
            controllerButton.OpenPin(pinButton, PinMode.InputPullUp);

            /* Set event for hardware button clicked */
            controllerButton.RegisterCallbackForPinValueChangedEvent(
                pinButton,
                PinEventTypes.Falling,
                OnButton_Press);
            /* Set event for hardware button released */
            controllerButton.RegisterCallbackForPinValueChangedEvent(
                pinButton,
                PinEventTypes.Rising,
                OnButton_Release);
        }

        public void StopButtonInput()
        {
            if (controllerButton != null)
            {
                controllerButton.ClosePin(pinButton);
                controllerButton.Dispose();
            }
        }

        async void OnButton_Press(object sender, PinValueChangedEventArgs args)
        {
            //TODO: Dispatcher ohne Avalonia
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                TurnOnLed(bankLed, pinLed);
            });
        }

        async void OnButton_Release(object sender, PinValueChangedEventArgs args)
        {
            //TODO: Dispatcher ohne Avalonia
            await Dispatcher.UIThread.InvokeAsync(TurnOffLed);
        }
        #endregion
    }
}
