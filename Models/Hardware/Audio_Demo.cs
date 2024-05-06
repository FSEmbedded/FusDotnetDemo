using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Iot.Device.Media;

namespace dotnetIot_Demo.Models.Hardware;

internal partial class Audio_Demo
{
    private readonly SoundConnectionSettings playbackSettings;
    private readonly SoundConnectionSettings recordingSettings;
    private readonly SoundDevice playbackDevice;
    private readonly SoundDevice recordingDevice;
    private bool inPlaybackLoop;
    private bool isRecording;
    /* Unmute SoundDevice? */
    readonly bool unmute = true;

    public Audio_Demo(string _playbackDeviceName)
    {
        try
        {
            /* Create Playback Device with standard values, except device name */
            playbackSettings = new()
            {
                PlaybackDeviceName = _playbackDeviceName
            };

            playbackDevice = SoundDevice.Create(playbackSettings, unmute);

            /* Create empty recording device as it is not needed */
            recordingSettings = new();
            recordingDevice = SoundDevice.Create(recordingSettings);
        }
        catch (Exception ex)
        {
            throw new("Exception: " + ex.Message);
        }
    }

    public Audio_Demo(string _playbackDeviceName, string _recordingDeviceName)
    {
        try
        {
            /* Create Playback Device with standard values, except device name */
            playbackSettings = new()
            {
                PlaybackDeviceName = _playbackDeviceName
            };

            playbackDevice = SoundDevice.Create(playbackSettings, unmute);

            /* Different settings for Recording Device */
            recordingSettings = new()
            {
                RecordingDeviceName = _recordingDeviceName,
                RecordingSampleRate = 48000,
                RecordingChannels = 2,
                RecordingBitsPerSample = 16
            };

            /* Unmute the recording device on creation */
            recordingDevice = SoundDevice.Create(recordingSettings, unmute);
        }
        catch (Exception ex)
        {
            throw new("Exception: " + ex.Message);
        }
    }

    #region Playback
    public void PlayAudioFile(string audioFile)
    {
        /* Play .wav audiofile */
        playbackDevice.Play(audioFile);
    }

    public void PlayInLoop(string audiofile)
    {
        inPlaybackLoop = true;
        /* Play .wav audiofile until stopped */
        while (inPlaybackLoop)
        {
            PlayAudioFile(audiofile);
        }
    }

    public void StopPlayInLoop()
    {
        /* End while-loop */
        inPlaybackLoop = false;
    }
    #endregion
    #region Recording
    public void RecordContinuous(string outputFile)
    {
        /* Stop if device is already recording */
        if (isRecording)
            return;

        /* Records until StopRecording() is called, save to audioFile */
        recordingDevice.StartRecording(outputFile);
        isRecording = true;
    }

    public bool StopRecordContinuous()
    {
        /* Stop continuous recording */
        recordingDevice.StopRecording();
        isRecording = false;
        return true;
    }

    public bool RecordFixedTime(string outputFile, uint time)
    {
        /* Stop if device is already recording */
        if (isRecording)
            return false;

        isRecording = true;
        /* Start recording for defined duration, save as file */
        recordingDevice.Record(time, outputFile);
        /* false after recording finished */
        isRecording = false;

        return true;
    }

    public static void SetAudioInputSignal(string input)
    {
        /* Set 'Capture Mux' to the select input signal (LINE_IN / MIC_IN) */
        string argument = $"-c \"amixer -c 0 set 'Capture Mux' {input}\"";

        ProcessStartInfo startInfo = new()
        {
            FileName = "/bin/bash",
            Arguments = argument,
        };

        using Process process = Process.Start(startInfo)!;
        process.WaitForExit();
    }
    #endregion

    #region GetAudioDevices
    [GeneratedRegex(@"card (\d+): .*?device (\d+): (.*?) \[(.*?)\]")]
    private static partial Regex MyRegex();

    public static List<string[]> GetPlaybackDevices()
    {
        List<string[]> playbackDevices = [];

        /* Start the process to run the aplay -l command */
        string argument = $"-c \"aplay -l\"";

        ProcessStartInfo startInfo = new()
        {
            FileName = "/bin/bash",
            Arguments = argument,
            RedirectStandardOutput = true,
        };
        using Process process = Process.Start(startInfo)!;
        
        /* Read the output of the command */
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        /* Regular expression to match device names and their corresponding linux device names */
        Regex regex = MyRegex();
        MatchCollection matches = regex.Matches(output);

        foreach (Match match in matches.Cast<Match>())
        {
            /* Extract the device name and the corresponding linux device name */
            string deviceName = match.Groups[3].Value;
            string linuxDeviceName = $"plughw:{match.Groups[1].Value},{match.Groups[2].Value}";
            /* Add the device name and the internal device name to the list */
            playbackDevices.Add([deviceName, linuxDeviceName]);
        }
        return playbackDevices;
    }

    public static List<string[]> GetRecordingDevices()
    {
        List<string[]> playbackDevices = [];

        /* Start the process to run the aplay -l command */
        string argument = $"-c \"arecord -l\"";

        ProcessStartInfo startInfo = new()
        {
            FileName = "/bin/bash",
            Arguments = argument,
            RedirectStandardOutput = true,
        };
        using Process process = Process.Start(startInfo)!;

        /* Read the output of the command */
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        /* Regular expression to match device names and their corresponding linux device names */
        Regex regex = MyRegex();
        MatchCollection matches = regex.Matches(output);

        foreach (Match match in matches.Cast<Match>())
        {
            /* Extract the device name and the corresponding linux device name */
            string deviceName = match.Groups[3].Value;
            string linuxDeviceName = $"plughw:{match.Groups[1].Value},{match.Groups[2].Value}";
            /* Add the device name and the internal device name to the list */
            playbackDevices.Add([deviceName, linuxDeviceName]);
        }
        return playbackDevices;
    }
    #endregion
}
