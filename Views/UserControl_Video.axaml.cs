using System;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models;
using Avalonia.Skia.Helpers;
using Avalonia.Data;
using Iot.Device.Graphics;
using Iot.Device.Gui;
using System.IO;


namespace IoTLib_Test.Views;

public partial class UserControl_Video : UserControl
{
    /* Video functions are in separate class */
    private readonly Video_Tests Video;
    public readonly string imgFile = "/home/root/img_camtest.jpg";

    public UserControl_Video()
    {
        InitializeComponent();
        AddButtonHandlers();
        FillTextBlockWithText();

        Video = new Video_Tests();
    }

    private void BtnCamera_Clicked(object sender, RoutedEventArgs args)
    {
        if (Video.CaptureCam(imgFile))
        {
            //TODO: Bild anzeigen
            txInfoCamera.Text = $"Image was taken, see {imgFile}";
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

    private void FillTextBlockWithText()
    {
        txDescCamera.Text = "Connect Camera to USB-Port";
        txInfoCamera.Text = "";
    }
}
