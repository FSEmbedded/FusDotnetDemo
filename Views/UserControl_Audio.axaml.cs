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
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using FusDotnetDemo.Models.Tools;
using FusDotnetDemo.Models.Hardware;

namespace FusDotnetDemo.Views;

public partial class UserControl_Audio : UserControl
{
    /* Audio functions are in a separate class */
    private Audio_Demo? Audio;

    private readonly string FilePathAudioTestfile = AppContext.BaseDirectory + "Assets/Audio_Test.wav";

    List<string[]>? PlaybackDevices;
    List<string[]>? RecordingDevices;

    private bool speakerIsOn = false;
    private bool isRecording = false;

    /* Default values from boardvalues.json */
    private uint RecordingTime = DefaultValues.AudioRecordingTime; // seconds
    private string InputSignal = DefaultValues.AudioInputSignal;
    private string? PlaybackDevice = DefaultValues.AudioPlaybackDevice;
    private string? RecordingDevice = DefaultValues.AudioRecordingDevice;
    private readonly string FilePathRecordingContinuous = DefaultValues.AudioFilePathRecordingContinuous;
    private readonly string FilePathRecordingTimed = DefaultValues.AudioFilePathRecordingTimed;

    public UserControl_Audio()
    {
        InitializeComponent();
        AddButtonHandlers();
        AddTextBoxHandlers();
        SetupComboBox();
        WriteStandardValuesInTextBox();
        FillTextBlockWithText();
        ActivateButtonPlayback(false);
        ActivateButtonRecording(false);
    }

    private void BtnGetAudioDev_Clicked(object sender, RoutedEventArgs args)
    {
        /* Clear ComboBoxes */
        cbPlaybackDev.Items.Clear();
        cbRecordDev.Items.Clear();

        /* Find all audio devices */
        PlaybackDevices = Audio_Demo.GetPlaybackDevices();
        RecordingDevices = Audio_Demo.GetRecordingDevices();

        /* Add audio devices to ComboBoxes */
        foreach (string[] device in PlaybackDevices)
        {
            cbPlaybackDev.Items.Add(device[0]);
            /* Select default item in list (marked with (*) */
            if (device[0].EndsWith("(*)"))
            {
                cbPlaybackDev.SelectedItem = device[0];
            }
        }
        foreach (string[] device in RecordingDevices)
        {
            cbRecordDev.Items.Add(device[0]);
            /* Select default item in list (marked with (*) */
            if (device[0].EndsWith("(*)"))
            {
                cbRecordDev.SelectedItem = device[0];
            }
        }
    }

    private void BtnAudioOut_Clicked(object sender, RoutedEventArgs args)
    {
        if (!speakerIsOn)
        {
            try
            {
                /* Create new object Audio_Demo */
                Audio = new Audio_Demo(PlaybackDevice!);
            }
            catch(Exception ex)
            {
                txInfoAudioOut.Text = ex.Message;
                txInfoAudioOut.Foreground = Brushes.Red;
                return;
            }
            /* Create new thread, play sound in Loop until manually stopped */
            Thread audioOutThread = new(() => Audio.PlayInLoop(FilePathAudioTestfile));
            audioOutThread.Start();
            speakerIsOn = true;
            /* Change UI */
            btnAudioOut.Content = "Stop Audio";
            btnAudioOut.Background = Brushes.Red;
            txInfoAudioOut.Text = "Speaker plays music";
            txInfoAudioOut.Foreground = Brushes.Blue;
        }
        else
        {
            /* Create new thread, stop sound */
            Thread stopAudioOutThread = new(new ThreadStart(Audio!.StopPlayInLoop));
            stopAudioOutThread.Start();
            Thread.Sleep(1500);
            speakerIsOn = false;
            /* Change UI */
            btnAudioOut.Content = "Play Audio";
            btnAudioOut.Background = Brushes.LightGreen;
            txInfoAudioOut.Text = "Speaker is off";
            txInfoAudioOut.Foreground = Brushes.Blue;
        }
    }

    private void BtnAudioInCont_Clicked(object sender, RoutedEventArgs args)
    {
        /* Recording until Stop is clicked */
        if (!isRecording)
        {
            try
            {
                /* Create new object Audio_Demo */
                Audio = new Audio_Demo(PlaybackDevice!, RecordingDevice!);
            }
            catch (Exception ex)
            {
                txInfoAudioInCont.Text = ex.Message;
                txInfoAudioInCont.Foreground = Brushes.Red;
                return;
            }

            /* Get selected Input Signal, set alsamixer to this input */
            InputSignal = GetSelectedInputSignal(0);
            Audio_Demo.SetAudioInputSignal(InputSignal);

            /* Start Recording */
            Audio.RecordContinuous(FilePathRecordingContinuous);
            isRecording = true;
            /* Change UI */
            btnAudioInCont.Content = "Stop Recording";
            btnAudioInCont.Background = Brushes.Red;
            txInfoAudioInCont.Text = "Device is recording";
            txInfoAudioInCont.Foreground = Brushes.Blue;
            btnAudioInTime.IsEnabled = false;
        }
        else
        {
            /* Stop Recording */
            if (Audio!.StopRecordContinuous())
            {
                isRecording = false;
                /* Change UI */
                btnAudioInCont.Content = "Start Recording";
                btnAudioInCont.Background = Brushes.LightGreen;
                txInfoAudioInCont.Text = "Recording finished";
                txInfoAudioInCont.Foreground = Brushes.Blue;
                btnAudioInTime.IsEnabled = true;
                /* Play recorded file */
                Audio.PlayAudioFile(FilePathRecordingContinuous);

                if (!(bool)cbKeepFileCont.IsChecked!)
                {
                    /* Delete recorded file */
                    File.Delete(FilePathRecordingContinuous);
                }
                else
                {
                    txInfoAudioInCont.Text += $"\r\nFile is stored at {FilePathRecordingContinuous}";
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
        InputSignal = GetSelectedInputSignal(1);
        Audio_Demo.SetAudioInputSignal(InputSignal);

        try
        {
            /* Create new object Audio_Demo */
            Audio = new Audio_Demo(PlaybackDevice!, RecordingDevice!);
        }
        catch (Exception ex)
        {
            txInfoAudioInTime.Text = ex.Message;
            txInfoAudioInTime.Foreground = Brushes.Red;
            return;
        }

        /* Start recording */
        if (Audio.RecordFixedTime(FilePathRecordingTimed, RecordingTime))
        {
            /* Play recorded file */
            Audio.PlayAudioFile(FilePathRecordingTimed);
            txInfoAudioInTime.Text = "Recording success";
            txInfoAudioInTime.Foreground = Brushes.Blue;
        }
        else
        {
            txInfoAudioInTime.Text = "Recording failed";
            txInfoAudioInTime.Foreground = Brushes.Red;
            return;
        }

        if (!(bool)cbKeepFileTime.IsChecked!)
        {
            /* Delete recorded file */
            File.Delete(FilePathRecordingTimed);
        }
        else
        {
            txInfoAudioInTime.Text += $"\r\nFile is stored at {FilePathRecordingTimed}";
        }
    }

    private void CbPlaybackDev_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbPlaybackDev.SelectedItem != null && !string.IsNullOrEmpty(cbPlaybackDev.SelectedItem.ToString()))
        {
            /* Set Playback Device, stored in playbackDevices field 1 */
            PlaybackDevice = PlaybackDevices!
                .Where(item => item.Length > 0 && item[0] == cbPlaybackDev.SelectedItem.ToString())
                .Select(item => item.Length > 1 ? item[1] : null)
                .FirstOrDefault();

            /* Close dropdown */
            cbPlaybackDev.IsDropDownOpen = false;
            ActivateButtonPlayback(true);
        }
        else
        {
            ActivateButtonPlayback(false);
        }
    }

    private void CbRecordDev_SelectionChanged(object sender, RoutedEventArgs args)
    {
        if (cbRecordDev.SelectedItem != null && !string.IsNullOrEmpty(cbRecordDev.SelectedItem.ToString()))
        {
            /* Set Recording Device, stored in recordingDevices field 1 */
            RecordingDevice = RecordingDevices!
                .Where(item => item.Length > 0 && item[0] == cbRecordDev.SelectedItem.ToString())
                .Select(item => item.Length > 1 ? item[1] : null)
                .FirstOrDefault();

            /* Close dropdown */
            cbRecordDev.IsDropDownOpen = false;
            ActivateButtonRecording(true);
        }
        else
        {
            ActivateButtonRecording(false);
        }
    }

    private void ActivateButtonPlayback(bool activate)
    {
        if (activate)
        {
            btnAudioOut.IsEnabled = true;
            txDescAudioOut.Text = "Audio file will be played until stopped. If the speaker is connected to lineout, try unmuting using 'alsamixer'.";
        }
        else
        {
            btnAudioOut.IsEnabled = false;
            txDescAudioOut.Text = "Select Playback Device first.";
        }
    }

    private void ActivateButtonRecording(bool activate)
    {
        if (activate)
        {
            btnAudioInCont.IsEnabled = true;
            txDescAudioInCont.Text = "This test will record audio until stopped. Recorded audio will then be played.";
            btnAudioInTime.IsEnabled = true;
            txDescAudioInTime.Text = "This test will record audio for a defined time. Recorded audio will then be played.";
        }
        else
        {
            btnAudioInCont.IsEnabled = false;
            txDescAudioInCont.Text = "Select Recording Device first.";
            btnAudioInTime.IsEnabled = false;
            txDescAudioInTime.Text = "Select Recording Device first.";
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

    private void GetValuesFromTextBox()
    {
        if (!string.IsNullOrEmpty(tbAudioInTime.Text))
            RecordingTime = Convert.ToUInt32(tbAudioInTime.Text);
        else
            tbAudioInTime.Text = RecordingTime.ToString();
    }

    private void AddButtonHandlers()
    {
        /* Button bindings */
        btnGetAudioDev.AddHandler(Button.ClickEvent, BtnGetAudioDev_Clicked!);
        btnAudioOut.AddHandler(Button.ClickEvent, BtnAudioOut_Clicked!);
        btnAudioInCont.AddHandler(Button.ClickEvent, BtnAudioInCont_Clicked!);
        btnAudioInTime.AddHandler(Button.ClickEvent, BtnAudioInDur_Clicked!);
    }

    private void WriteStandardValuesInTextBox()
    {
        /* Write standard values in textboxes */
        tbAudioInTime.Text = Convert.ToString(RecordingTime);
    }

    private void AddTextBoxHandlers()
    {
        /* Handler to only allow decimal value inputs */
        tbAudioInTime.AddHandler(KeyDownEvent, InputControl.TextBox_DecimalInput!, RoutingStrategies.Tunnel);
    }

    private void FillTextBlockWithText()
    {
        txDescGetAudioDev.Text = "Find playback and recording audio devices. Devices marked with (*) are the Linux default.";
        txInfoGetAudioDev.Text = "";
        txDescAudioOut.Text = "Select Playback Device first.";
        txInfoAudioOut.Text = "";
        txDescAudioInCont.Text = "Select Recording Device first.";
        txInfoAudioInCont.Text = "";
        txDescAudioInTime.Text = "Select Recording Device first.";
        txInfoAudioInTime.Text = "";
    }

    private void SetupComboBox()
    {
        cbPlaybackDev.AddHandler(ComboBox.SelectionChangedEvent, CbPlaybackDev_SelectionChanged!);
        cbRecordDev.AddHandler(ComboBox.SelectionChangedEvent, CbRecordDev_SelectionChanged!);
    }
}
