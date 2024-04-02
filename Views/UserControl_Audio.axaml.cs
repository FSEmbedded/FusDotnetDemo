using System;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using IoTLib_Test.Models;
using System.Threading;

namespace IoTLib_Test.Views;

public partial class UserControl_Audio : UserControl
{
    /* Audio functions are in separate class */
    private readonly Audio_Tests Audio;
    private bool speakerIsOn = false;
    private bool recording = false;
    private uint recordTime = 5;

    public UserControl_Audio()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();

        Audio = new Audio_Tests();
    }

    void BtnAudioOut_Clicked(object sender, RoutedEventArgs args)
    {
        if (!speakerIsOn)
        {
            /* Create new thread, play sound */
            Thread audioOutThread = new(() => Audio.PlayAudio());
            audioOutThread.Start();
            speakerIsOn = true;
            /* Change UI */
            btnAudioOut.Content = "Stop Audio";
            btnAudioOut.Background = Brushes.Red;
            txInfoOut.Text = "Speaker should play music";
        }
        else
        {
            /* Create new thread, stop sound */
            Thread stopAudioOutThread = new(new ThreadStart(Audio.StopAudio));
            stopAudioOutThread.Start();
            speakerIsOn = false;
            /* Change UI */
            btnAudioOut.Content = "Play Audio";
            btnAudioOut.Background = Brushes.LightGreen;
            txInfoOut.Text = "Speaker is off";
        }
    }

    void BtnAudioIn_Clicked(object sender, RoutedEventArgs args)
    {
        recordTime = Convert.ToUInt32(tbAudioInTime.Text);
        Audio.RecordAudio(recordTime);
        //bool recordingSuccess = false;

        //if (!recording)
        //{
        //    /* Create new thread, light up LED */
        //    Thread recordThread = new(() => recordingSuccess = Audio.RecordAudio(recordTime));
        //    recordThread.Start();
        //    //recordThread.Join();
        //    recording = true;
        //    /* Change UI */
        //    btnAudioIn.Content = "Recording";
        //    btnAudioIn.Background = Brushes.Red;
        //    txInfoOut.Text = "Speaker should play music after recording finished";
        //}
        //if (recordingSuccess == true)
        //{
        //    ///* Create new thread, turn off LED */
        //    //Thread stopRecordThread = new(new ThreadStart(Audio.StopRecord));
        //    //stopRecordThread.Start();
        //    //Audio.StopRecord();
        //    recording = false;
        //    /* Change UI */
        //    btnAudioIn.Content = "Record Audio";
        //    btnAudioIn.Background = Brushes.LightGreen;
        //    txInfoOut.Text = "Input is off";
        //}
    }

    void AddButtonHandlers()
    {
        /* Button bindings */
        btnAudioOut.AddHandler(Button.ClickEvent, BtnAudioOut_Clicked!);
        btnAudioIn.AddHandler(Button.ClickEvent, BtnAudioIn_Clicked!);
    }

    void WriteStandardValuesInTextBox()
    {
        //TODO

        ///* Write standard GPIO pins in textboxes */
        //tbAudioOutDev.Text = Convert.ToString(gpioNoLed);
        //tbAudioOutEmpty.Text = Convert.ToString(gpioNoInput);
        //tbAudioInDev.Text = Convert.ToString();
        //tbAudioInEmpty.Text = Convert.ToString();
        tbAudioInTime.Text = Convert.ToString(recordTime);
    }

    void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbAudioInTime.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    void FillTextBlockWithText()
    {
        txDescAudioOut.Text = "Connect Speaker to PcoreBBDSI Rev1.40 - AUDIO_A_LOUT_L - J11-49, AUDIO_A_LOUT_R - J11-45, GND - J11-47";
        txDescAudioIn.Text = "Connect Button to PcoreBBDSI Rev1.40 - AUDIO_A_LIN_L - J11-48, AUDIO_A_LIN_R - J11-44, GND - J11-46";
        txInfoOut.Text = "";
        txInfoIn.Text = "";
    }
}
//TODO