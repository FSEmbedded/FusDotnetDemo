using Avalonia.Threading;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;

namespace IoTLib_Test.Models
{
    internal class Gpio_Tests
    {
        /* GPIO_LED Pin Number*/
        readonly int GPIO_J1_54 = 1; //TODO: Wert bei Initialisierung übergeben
        readonly int ledBank;
        readonly int ledPin;
        GpioDriver? drvGpioLed;

        /* GPIO_Input Pin Number */
        readonly int GPIO_J1_52 = 78; //TODO: Wert bei Initialisierung übergeben
        readonly int inputbank;
        readonly int inputPin;
        GpioDriver? drvGpioInput;
        GpioController? inputController;

        public Gpio_Tests()
        {
            /* GPIO_LED */
            ledBank = PinConverter.GetGpioBank(GPIO_J1_54);
            ledPin = PinConverter.GetGpioPin(GPIO_J1_54);
            drvGpioLed = new LibGpiodDriver(ledBank);

            /* GPIO_Input */
            inputbank = PinConverter.GetGpioBank(GPIO_J1_52);
            inputPin = PinConverter.GetGpioPin(GPIO_J1_52);
            drvGpioInput = new LibGpiodDriver(inputbank);
        }

        #region GPIO_LED
        public void LedOn()
        {
            drvGpioLed = new LibGpiodDriver(ledBank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioLed);
            controller.OpenPin(ledPin, PinMode.Output);
            controller.Write(ledPin, PinValue.High);
        }

        public void LedOff()
        {
            drvGpioLed = new LibGpiodDriver(ledBank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioLed);
            controller.OpenPin(ledPin, PinMode.Output);
            controller.Write(ledPin, PinValue.Low);
        }

        public void LedBlink()
        {
            drvGpioLed = new LibGpiodDriver(ledBank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvGpioLed);
            controller.OpenPin(ledPin, PinMode.Output);

            bool ledOn = false;
            int blinkCount = 0;
            /* Blink 5 times */
            while (blinkCount < 10)
            {
                controller.Write(ledPin, ledOn ? PinValue.Low : PinValue.High);
                Thread.Sleep(500);
                ledOn = !ledOn;
                blinkCount++;
            }
        }
        #endregion
        #region GPIO_Input
        public void ReadGpioInput()
        {
            drvGpioInput = new LibGpiodDriver(inputbank);
            inputController = new GpioController(PinNumberingScheme.Logical, drvGpioInput);
            inputController.OpenPin(inputPin, PinMode.InputPullUp);

            /* Set event for hardware button clicked */
            inputController.RegisterCallbackForPinValueChangedEvent(
                inputPin,
                PinEventTypes.Falling,
                ButtonClicked);
            /* Set event for hardware button released */
            inputController.RegisterCallbackForPinValueChangedEvent(
                inputPin,
                PinEventTypes.Rising,
                ButtonReleased);
        }

        public void StopGpioInput()
        {
            if (inputController != null)
            {
                inputController.ClosePin(inputPin);
                inputController.Dispose();
            }
        }

        async void ButtonClicked(object sender, PinValueChangedEventArgs args)
        {
            //TODO: Thread
            /* Only update text if LED switched from off to on */
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                LedOn();
                UpdateInfoText();
                //TODO: Feedback in UI!

            });
        }

        async void ButtonReleased(object sender, PinValueChangedEventArgs args)
        {
            //TODO: Thread
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                LedOff();
                //TODO: Feedback in UI!
            });
        }

        void UpdateInfoText()
        {
            //tbGpioIn.Text = "Button press detected. Count: " + buttonClickCount.ToString();
        }
        #endregion
    }
}
