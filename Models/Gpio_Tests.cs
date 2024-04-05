using System.Threading;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;

namespace IoTLib_Test.Models
{
    internal class Gpio_Tests(int _bankLed, int _pinLed, int _bankButton, int _pinButton)
    {
        /* GPIO_LED Pin Number*/
        private int bankLed = _bankLed;
        private int pinLed = _pinLed;
        private GpioDriver? drvGpioLed;

        /* GPIO_Input Pin Number */
        private int bankButton = _bankButton;
        private int pinButton = _pinButton;
        private GpioDriver? drvGpioButton;
        private GpioController? controllerButton;

        private bool ledIsOn;

        #region GPIO_LED
        public void TurnOnLed(int _bank, int _pin)
        {
            bankLed = _bank;
            pinLed = _pin;

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

        public void BlinkLed(int _bank, int _pin)
        {
            bankLed = _bank;
            pinLed = _pin;

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
        public void ActivateButtonInput(int _bank, int _pin)
        {
            bankButton = _bank;
            pinButton = _pin;

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

        private void OnButton_Press(object sender, PinValueChangedEventArgs args)
        {
            if(ledIsOn) return;
            TurnOnLed(bankLed, pinLed);
        }

        private void OnButton_Release(object sender, PinValueChangedEventArgs args)
        {
            if(!ledIsOn) return;
            TurnOffLed();
        }
        #endregion
    }
}
