using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Iot.Device.Media;
using System.Collections.Generic;
using Iot.Device.Graphics;
using System.Drawing;
using Iot.Device.Gui;
using Iot.Device.Camera;
using Iot.Device.Camera.Settings;
using Iot.Device.Common;
using System.Text.RegularExpressions;
using System.IO;

namespace IoTLib_Test.Models
{
    internal class Camera_Tests
    {
        private readonly VideoConnectionSettings settings;
        private readonly VideoDevice vidDevice;

        public Camera_Tests(int _busid, uint _width, uint _height)
        {
            try
            {
                settings = new(busId: _busid, captureSize: (_width, _height));
                vidDevice = VideoDevice.Create(settings);
            }
            catch (Exception ex)
            {
                throw new("Exception: " + ex.Message);
            }
        }


        public bool CaptureCam(string imgFile)
        {
            /* Capture static image */
            vidDevice.Capture(imgFile);

            //TODO: Datei wird erzeugt, Inhalt kann nicht angezeigt werden!
            {
                /* Test - innerhalb Klammern kann gelöscht werden */
                byte[] buffer = vidDevice.Capture();


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
}
