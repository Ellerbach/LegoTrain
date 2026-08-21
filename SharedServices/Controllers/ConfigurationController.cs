// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.WebServer;
using System.Reflection;
using System.Text;
using System.Web;
using nanoFramework.Runtime;
using SharedServices.Models;
using SharedServices.Services;
using nanoFramework.Hardware.Esp32;
using System.Threading;

namespace SharedServices.Controllers
{
    /// <summary>
    /// Controller for handling configuration web pages on nanoFramework devices.
    /// </summary>
    internal class ConfigurationController
    {
        /// <summary>
        /// Gets or sets the application configuration instance.
        /// </summary>
        public static IAppConfiguration AppConfiguration { get; set; }

        /// <summary>
        /// Displays the configuration page for editing device settings.
        /// </summary>
        /// <param name="e">The web server event arguments.</param>
        [Route("config")]
        public void Config(WebServerEventArgs e)
        {
            // We need to clean things to get some memory
            //nanoFramework.Runtime.Native.GC.Run(true);

            // TODO: check the basic authentication
            string route = "<!DOCTYPE html><html><head><title>Configuration</title><link rel=\"stylesheet\" href=\"style.css\"></head><body><form action=\"/process\" method=\"post\"><fieldset><legend>Configuration</legend>";
            e.Context.Response.ContentType = "text/html";
            // It's the moment to create a new configuration

            string hiddenBool = "<input type=\"hidden\" name=\"bool\" value=\"";
            var methods = AppConfiguration.GetType().GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (method.Name.StartsWith("get_"))
                {
                    string name = method.Name.Substring(4);
                    var paramType = method.ReturnType;
                    string type;
                    switch (paramType.FullName)
                    {
                        case "System.Int32":
                            type = $"number\" value=\"{method.Invoke(AppConfiguration, null)}";
                            break;
                        case "System.Boolean":
                            var ret = method.Invoke(AppConfiguration, null);
                            type = $"checkbox\" {((bool)ret ? $"checked=\"" : string.Empty)}";
                            hiddenBool += $"{name}={ret};";
                            break;
                        default:                            
                            if (name.Contains("Password"))
                            {
                                type = $"password\" value=\"{method.Invoke(AppConfiguration, null)}";
                            }
                            else
                            {
                                type = $"input\" value=\"{method.Invoke(AppConfiguration, null)}";
                            }

                            break;
                    }

                    route += $"<label for=\"{name}\">{name}:</label><input name=\"{name}\" type=\"{type}\"/><br>";
                }
            }

            // We need to clean things to get some memory
            //methods = null;
            //nanoFramework.Runtime.Native.GC.Run(true);

            hiddenBool += hiddenBool.TrimEnd(';') + "\"/>";
            route += hiddenBool;
            route += "<input type=\"submit\" value=\"Save\"></fieldset></form><br>Device ID should start at 1.</body></html>";
            WebServer.OutputAsStream(e.Context.Response, route);
        }

        /// <summary>
        /// Processes the configuration form submission and saves the settings.
        /// </summary>
        /// <param name="e">The web server event arguments.</param>
        [Route("process")]
        [Method("POST")]
        public void Process(WebServerEventArgs e)
        {
            byte[] buff = new byte[e.Context.Request.ContentLength64];
            e.Context.Request.InputStream.Read(buff, 0, buff.Length);
            string paramString = Encoding.UTF8.GetString(buff, 0, buff.Length);

            // We're adding back the question mark as it's not present when posting
            var parameters = WebServer.DecodeParam($"{WebServer.ParamStart}{paramString}");
            // Let's keep track of our bools
            string[] bools = new string[0];
            foreach (UrlParameter param in parameters)
            {
                if (param.Name == "bool")
                {
                    bools = HttpUtility.UrlDecode(param.Value).Split(';');
                }
            }

            foreach (string elem in bools)
            {
                var keyVal = elem.Split('=');
                if (keyVal[1].ToLower() == "true")
                {
                    bool found = false;
                    foreach (UrlParameter param in parameters)
                    {
                        if (keyVal[0] == param.Name)
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        var memberPropSetMethod = AppConfiguration.GetType().GetMethod("set_" + keyVal[0]);
                        if (memberPropSetMethod != null)
                        {
                            memberPropSetMethod.Invoke(AppConfiguration, new object[] { false });
                        }
                    }
                }
            }

            foreach (UrlParameter param in parameters)
            {
                var memberPropSetMethod = AppConfiguration.GetType().GetMethod("set_" + param.Name);
                if (memberPropSetMethod != null)
                {
                    var setter = memberPropSetMethod.GetParameters()[0];
                    switch (setter.ParameterType.FullName)
                    {
                        case "System.Int32":
                            if (int.TryParse(param.Value, out int val))
                            {
                                memberPropSetMethod.Invoke(AppConfiguration, new object[] { val });
                            }
                            break;
                        case "System.String":
                            memberPropSetMethod.Invoke(AppConfiguration, new object[] { HttpUtility.UrlDecode(param.Value) });
                            break;
                        case "System.Boolean":
                            // The value is only sent when the checkbox is checked!
                            memberPropSetMethod.Invoke(AppConfiguration, new object[] { true });
                            break;
                        default:
                            break;
                    }
                }
            }

            // We need to clean things to get some memory
            nanoFramework.Runtime.Native.GC.Run(true);
            AppConfiguration.Save();
            string route = $"<!DOCTYPE html><html><head><title>Lego Element Configuration Page</title><link rel=\"stylesheet\" href=\"style.css\"></head><body>Configuration saved and updated. Return to the <a href=\"http://{Wireless80211.GetCurrentIPAddress()}\">home page</a>.</body></html>";
            WebServer.OutputAsStream(e.Context.Response, route);
        }

        /// <summary>
        /// Displays the WiFi reset page and starts the Access Point mode.
        /// </summary>
        /// <param name="e">The web server event arguments.</param>
        [Route("resetwifi")]
        public void ResetWifi(WebServerEventArgs e)
        {
            string route = $"<!DOCTYPE html><html><head><title>Lego Element Wireless Configuration</title><link rel=\"stylesheet\" href=\"style.css\"></head><body>" +
                    "<h1>Wireless Lego Element Configuration</h1>" +
                    "<form method='POST' action='/'>" +
                    "<fieldset><legend>Wireless configuration</legend>" +
                    "Ssid:</br><input type='input' name='ssid' value='' ></br>" +
                    "Password:</br><input type='password' name='password' value='' >" +
                    "<br><br>" +
                    "<input type='submit' value='Save'>" +
                    "</fieldset>" +
                    "</form></body></html>";
            WebServer.OutputAsStream(e.Context.Response, route);
            WirelessAP.SetWifiAp();
        }

        /// <summary>
        /// Reboots the device after displaying a confirmation page.
        /// </summary>
        /// <param name="e">The web server event arguments.</param>
        [Route("reboot")]
        public void Reboot(WebServerEventArgs e)
        {
            string route = $"<!DOCTYPE html><html><head><title>Reboot</title></head><body>" +
                "<meta http-equiv=\"refresh\" content=\"0; url=/\" />" +
                "</body></html>";
            WebServer.OutputAsStream(e.Context.Response, route);
            Sleep.EnableWakeupByTimer(new System.TimeSpan(0, 0, 0, 1));
            Sleep.StartDeepSleep();
        }
    }
}
