﻿using System;
using System.Globalization;

namespace IoTLib_Test.Models
{
    static class Helper
    {
        /*
         * 
         * Helper Class for different kind of conversions, byte comparison etc.
         *
         */

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

        public static int ConvertHexStringToHexInt(string? hexString, int hexInt)
        {
            if (!string.IsNullOrEmpty(hexString))
                hexInt = int.Parse(hexString, NumberStyles.HexNumber);

            return hexInt;
        }

        public static byte ConvertStringToByte(string? hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return 0;

            /* Convert the string to an integer and then to a byte */
            int value = Convert.ToInt32(hexString, 16);
            return Convert.ToByte(value);
        }

        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            /* Compare byte arrays, return true if equal */
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}