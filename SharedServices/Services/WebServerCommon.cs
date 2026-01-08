// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.Hardware.Esp32;
using nanoFramework.Runtime.Native;
using nanoFramework.WebServer;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

namespace SharedServices.Services
{
    /// <summary>
    /// Provides common web server functionality for nanoFramework devices.
    /// </summary>
    public static class WebServerCommon
    {
        private static ArrayList _files = new ArrayList();

        /// <summary>
        /// Handles WiFi configuration requests via web interface.
        /// </summary>
        /// <param name="e">The web server event arguments.</param>
        public static void SetupWifi(WebServerEventArgs e)
        {
            if (e.Context.Request.HttpMethod == "GET")
            {
                string route = $"<!DOCTYPE html><html<head><title>Configuration Page</title><link rel=\"stylesheet\" href=\"style.css\"></head><body>" +
                    "<h1>NanoFramework</h1>" +
                    "<form method='POST'  action='/'>" +
                    "<fieldset><legend>Wireless configuration</legend>" +
                    "Ssid:</br><input type='input' name='ssid' value='' ></br>" +
                    "Password:</br><input type='password' name='password' value='' >" +
                    "<br><br>" +
                    "<input type='submit' value='Save'>" +
                    "</fieldset>" +
                    "</form></body></html>";
                WebServer.OutputAsStream(e.Context.Response, route);
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

                var route = $"<!DOCTYPE html><html><head><title>WiFi configured</title><link rel=\"stylesheet\" href=\"style.css\"></head><body>" +
                    "<h1>Lego element wifi configuration</h1>" +
                    "<p>New settings saved.</p><p>Rebooting device to put into normal mode.</p>" +
                    "<p>Please allow up to 10 seconds to reconnect to the IP address.</p>";
                if (res)
                {
                    route += $"<p>IP Address shoud be <a href='http://{Wireless80211.GetCurrentIPAddress()}'>http://{Wireless80211.GetCurrentIPAddress()}</a>.</p>";
                }

                route += $"<p>If not configured properly, connect again to the SSID {WirelessAP.SoftApSsid} and then to <a href='http://{WirelessAP.SoftApIP}'>http://{WirelessAP.SoftApIP}</a></p>" +
                "</body></html>";

                WebServer.OutputAsStream(e.Context.Response, route);

                // Needed to make sure all is getting out
                Thread.Sleep(200);

                // Disable the Soft AP
                WirelessAP.Disable();
                Thread.Sleep(200);
                Sleep.EnableWakeupByTimer(new TimeSpan(0, 0, 0, 1));
                Sleep.StartDeepSleep();
            }
        }

        /// <summary>
        /// Populates the list of available files from the I:\ drive.
        /// </summary>
        public static void PopulateFiles()
        {
            _files.Clear();
            // list all files in directories and subdirectories of I:\ drive
            ListFiles("I:\\");
        }

        private static void ListFiles(string directory)
        {
            try
            {
                // Get all files in the current directory
                string[] files = Directory.GetFiles(directory);
                foreach (string file in files)
                {
                    _files.Add(file);
                }

                // Get all subdirectories in the current directory
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    // Recursively list files in each subdirectory
                    ListFiles(subdirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing files in directory {directory}: {ex.Message}");
            }
        }

        /// <summary>
        /// Serves static files from the I:\ drive.
        /// </summary>
        /// <param name="e">The web server event arguments.</param>
        public static void ServeStaticFiles(WebServerEventArgs e)
        {
            string path = e.Context.Request.RawUrl;

            try
            {
                var file = $"I:\\{ReplaceSlashWithBackslash(path.Substring(1))}";
                // Checks if the file exists in the list
                if (!_files.Contains(file))
                {
                    WebServer.OutputAsStream(e.Context.Response, "File not found");
                    return;
                }

                WebServer.SendFileOverHTTP(e.Context.Response, file);
            }
            catch (Exception)
            {
                WebServer.OutputAsStream(e.Context.Response, "File not found");
            }
        }

        /// <summary>
        /// Replaces forward slashes with backslashes in a path string.
        /// </summary>
        /// <param name="input">The input path string.</param>
        /// <returns>The path string with backslashes.</returns>
        public static string ReplaceSlashWithBackslash(string input)
        {
            char[] chars = input.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '/')
                {
                    chars[i] = '\\';
                }
            }
            return new string(chars);
        }
    }
}
