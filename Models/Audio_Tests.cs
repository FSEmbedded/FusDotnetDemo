using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Iot.Device.Media;
using Avalonia.Metadata;
using Iot.Device.FtCommon;
using System.Transactions;
using System.ComponentModel;

namespace IoTLib_Test.Models
{
    internal class Audio_Tests
    {
        //TODO: Audio Tests

        // https://github.com/dotnet/iot/tree/ab3f910a76568d8a0c234aee0227c65705729da8/src/devices/Media#usage
        // https://www.nuget.org/packages/Alsa.Net
        // https://github.com/omegaframe/alsa.net
        // https://github.com/mobiletechtracker/NetCoreAudio

        // Pins: AUDIO_A_LOUT_L - J11-49, AUDIO_A_LOUT_R - J11-45, GND - J11-47
        // AUDIO_A_MIC - J11-41, GND - J11-42
        // AUDIO_A_LIN_L - J11-48, AUDIO_A_LIN_R - J11-44, GND - J11-46
        
        // include alsa-dev to yocto release

        private bool playAudio;
        private bool recordAudio;

        public void PlayAudio()
        {
            playAudio = true;

            SoundConnectionSettings settings = new SoundConnectionSettings();
            using SoundDevice device = SoundDevice.Create(settings);

            while(playAudio)
            {
                device.Play("IoTLib_Test/Assets/Audio_Test.wav"); //TODO: Pfad vereinfachen, Datei zuverlässig auf Board kopieren
            }
        }

        public void StopAudio()
        {
            playAudio = false;
        }

        public bool RecordAudio(uint recordTime)
        {
            //TODO: ALSA Einstellungen aus C# anpassen?
            recordAudio = true;

            SoundConnectionSettings settings = new SoundConnectionSettings{ RecordingSampleRate = 48000 };
            using SoundDevice device = SoundDevice.Create(settings);

            device.Record(recordTime, "/home/root/record.wav");

            Thread.Sleep(50);

            device.Play("/home/root/record.wav");

            return true;
        }
    }
}
