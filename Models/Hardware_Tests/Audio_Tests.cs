using Iot.Device.Media;

namespace IoTLib_Test.Models.Hardware_Tests;

internal class Audio_Tests
{
    private readonly SoundConnectionSettings playbackSettings;
    private readonly SoundConnectionSettings recordingSettings;
    private readonly SoundDevice playbackDevice;
    private readonly SoundDevice recordingDevice;
    private bool inPlaybackLoop;
    private bool isRecording;
    /* Unmute SoundDevice? */
    readonly bool unmute = true;

    public Audio_Tests()
    {
        /* Create Playback Device with standard values */
        playbackSettings = new();
        playbackDevice = SoundDevice.Create(playbackSettings);
        /* Different settings for Recording Device */
        recordingSettings = new()
        {
            //MixerDeviceName = "Capture Mux",
            //RecordingDeviceName = "hw:0,1",
            RecordingSampleRate = 48000,
            RecordingChannels = 2,
            RecordingBitsPerSample = 16
        };
        /* Unmute the recording device on creation */
        recordingDevice = SoundDevice.Create(recordingSettings, unmute);

        //TODO: ALSA Einstellungen aus C# anpassen?
        // alsamixer - capture -> mute, LINE_IN
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

    public bool RecordFixedDuration(string outputFile, uint duration)
    {
        /* Stop if device is already recording */
        if (isRecording)
            return false;

        isRecording = true;
        /* Start recording for defined duration, save as file */
        recordingDevice.Record(duration, outputFile);
        /* false after recording finished */
        isRecording = false;

        return true;
    }
    #endregion
}
