using Avalonia.Threading;
using System.Threading;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;

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

        bool ledIsOn = false;

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

        public void LedBlink()
        {
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

            ///* Set event for hardware button released */
            //inputController.RegisterCallbackForPinValueChangedEvent(
            //    inputPin,
            //    PinEventTypes.Falling | PinEventTypes.Rising,
            //    ButtonAction);
        }

        public void StopGpioInput()
        {
            if (inputController != null)
            {
                inputController.ClosePin(inputPin);
                inputController.Dispose();
            }
        }

        //void ButtonAction(object sender, PinValueChangedEventArgs args)
        //{
        //    Thread test;
        //    Thread ledOnThread = new Thread(new ThreadStart(LedOn));
        //    Thread ledOffThread = new Thread(new ThreadStart(LedOff));

        //    test = args.ChangeType is PinEventTypes.Rising ? ledOffThread : ledOnThread;

        //    test.Start();
        //    //if(ledOnThread.ThreadState != ThreadState.Running && ledOffThread.ThreadState != ThreadState.Running)
        //    //{
        //    //    if (!ledIsOn)
        //    //    {
        //    //        ledOnThread.Start();
        //    //    }
        //    //    else if (ledIsOn)
        //    //    {
        //    //        ledOffThread.Start();
        //    //    }
        //    //}
        //}
        async void ButtonClicked(object sender, PinValueChangedEventArgs args)
        {
            //if (!ledIsOn)
            //{
            //    Thread ledOnThread = new Thread(new ThreadStart(LedOn));
            //    ledOnThread.Start();
            //}


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
            //if(ledIsOn)
            //{
            //    Thread ledOffThread = new Thread(new ThreadStart(LedOff));
            //    ledOffThread.Start();
            //}


            //TODO: Thread
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                LedOff();
                //TODO: Feedback in UI!
            });
        }

        void UpdateInfoText()
        {
            //TODO
            //tbGpioIn.Text = "Button press detected. Count: " + buttonClickCount.ToString();
        }
        #endregion
    }
}
