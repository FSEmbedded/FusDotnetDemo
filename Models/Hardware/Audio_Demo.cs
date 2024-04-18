using System.Diagnostics;
using Iot.Device.Media;

namespace dotnetIot_Demo.Models.Hardware;

internal class Audio_Demo
{
    private readonly SoundConnectionSettings playbackSettings;
    private readonly SoundConnectionSettings recordingSettings;
    private readonly SoundDevice playbackDevice;
    private readonly SoundDevice recordingDevice;
    private bool inPlaybackLoop;
    private bool isRecording;
    /* Unmute SoundDevice? */
    readonly bool unmute = true;

    public Audio_Demo()
    {
        /* Create Playback Device with standard values */
        playbackSettings = new();
        playbackDevice = SoundDevice.Create(playbackSettings);
        /* Different settings for Recording Device */
        recordingSettings = new()
        {
            RecordingSampleRate = 48000,
            RecordingChannels = 2,
            RecordingBitsPerSample = 16
        };
        /* Unmute the recording device on creation */
        recordingDevice = SoundDevice.Create(recordingSettings, unmute);
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

    public static void SetAudioInput(string input)
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
}
