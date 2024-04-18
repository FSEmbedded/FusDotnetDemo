using System;
using System.IO;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using dotnetIot_Demo.Models.Tools;
using dotnetIot_Demo.Models.Hardware;
using Iot.Device.Card.CreditCardProcessing;


namespace dotnetIot_Demo.Views;

public partial class UserControl_Camera : UserControl
{
    /* Camera functions are in a separate class */
    private Camera_Demo? Camera;
    private readonly string imgFile = "/home/root/img_camtest.png";
    private int busId;
    private uint width = 1920;
    private uint height = 1080;

    public UserControl_Camera()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        SetupComboBox();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
        ActivateButtonCamera(false);
    }

    private void BtnGetCams_Clicked(object sender, RoutedEventArgs args)
    {
        /* Find all available Cameras */
        List<string> cameras = Camera_Demo.GetAvailableCameras();

        /* Add all available cameras to the ComboBox */
        cbCams.ItemsSource = cameras;

        /* Select camera in ComboBox */
        cbCams.SelectedIndex = 0;
    }

    private void BtnCamera_Clicked(object sender, RoutedEventArgs args)
    {
        /* Get Camera Bus ID and resolution from TextBoxes */
        GetValuesFromTextBox();

        try
        {
            /* Create new object Camera_Tests */
            Camera = new Camera_Demo(busId, width, height);
        }
        catch (Exception ex)
        {
            txInfoCamera.Text = ex.Message;
            txInfoCamera.Foreground = Brushes.Red;
            return;
        }

        if (Camera.CaptureCam(imgFile))
        {
            try
            {
                /* Show image in UI */
                Avalonia.Media.Imaging.Bitmap bitmap = new(imgFile);
                imgCamCapture.Source = bitmap;
                if ((bool)cbKeepFile.IsChecked!)
                {
                    txInfoCamera.Text = $"Image is stored at {imgFile}";
                    txInfoCamera.Foreground = Brushes.Green;
                }
            }
            catch (Exception ex)
            {
                imgCamCapture.Source = null;
                txInfoCamera.Text = $"Error showing image \"{imgFile}\":\r\n{ex.Message}";
                txInfoCamera.Foreground = Brushes.Red;
            }
        }
        else
        {
            imgCamCapture.Source = null;
            txInfoCamera.Text = $"Is {cbCams.SelectedItem} a valid camera?\r\n{imgFile} has a size of 0 byte, please select another camera from the dropdown.";
            txInfoCamera.Foreground = Brushes.Red;
        }

        if (!(bool)cbKeepFile.IsChecked!)
        {
            /* Delete image testfile */
            File.Delete(imgFile);
        }
    }

    private void ActivateButtonCamera(bool activate)
    {
        if (activate)
        {
            btnCamera.IsEnabled = true;
            txDescCamera.Text = "Take a picture and show it below.";
        }
        else
        {
            btnCamera.IsEnabled = false;
            txDescCamera.Text = "Select Camera first.";
        }
    }

    private void CbCams_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbCams.SelectedItem != null && !string.IsNullOrEmpty(cbCams.SelectedItem.ToString()))
        {
            /* Set Camera */
            busId = ExtractNumber(cbCams.SelectedItem.ToString()!);
            /* Close dropdown */
            cbCams.IsDropDownOpen = false;
            ActivateButtonCamera(true);
        }
        else
        {
            ActivateButtonCamera(false);
        }
    }

    static int ExtractNumber(string input)
    {
        /* cbCams contains strings. VideoDevice needs an int busId 
         * This is in the ComboBox: ../../video4
         * This is what we need: 4 */

        // Find the last occurrence of digits in the input string
        int lastIndex = input.Length - 1;
        while (lastIndex >= 0 && char.IsDigit(input[lastIndex]))
        {
            lastIndex--;
        }

        // Extract the digits from the last occurrence to the end of the string
        string numberString = input.Substring(lastIndex + 1);

        // Parse the extracted digits into an integer
        if (int.TryParse(numberString, out int number))
        {
            return number;
        }

        // Return 0 or throw an exception based on your requirement
        return 0;
    }

    private void GetValuesFromTextBox()
    {
        if (!string.IsNullOrEmpty(tbCamWidth.Text))
            width = Convert.ToUInt32(tbCamWidth.Text);
        else
            tbCamWidth.Text = width.ToString();
        if (!string.IsNullOrEmpty(tbCamHeight.Text))
            height = Convert.ToUInt32(tbCamHeight.Text);
        else
            tbCamHeight.Text = height.ToString();
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnGetCams.AddHandler(Button.ClickEvent, BtnGetCams_Clicked!);
        btnCamera.AddHandler(Button.ClickEvent, BtnCamera_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes*/
        tbCamWidth.Text = Convert.ToString(width);
        tbCamHeight.Text = Convert.ToString(height);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbCamWidth.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbCamHeight.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        txDescGetCams.Text = "Find connected cameras.";
        txInfoGetCams.Text = "";
        txDescCamera.Text = "Select Camera first.";
        txInfoCamera.Text = "";
    }

    private void SetupComboBox()
    {
        cbCams.AddHandler(ComboBox.SelectionChangedEvent, CbCams_SelectionChanged!);
    }
}
