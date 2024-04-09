using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models.Tools;
using IoTLib_Test.Models.Hardware_Tests;


namespace IoTLib_Test.Views;

public partial class UserControl_Camera : UserControl
{
    /* Camera functions are in separate class */
    private Camera_Tests? Camera;
    private readonly string imgFile = "/home/root/img_camtest.png";
    private int busId = 1;
    private uint width = 1920;
    private uint height = 1080;

    public UserControl_Camera()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
    }

    private void BtnCamera_Clicked(object sender, RoutedEventArgs args)
    {
        /* Get Camera Bus ID and resolution from TextBoxes */
        if (!string.IsNullOrEmpty(tbCamWidth.Text))
            width = Convert.ToUInt32(tbCamWidth.Text);
        if (!string.IsNullOrEmpty(tbCamHeight.Text))
            height = Convert.ToUInt32(tbCamHeight.Text);
        if (!string.IsNullOrEmpty(tbCamBus.Text))
            busId = Convert.ToInt32(tbCamBus.Text);

        try
        {
            /* Video tests are in separate class */
            Camera = new Camera_Tests(busId, width, height);
        }
        catch (Exception ex)
        {
            txInfoCamera.Text = ex.Message;
            txInfoCamera.Foreground = Brushes.Red;
            return;
        }

        if (Camera.CaptureCam(imgFile))
        {
            /* Show image in UI */
            try
            {
                Avalonia.Media.Imaging.Bitmap bitmap = new(imgFile);
                imgCamCapture.Source = bitmap;
                txInfoCamera.Text = $"Image was taken, see {imgFile}";
                txInfoCamera.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                txInfoCamera.Text = $"Error showing image \"{imgFile}\":\r\n{ex.Message}";
                txInfoCamera.Foreground = Brushes.Red;
            }
        }

        if (!(bool)cbKeepFile.IsChecked!)
        {
            /* Delete image testfile */
            File.Delete(imgFile);
        }
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnCamera.AddHandler(Button.ClickEvent, BtnCamera_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes*/
        tbCamWidth.Text = Convert.ToString(width);
        tbCamHeight.Text = Convert.ToString(height);
        tbCamBus.Text = Convert.ToString(busId);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbCamWidth.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbCamHeight.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
        tbCamBus.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        txDescCamera.Text = "Connect Camera to USB-Port";
        txInfoCamera.Text = "";
    }
}
