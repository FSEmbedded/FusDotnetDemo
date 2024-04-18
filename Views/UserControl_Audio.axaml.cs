using System;
using System.IO;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using dotnetIot_Demo.Models.Tools;
using dotnetIot_Demo.Models.Hardware;

namespace dotnetIot_Demo.Views;

public partial class UserControl_Audio : UserControl
{
    /* Audio functions are in a separate class */
    private readonly Audio_Demo Audio;
    private bool speakerIsOn = false;
    private bool isRecording = false;
    private uint recDuration = 5; // seconds
    private readonly string audioTestfile = "IoTLib_Test/Assets/Audio_Test.wav"; // comes with this tool
    private readonly string recFileCont = "/home/root/record_continuous.wav"; // is created from software
    private readonly string recFileDur = "/home/root/record_fixedduration.wav"; // is created from software

    private string inputSignal = "LINE_IN";

    public UserControl_Audio()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
        /* Create new object Audio_Tests */
        Audio = new Audio_Demo();
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
            txInfoAudioOut.Text = "Speaker plays music";
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
            /* Get selected Input Signal, set alsamixer to this input */
            inputSignal = GetSelectedInputSignal(0);
            Audio_Demo.SetAudioInput(inputSignal);

            /* Start Recording */
            Audio.RecordContinuous(recFileCont);
            isRecording = true;
            /* Change UI */
            btnAudioInCont.Content = "Stop Recording";
            btnAudioInCont.Background = Brushes.Red;
            txInfoAudioInCont.Text = "Device is recording";
            btnAudioInTime.IsEnabled = false;
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
                btnAudioInTime.IsEnabled = true;
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

        /* Get selected Input Signal, set alsamixer to this input */
        inputSignal = GetSelectedInputSignal(1);
        Audio_Demo.SetAudioInput(inputSignal);

        /* Start recording */
        if (Audio.RecordFixedTime(recFileDur, recDuration))
        {
            /* Play recorded file */
            Audio.PlayAudioFile(recFileDur);
            txInfoAudioInTime.Text = "Recording success";
        }
        else
        {
            txInfoAudioInTime.Text = "Recording failed";
            return;
        }

        if (!(bool)cbKeepFileTime.IsChecked!)
        {
            /* Delete recorded file */
            File.Delete(recFileDur);
        }
        else
        {
            txInfoAudioInTime.Text += $"\r\nFile is stored at {recFileDur}";
        }
    }

    private string GetSelectedInputSignal(int caller)
    {
        string signal = "";

        switch (caller)
        {
            /* BtnAudioInCont_Clicked */
            case 0:
                if (rbSigContLineIn.IsChecked == true)
                    signal = "LINE_IN";
                else
                    signal = "MIC_IN";
                break;
            /* BtnAudioInDur_Clicked */
            case 1:
                if (rbSigTimeLineIn.IsChecked == true)
                    signal = "LINE_IN";
                else
                    signal = "MIC_IN";
                break;
        }
        return signal;
    }

    public void GetValuesFromTextBox()
    {
        if (!string.IsNullOrEmpty(tbAudioInTime.Text))
            recDuration = Convert.ToUInt32(tbAudioInTime.Text);
        else
            tbAudioInTime.Text = recDuration.ToString();
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnAudioOut.AddHandler(Button.ClickEvent, BtnAudioOut_Clicked!);
        btnAudioInCont.AddHandler(Button.ClickEvent, BtnAudioInCont_Clicked!);
        btnAudioInTime.AddHandler(Button.ClickEvent, BtnAudioInDur_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes */
        tbAudioInTime.Text = Convert.ToString(recDuration);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbAudioInTime.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        txDescAudioOut.Text = "Audio file will be played until stopped.";
        txInfoAudioOut.Text = "";
        txDescAudioInCont.Text = "This test will record audio until stopped. Recorded audio will then be played.";
        txInfoAudioInCont.Text = "";
        txDescAudioInTime.Text = "This test will record audio for a defined time. Recorded audio will then be played.";
        txInfoAudioInTime.Text = "";
    }
}
