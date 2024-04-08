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
        private readonly VideoConnectionSettings settings;
        private readonly VideoDevice device;

        public Video_Tests(int _busid, uint _width, uint _height)
        {
            try
            {
                settings = new(busId: _busid, captureSize: (_width, _height));
                device = VideoDevice.Create(settings);
            }
            catch (Exception ex)
            {
                throw new("Exception: " + ex.Message);
            }
        }


        public bool CaptureCam(string imgFile)
        {
            /* Capture static image */
            device.Capture(imgFile);

            //TODO: Datei wird erzeugt, Inhalt kann nicht angezeigt werden!
            
            return true;
        }
    }
}
