using System;
using System.IO;
using Iot.Device.Media;
using Iot.Device.Camera.Settings;
using Iot.Device.Common;

namespace IoTLib_Test.Models.Hardware_Tests;

internal class Camera_Tests
{
    private readonly VideoConnectionSettings settings;
    private readonly VideoDevice videoDevice;

    public Camera_Tests(int _busid, uint _width, uint _height)
    {
        try
        {
            settings = new(busId: _busid, captureSize: (_width, _height));
            videoDevice = VideoDevice.Create(settings);
        }
        catch (Exception ex)
        {
            throw new("Exception: " + ex.Message);
        }
    }


    public bool CaptureCam(string imgFile)
    {
        /* Capture static image */
        videoDevice.Capture(imgFile);

        //TODO: Datei wird erzeugt, hat keinen Inhalt
        {
            /* Test - innerhalb Klammern kann gelöscht werden */
            byte[] buffer = videoDevice.Capture();


            var processSettings = ProcessSettingsFactory.CreateForLibcamerastill();

            var builder = new CommandOptionsBuilder()
            .WithTimeout(1)
            .WithVflip()
            .WithHflip()
            .WithPictureOptions(90, "jpg")
            .WithResolution(640, 480);
            var args = builder.GetArguments();

            using var proc = new ProcessRunner(processSettings);

            var filename = "test.jpg";
            using var file = File.OpenWrite(filename);
            proc.ExecuteAsync(args, file);



            var process2 = ProcessSettingsFactory.CreateForLibcamerastillAndStderr();
            using var proc2 = new ProcessRunner(process2);
            var text = proc2.ExecuteReadOutputAsStringAsync(string.Empty);
            //IEnumerable<CameraInfo> cameras = CameraInfo.From(text);

        }

        return true;
    }
}
