using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace IoTLib_Test
{
    public partial class MainWindow : Window
    {
        /* GPIO_LED Pin Number*/
        int GPIO_J1_54 = 1;
        /* GPIO_Input Pin Number */
        int GPIO_J1_52 = 78;

        /* GPIO_LED Values */
        int ledBank;
        int ledPin;
        GpioDriver? drvLed;
        /* GPIO_Input values */
        int inputBank;
        int inputPin;
        GpioDriver? drvInput;
        /* Used for button (de)activation */
        bool LedButtonEnabled = true;
        bool GpioInButtonEnabled = true;

        bool LedIsOn = false;
        int buttonClickCount;
        GpioController? inputController;
        GpioController? ledController;

        public MainWindow()
        {
            InitializeComponent();

            /* GPIO_LED */
            ledBank = PinConverter.GetGpioBank(GPIO_J1_54);
            ledPin = PinConverter.GetGpioPin(GPIO_J1_54);
            drvLed = new LibGpiodDriver(ledBank);
            /* GPIO_LED button bindings */
            btnLedOn.Command = ReactiveCommand.Create(LedOn);
            btnLedOn.IsEnabled = LedButtonEnabled;
            btnLedBlink.Command = ReactiveCommand.Create(LedBlink);
            btnLedOff.Command = ReactiveCommand.Create(LedOff);
            btnLedOff.IsEnabled = !LedButtonEnabled;
            /* GPIO_Input */
            inputBank = PinConverter.GetGpioBank(GPIO_J1_52);
            inputPin = PinConverter.GetGpioPin(GPIO_J1_52);
            drvInput = new LibGpiodDriver(inputBank);
            /* GPIO_Input button bindings */
            btnGpioInputActive.Command = ReactiveCommand.Create(ReadGpioInput);
            btnGpioInputActive.IsEnabled = GpioInButtonEnabled;
            btnGpioInputInactive.Command = ReactiveCommand.Create(StopGpioInput);
            btnGpioInputInactive.IsEnabled = !GpioInButtonEnabled;
        }
        #region GPIO_LED
        void LedOn()
        {
            drvLed = new LibGpiodDriver(ledBank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvLed);
            controller.OpenPin(ledPin, PinMode.Output);
            controller.Write(ledPin, PinValue.High);

            LedIsOn = true;
            tbInfo.Text = "LED on Pin J11-8 is on";
            btnLedOn.IsEnabled = !LedButtonEnabled;
            btnLedOff.IsEnabled = LedButtonEnabled;
        }

        void LedOff()
        {
            drvLed = new LibGpiodDriver(ledBank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvLed);
            controller.OpenPin(ledPin, PinMode.Output);
            controller.Write(ledPin, PinValue.Low);

            LedIsOn = false;
            tbInfo.Text = "LED on Pin J11-8 is off";
            btnLedOn.IsEnabled = LedButtonEnabled;
            btnLedOff.IsEnabled = !LedButtonEnabled;
        }

        void LedBlink()
        {
            drvLed = new LibGpiodDriver(ledBank);
            using var controller = new GpioController(PinNumberingScheme.Logical, drvLed);
            controller.OpenPin(ledPin, PinMode.Output);

            bool ledOn = true;
            int blinkCount = 0;
            tbInfo.Text = "LED on Pin J11-8 is blinking";
            /* Blink 5 times */
            while (blinkCount < 10)
            {
                controller.Write(ledPin, ledOn ? PinValue.High : PinValue.Low);
                Thread.Sleep(500);
                ledOn = !ledOn;
                blinkCount++;
                LedIsOn = ledOn;
            }
            /* Turn off the lights before leaving */
            LedOff();
        }
        #endregion
        #region GPIO_Input
        void ReadGpioInput()
        {
            /* Reset counter */
            buttonClickCount = 0;

            inputBank = PinConverter.GetGpioBank(GPIO_J1_52);
            inputPin = PinConverter.GetGpioPin(GPIO_J1_52);
            drvInput = new LibGpiodDriver(inputBank);

            inputController = new GpioController(PinNumberingScheme.Logical, drvInput);
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

            tbInfo.Text = "Waiting for Hardware-Button click";
            btnGpioInputActive.IsEnabled = !GpioInButtonEnabled;
            btnGpioInputInactive.IsEnabled = GpioInButtonEnabled;
        }

        void StopGpioInput()
        {
            if(inputController != null)
            {
                inputController.ClosePin(inputPin);
                inputController.Dispose();
            }
            if(ledController != null)
            {
                ledController.ClosePin(ledPin);
                ledController.Dispose();
            }
            /* Turn off the lights before leaving */
            LedOff();

            tbInfo.Text = "Hardware-Button deactivated";
            btnGpioInputActive.IsEnabled = GpioInButtonEnabled;
            btnGpioInputInactive.IsEnabled = !GpioInButtonEnabled;
        }

        async void ButtonClicked(object sender, PinValueChangedEventArgs args)
        {
            bool tempLedIsOn = LedIsOn;

            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                LedOn();
            });
            /* Only update text if LED switched from off to on */
            if (tempLedIsOn != LedIsOn)
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    UpdateInfoText(); //TODO
                });
            }
        }

        async void ButtonReleased(object sender, PinValueChangedEventArgs args)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                LedOff();
            });
        }

        void UpdateInfoText()
        {
            buttonClickCount++;
            tbGpioIn.Text = "Button press detected. Count: " + buttonClickCount.ToString();
        }
        #endregion
    }
}