// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using LegoElement.Services;
using nanoFramework.WebServer;
using System.Net;

namespace LegoElement.Controllers
{
    internal class ApiController
    {
        public const string PageCombo = "combo";
        public const string PageSinglePwm = "singlepwm";
        public const string PageContinuous = "continuous";
        public const string PageSingleCst = "singlecst";
        public const string PageTimeout = "timeout";
        public const string PageComboAll = "comboall";
        public const string PageContinuousAll = "continuousall";
        public const string PageSinglePwmAll = "singlepwmall";
        public const string PageComboPwm = "combopwm";
        public const string PageComboPwmAll = "combopwmall";

        [Route("api/" + PageCombo)]
        public void Combo(WebServerEventArgs e)
        {
            // http://192.168.1.85/combo?rd=0&bl=0&ch=0
            var ret = LegoInfraredExecute.Combo(e.Context.Request.RawUrl);

            WebServer.OutputHttpCode(e.Context.Response, ret ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }

        [Route("api/" + PageSinglePwm)]
        public void SinglePwm(WebServerEventArgs e)
        {
            // http://192.168.1.85/singlepwm?pw=0&op=0&ch=0
            var ret = LegoInfraredExecute.SinglePwm(e.Context.Request.RawUrl);

            WebServer.OutputHttpCode(e.Context.Response, ret ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }

        [Route("api/" + PageContinuous)]
        public void Continuous(WebServerEventArgs e)
        {
            // http://192.168.1.85/continuous?fc=0&op=0
            var ret = LegoInfraredExecute.Continuous(e.Context.Request.RawUrl);

            WebServer.OutputHttpCode(e.Context.Response, ret ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }

        [Route("api/" + PageSingleCst)]
        public void SingleCst(WebServerEventArgs e)
        {
            // http://192.168.1.85/singlecst?pw=0&op=0&ch=0
            var ret = LegoInfraredExecute.SingleCst(e.Context.Request.RawUrl);

            WebServer.OutputHttpCode(e.Context.Response, ret ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }

        [Route("api/" + PageTimeout)]
        public void Timeout(WebServerEventArgs e)
        {
            // http://192.168.1.85/timeout?fc=0&op=0&ch=0
            var ret = LegoInfraredExecute.SingleTimeout(e.Context.Request.RawUrl);

            WebServer.OutputHttpCode(e.Context.Response, ret ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }

        [Route("api/" + PageComboAll)]
        public void ComboAll(WebServerEventArgs e)
        {
            // http://192.168.1.85/comboall?rd0=0&bl0=0&rd1=0&bl1=0&rd2=0&bl2=0&rd3=0&bl3=0
            var ret = LegoInfraredExecute.ComboAll(e.Context.Request.RawUrl);

            WebServer.OutputHttpCode(e.Context.Response, ret ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }

        [Route("api/" + PageContinuousAll)]
        public void ContinuousAll(WebServerEventArgs e)
        {
            // http://192.168.1.85/continuousall?fc0=0&op0=0&fc1=0&op1=0&fc2=0&op2=0&fc3=0&op3=0
            var ret = LegoInfraredExecute.ContinuousAll(e.Context.Request.RawUrl);

            WebServer.OutputHttpCode(e.Context.Response, ret ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }

        [Route("api/" + PageSinglePwmAll)]
        public void SinglePwmAll(WebServerEventArgs e)
        {
            // http://192.168.1.85/singlepwmall?pw0=0&op0=0&pw1=0&op1=0&pw2=0&op2=0&pw3=0&op3=0
            var ret = LegoInfraredExecute.SinglePwmAll(e.Context.Request.RawUrl);

            WebServer.OutputHttpCode(e.Context.Response, ret ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        }
    }
}
