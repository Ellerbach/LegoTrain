// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.Runtime.Native;
using nanoFramework.WebServer;
using System;
using System.Text;
using System.Threading;
using System.Web;

namespace SharedServices.Services
{
    public static  class WebServerCommon
    {
        public static void SetupWifi(WebServerEventArgs e)
        {
            if (e.Context.Request.HttpMethod == "GET")
            {
                string route = $"<!DOCTYPE html><html<head><title>Configuration Page</title><link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\"></head><body>" +
                    "<h1>NanoFramework</h1>" +
                    "<form method='POST'  action='/'>" +
                    "<fieldset><legend>Wireless configuration</legend>" +
                    "Ssid:</br><input type='input' name='ssid' value='' ></br>" +
                    "Password:</br><input type='password' name='password' value='' >" +
                    "<br><br>" +
                    "<input type='submit' value='Save'>" +
                    "</fieldset>" +
                    "</form></body></html>";
                WebServer.OutPutStream(e.Context.Response, route);
            }
            else
            {
                byte[] buff = new byte[e.Context.Request.ContentLength64];
                e.Context.Request.InputStream.Read(buff, 0, buff.Length);
                string paramString = Encoding.UTF8.GetString(buff, 0, buff.Length);

                // We're adding back the question mark as it's not present when posting
                var parameters = WebServer.DecodeParam($"{WebServer.ParamStart}{paramString}");
                string ssid = string.Empty;
                string password = string.Empty;
                foreach (UrlParameter param in parameters)
                {
                    if (param.Name == "ssid")
                    {
                        ssid = HttpUtility.UrlDecode(param.Value);
                    }
                    else if (param.Name == "password")
                    {
                        password = HttpUtility.UrlDecode(param.Value);
                    }
                }

                Console.WriteLine($"SSID: {ssid}, password: {password}");

                // Enable the Wireless station interface
                bool res = Wireless80211.Configure(ssid, password);

                var route = $"<!DOCTYPE html><html><body>" +
                    "<h1>NanoFramework</h1>" +
                    "<p>New settings saved.</p><p>Rebooting device to put into normal mode.</p>" +
                    "<p>Please allow up to 10 seconds to reconnect to the IP address.</p>";
                if (res)
                {
                    route += $"<p>IP Address shoud be <a href='http://{Wireless80211.GetCurrentIPAddress()}'>http://{Wireless80211.GetCurrentIPAddress()}</a>.</p>";
                }

                route += $"<p>If not configured properly, connect again to the SSID {WirelessAP.SoftApSsid} and then to <a href='http://{WirelessAP.SoftApIP}'>http://{WirelessAP.SoftApIP}</a></p>" +
                "</body></html>";

                WebServer.OutPutStream(e.Context.Response, route);

                // Needed to make sure all is getting out
                Thread.Sleep(200);

                // Disable the Soft AP
                WirelessAP.Disable();
                Thread.Sleep(200);
                Power.RebootDevice();
            }
        }
    }
}
