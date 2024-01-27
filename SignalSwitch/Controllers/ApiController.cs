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
        private const string ParamSignal = "md";

        [Route("signal")]
        public void Signal(WebServerEventArgs e)
        {
            var parameters = WebServer.DecodeParam(e.Context.Request.RawUrl);
            //check if Params contains anything and is valid
            if ((parameters == null) || (parameters.Length == 0) || Application.Signal == null)
            {
                WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                return;
            }

            int sig = -1;
            foreach (UrlParameter param in parameters)
            {
                if (ParamSignal == param.Name)
                {
                    if (!TryConvertInt32(param.Value, out sig, (int)SignalState.Black, (int)SignalState.Green))
                    {
                        WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                        return;
                    }
                }
            }

            switch ((SignalState)sig)
            {
                case SignalState.Black:
                    Application.Signal.SetBlack();
                    break;
                case SignalState.Red:
                    Application.Signal.SetRed();
                    break;
                case SignalState.Green:
                    Application.Signal.SetGreen();
                    break;
                default:
                    WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                    return;
            }

            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
            return;
        }

        [Route("signalstatus")]
        public void SignalStatus(WebServerEventArgs e)
        {
            WebServer.OutPutStream(e.Context.Response, Application.Signal.State.ToString());
        }

        [Route("switch")]
        public void Switch(WebServerEventArgs e)
        {
            var parameters = WebServer.DecodeParam(e.Context.Request.RawUrl);
            //check if Params contains anything and is valid
            if ((parameters == null) || (parameters.Length == 0) || Application.Switch == null)
            {
                WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                return;
            }

            int sig = -1;
            foreach (UrlParameter param in parameters)
            {
                if (ParamSignal == param.Name)
                {
                    if (!TryConvertInt32(param.Value, out sig, 0, 1))
                    {
                        WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                        return;
                    }
                }
            }

            if (sig == -1)
            {
                WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                return;
            }
            else if (sig == 0)
            {
                Application.Switch.SetStraight();
            }
            else
            {
                Application.Switch.SetTurn();
            }

            WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
        }


        [Route("switchstatus")]
        public void SwitchStatus(WebServerEventArgs e)
        {
            WebServer.OutPutStream(e.Context.Response, Application.Switch.IsStraight ? "0" : "1");
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
