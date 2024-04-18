using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using dotnetIot_Demo.Models.Tools;
using dotnetIot_Demo.Models.Hardware;


namespace dotnetIot_Demo.Views;

public partial class UserControl_Camera : UserControl
{
    /* Camera functions are in a separate class */
    private Camera_Demo? Camera;
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

    public void GetValuesFromTextBox()
    {
        if (!string.IsNullOrEmpty(tbCamWidth.Text))
            width = Convert.ToUInt32(tbCamWidth.Text);
        else
            tbCamWidth.Text = width.ToString();
        if (!string.IsNullOrEmpty(tbCamHeight.Text))
            height = Convert.ToUInt32(tbCamHeight.Text);
        else
            tbCamHeight.Text = height.ToString();
        if (!string.IsNullOrEmpty(tbCamBus.Text))
            busId = Convert.ToInt32(tbCamBus.Text);
        else
            tbCamBus.Text = busId.ToString();
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
        txDescCamera.Text = "This test will take a picture and show it below.";
        txInfoCamera.Text = "";
    }
}
