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

namespace IoTLib_Test.Models
{
    internal class Video_Tests
    {
        //TODO: Video Tests

        // https://github.com/dotnet/iot/tree/ab3f910a76568d8a0c234aee0227c65705729da8/src/devices/Media

        public bool CaptureCam(string imgFile)
        {
            VideoConnectionSettings settings = new(busId: 1, captureSize: (2560, 1920));
            using VideoDevice device = VideoDevice.Create(settings);

            // Capture static image
            device.Capture(imgFile);
            //TODO: Datei wird erzeugt, Inhalt kann nicht angezeigt werden!
            
            return true;
        }
    }
}
