// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.WebServer;
using LegoElement.Models;
using System.Device.Gpio;
using System.Net;

namespace LegoElement.Controllers
{
    internal class ApiController
    {
        [Route("detectstatus")]
        public void DectectStatus(WebServerEventArgs e)
        {
            var resp = string.Empty;
            if (Application.Detectors[0] != null && Application.Detectors[1] != null)
            {
                resp = Application.Detectors[0].Value.ToString() + ";" + Application.Detectors[1].Value.ToString();
            }
            else if (Application.Detectors[0] != null)
            {
                resp = Application.Detectors[0].Value.ToString();
            }
            else if (Application.Detectors[1] != null)
            {
                resp = Application.Detectors[1].Value.ToString();
            }

            WebServer.OutPutStream(e.Context.Response, resp);
        }

        [Route("detectstart")]
        public void StartDetection(WebServerEventArgs e)
        {
            if (Application.Detectors[0] != null)
            {
                Application.Detectors[0].Detect = true;
            }

            if (Application.Detectors[1] != null)
            {
                Application.Detectors[1].Detect = true;
            }

            WebServer.OutPutStream(e.Context.Response, "Detection started");
        }

        private static bool TryConvertInt32(string val, out int result, int min, int max)
        {
            if (!int.TryParse(val, out result))
            {
                return false;
            }

            if (!((result >= min) && (result <= max)))
            {
                return false;
            }

            return true;
        }
    }
}
