/********************************************************
*                                                       *
*    Copyright (C) 2024 F&S Elektronik Systeme GmbH     *
*                                                       *
*    Author: Simon Bruegel                              *
*                                                       *
*    This file is part of FusDotnetDemo.                *
*                                                       *
*********************************************************/

using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace FusDotnetDemo.Models.Tools;

internal class InputControl
{
    /*
     * 
     * Helper Class for TexBox Inputs
     * Only allow specific characters, block input of invalid keys
     * 
     */

    public static void TextBox_DecimalInput(object sender, KeyEventArgs e)
    {
        if (sender is TextBox)
        {
            /* Check if the pressed key is a control character (like Backspace) or a digit */
            if (e.KeySymbol != null &&
                !char.IsControl(Convert.ToChar(e.KeySymbol!)) &&
                !char.IsDigit(Convert.ToChar(e.KeySymbol!)))
            {
                /* If it's not, prevent the character from being entered */
                e.Handled = true;
            }
        }
    }

    public static void TextBox_HexInput(object sender, KeyEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            /* Check if the pressed key is a control character (like Backspace) or a hexadecimal character (0-9, a-f, A-F) */
            if (e.KeySymbol != null &&
                !char.IsControl(Convert.ToChar(e.KeySymbol!)) &&
                !char.IsDigit(Convert.ToChar(e.KeySymbol!)) &&
                !(Convert.ToChar(e.KeySymbol!) >= 'a' && Convert.ToChar(e.KeySymbol!) <= 'f') &&
                !(Convert.ToChar(e.KeySymbol!) >= 'A' && Convert.ToChar(e.KeySymbol!) <= 'F'))
            {
                /* If it's not, prevent the character from being entered */
                e.Handled = true;
            }
            /* Check if the TextBox already has 2 characters */
            else if (e.KeySymbol != null && 
                    !char.IsControl(Convert.ToChar(e.KeySymbol!)) && 
                    textBox.Text != null && 
                    textBox.Text.Length >= 2)
            {
                /* Prevent further characters from being entered */
                e.Handled = true;
            }
        }
    }
}
