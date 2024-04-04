using System;
using System.IO;
using System.Threading;
using Iot.Device.Media;

namespace IoTLib_Test.Models
{
    internal class Audio_Tests
    {
        //TODO: Audio Tests
        // include alsa-dev to yocto release

        private bool playAudio;

        public void PlayAudio(string audio_testfile)
        {
            playAudio = true;

            SoundConnectionSettings settings = new();
            using SoundDevice device = SoundDevice.Create(settings);

            while (playAudio)
            {
                device.Play(audio_testfile);
            }            
        }

        public void StopAudio()
        {
            playAudio = false;
        }

        public bool RecordAudio(string audio_recording, uint recordTime)
        {
            //TODO: ALSA Einstellungen aus C# anpassen?
            // alsamixer - capture -> mute, LINE_IN

            /* Unmute SoundDevice? */
            bool unmute = true;

            /* Define settings for recording */
            SoundConnectionSettings settings = new()
            {
                RecordingSampleRate = 48000,
                RecordingChannels = 2,
                RecordingBitsPerSample = 16
            };

            /* Create SoundDevice with defined settings, unmute */
            using SoundDevice device = SoundDevice.Create(settings, unmute);

            /* Start recording for defined time, save as file */
            device.Record(recordTime, audio_recording);
            Thread.Sleep(50);

            /* Playback of the recording */
            device.Play(audio_recording);

            return true;
        }

        public void AudioPassThrough(uint recordTime)
        {
            /* Unmute SoundDevice? */
            bool unmute = true;

            using Stream stream = new MemoryStream();

            /* Define settings for recording */
            SoundConnectionSettings settings = new()
            {
                //RecordingSampleRate = 48000,
                //RecordingChannels = 2,
                RecordingBitsPerSample = 16
            };

            /* Create SoundDevice with defined settings, unmute */
            using SoundDevice recDevice = SoundDevice.Create(settings, unmute);
            //Thread recordThread = new(() => recDevice.Record(recordTime, stream));

            using SoundDevice playDevice = SoundDevice.Create(settings);
            //Thread playThread = new(() => playDevice.Play(stream));

            //recordThread.Start();
            //playThread.Start();

            recDevice.Record(recordTime, stream);
            playDevice.Play(stream);
        }
    }
}
