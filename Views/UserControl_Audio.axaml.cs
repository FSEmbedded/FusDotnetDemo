using System;
using System.IO;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models.Tools;
using IoTLib_Test.Models.Hardware_Tests;

namespace IoTLib_Test.Views;

public partial class UserControl_Audio : UserControl
{
    /* Audio functions are in a separate class */
    private readonly Audio_Tests Audio;
    private bool speakerIsOn = false;
    private bool isRecording = false;
    private uint recDuration = 5; // seconds
    private readonly string audioTestfile = "IoTLib_Test/Assets/Audio_Test.wav"; // comes with this tool
    private readonly string recFileCont = "/home/root/record_continuous.wav"; // is created from software
    private readonly string recFileDur = "/home/root/record_fixedduration.wav"; // is created from software

    public UserControl_Audio()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
        /* Create new object Audio_Tests */
        Audio = new Audio_Tests();
    }

    private void BtnAudioOut_Clicked(object sender, RoutedEventArgs args)
    {
        if (!speakerIsOn)
        {
            /* Create new thread, play sound in Loop until manually stopped */
            Thread audioOutThread = new(() => Audio.PlayInLoop(audioTestfile));
            audioOutThread.Start();
            speakerIsOn = true;
            /* Change UI */
            btnAudioOut.Content = "Stop Audio";
            btnAudioOut.Background = Brushes.Red;
            txInfoAudioOut.Text = "Speaker should play music";
        }
        else
        {
            /* Create new thread, stop sound */
            Thread stopAudioOutThread = new(new ThreadStart(Audio.StopPlayInLoop));
            stopAudioOutThread.Start();
            speakerIsOn = false;
            /* Change UI */
            btnAudioOut.Content = "Play Audio";
            btnAudioOut.Background = Brushes.LightGreen;
            txInfoAudioOut.Text = "Speaker is off";
        }
    }

    private void BtnAudioInCont_Clicked(object sender, RoutedEventArgs args)
    {
        /* Recording until Stop is clicked */
        if (!isRecording)
        {
            /* Start Recording */
            Audio.RecordContinuous(recFileCont);
            isRecording = true;
            /* Change UI */
            btnAudioInCont.Content = "Stop Recording";
            btnAudioInCont.Background = Brushes.Red;
            txInfoAudioInCont.Text = "Device is recording";
        }
        else
        {
            /* Stop Recording */
            if (Audio.StopRecordContinuous())
            {
                isRecording = false;
                /* Change UI */
                btnAudioInCont.Content = "Start Recording";
                btnAudioInCont.Background = Brushes.LightGreen;
                txInfoAudioInCont.Text = "Recording finished";
                /* Play recorded file */
                Audio.PlayAudioFile(recFileCont);

                if (!(bool)cbKeepFileCont.IsChecked!)
                {
                    /* Delete recorded file */
                    File.Delete(recFileCont);
                }
                else
                {
                    txInfoAudioInCont.Text += $"\r\nFile is stored at {recFileCont}";
                }
            }
        }
    }

    private void BtnAudioInDur_Clicked(object sender, RoutedEventArgs args)
    {
        /* Stop if device is already recording */
        if (isRecording)
            return;

        /* Get duration from TextBox */
        GetValuesFromTextBox();

        /* Start recording */
        if (Audio.RecordFixedDuration(recFileDur, recDuration))
        {
            /* Play recorded file */
            Audio.PlayAudioFile(recFileDur);
            txInfoAudioInDur.Text = "Recording success";
        }
        else
        {
            txInfoAudioInDur.Text = "Recording failed";
            return;
        }

        if (!(bool)cbKeepFileDur.IsChecked!)
        {
            /* Delete recorded file */
            File.Delete(recFileDur);
        }
        else
        {
            txInfoAudioInDur.Text += $"\r\nFile is stored at {recFileDur}";
        }
    }

    public void GetValuesFromTextBox()
    {
        if (!string.IsNullOrEmpty(tbAudioInDur.Text))
            recDuration = Convert.ToUInt32(tbAudioInDur.Text);
        else
            tbAudioInDur.Text = recDuration.ToString();
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnAudioOut.AddHandler(Button.ClickEvent, BtnAudioOut_Clicked!);
        btnAudioInCont.AddHandler(Button.ClickEvent, BtnAudioInCont_Clicked!);
        btnAudioInDur.AddHandler(Button.ClickEvent, BtnAudioInDur_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes */
        tbAudioInDur.Text = Convert.ToString(recDuration);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbAudioInDur.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        txDescAudioOut.Text = "Audio file will be played until stopped.";
        txDescAudioInCont.Text = "This test will record audio until stopped.";
        txDescAudioInDur.Text = "This test will record audio for a defined time.";
        txInfoAudioOut.Text = "";
        txInfoAudioInCont.Text = "";
        txInfoAudioInDur.Text = "";
    }
}
