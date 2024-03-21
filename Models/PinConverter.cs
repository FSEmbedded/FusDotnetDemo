namespace IoTLib_Test.Models
{
    static class PinConverter
    {
        public static int GetGpioBank(int pin)
        {
            int bank = pin / 32;
            return bank;
        }

        public static int GetGpioPin(int pin)
        {
            int gpioPin = pin % 32;
            return gpioPin;
        }
    }
}
