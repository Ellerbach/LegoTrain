// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using nanoFramework.Hardware.Esp32;
using nanoFramework.WebServer;
using SharedServices.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using SharedServices.Controllers;
using SharedServices.Services;
using nanoDiscovery.Common;
using LegoElement.Models;
using LegoElement.Controllers;
using System.Net.Http;
using nanoFramework.Hardware.Esp32.Touch;

namespace LegoElement
{
    public class Application
    {
        public static AppConfiguration AppConfiguration { get => _appConfiguration; }
        public static Detector[] Detectors { get; } = new Detector[2];

        private static AppConfiguration _appConfiguration;
        private static WebServer _server;

        private static LegoDiscovery _legoDiscovery;
        private static CancellationTokenSource _legoDiscoToken;
        private static bool _wifiApMode = false;
        private static Blinky _blinky;
        private static int _tries = 0;
        private static Timer _timerWiatingWifiSetup;
        private static HttpClient _httpClient;

        public static void Main()
        {
            Debug.WriteLine("Hello Lego Detector");

            // Try to read the configuration
            _appConfiguration = AppConfiguration.Load();
            if (AppConfiguration == null)
            {
                _appConfiguration = new AppConfiguration();
                _appConfiguration.LedGpio = 8;
                _appConfiguration.FirstDetectorActivated = false;
                _appConfiguration.SecondDetectorActivated = false;
                _appConfiguration.Detector1Pin = 7;
                _appConfiguration.Detector2Pin = 10;
                _appConfiguration.Detector1MinimumThreshold = 100;
                _appConfiguration.Detector1MaximumThreshold = 200;
                _appConfiguration.ApiPort = 8080;
                _appConfiguration.Save();
            }
            else
            {
                SetDetector(0);
                SetDetector(1);
                SetThreashold();
                if (_appConfiguration.StartDetectionAutomatically)
                {
                    if (Detectors[0] != null)
                    {
                        Detectors[0].Detect = true;
                    }

                    if (Detectors[1] != null)
                    {
                        Detectors[1].Detect = true;
                    }
                }
            }

            ConfigurationController.AppConfiguration = _appConfiguration;

            _wifiApMode = Wireless80211.ConnectOrSetAp();
            _blinky = new Blinky(_appConfiguration.LedGpio);

            // If we are in normal mode, advertize the service
            if (!_wifiApMode)
            {
                SetDiscovery();
                _blinky.BlinkNormal();
            }
            else
            {
                _blinky.BlinkWaiWifi();
                if (Wireless80211.IsEnabled())
                {
                    // This will reboot the device every 5 minutes as we seem to have a valid configuration
                    // It will then try to reconnect
                    _timerWiatingWifiSetup = new Timer(TimerCallBackReboot, null, 5 * 60 * 1000, 0);
                }
            }

            Debug.WriteLine($"Connected with wifi credentials. IP Address: {(_wifiApMode ? WirelessAP.GetIP() : Wireless80211.GetCurrentIPAddress())}");
            _server = new WebServer(80, HttpProtocol.Http, new Type[] { typeof(ApiController), typeof(ConfigurationController) });
            // Add a handler for commands that are received by the server.
            _server.CommandReceived += ServerCommandReceived;
            _server.WebServerStatusChanged += WebServerStatusChanged;

            // Start the server.
            _server.Start();

            AppConfiguration.OnConfigurationUpdated += OnConfigurationUpdated;

            Thread.Sleep(Timeout.Infinite);
        }

        private static void TimerCallBackReboot(object state)
        {
            // We will basically try to reconnect after 1h
            Sleep.EnableWakeupByTimer(new TimeSpan(0, 0, 0, 1));
            Sleep.StartDeepSleep();
        }

        private static void WebServerStatusChanged(object obj, WebServerStatusEventArgs e)
        {
            if (e.Status == WebServerStatus.Stopped)
            {
                if (_tries++ < 5)
                {
                    _server.Start();
                }
                else
                {
                    Sleep.EnableWakeupByTimer(new TimeSpan(0, 0, 0, 1));
                    Sleep.StartDeepSleep();
                }
            }
        }

        private static void OnConfigurationUpdated(object sender, ConfigurationEventArgs e)
        {
            Debug.WriteLine($"Parameter updated: {e.ParamName}");
            // Check the mode
            if (e.ParamName.StartsWith("Detector1"))
            {
                SetDetector(0);
            }
            else if (e.ParamName.StartsWith("Detector2"))
            {
                SetDetector(1);
            }

            if (e.ParamName.EndsWith("Threshold"))
            {
                SetThreashold();
            }

            if (e.ParamName.StartsWith("Device") || e.ParamName.EndsWith("Activated"))
            {
                SetDiscovery();
            }
            else
            {
                _blinky?.Dispose();
                _blinky.BlinkNormal();
            }
        }

        private static void SetThreashold()
        {

            if (Detectors[0] != null)
            {
                Detectors[0].Threshold = (AppConfiguration.Detector1MaximumThreshold - AppConfiguration.Detector1MinimumThreshold) / 2 + AppConfiguration.Detector1MinimumThreshold;
            }

            if (Detectors[1] != null)
            {
                Detectors[1].Threshold = (AppConfiguration.Detector2MaximumThreshold - AppConfiguration.Detector2MinimumThreshold) / 2 + AppConfiguration.Detector2MinimumThreshold;
            }
        }

        private static void SetDetector(int decNum)
        {
            try
            {
                Detector detector = new Detector(decNum == 0 ? AppConfiguration.Detector1Pin : AppConfiguration.Detector2Pin, decNum);
                Detectors[decNum] = detector;
                Detectors[decNum].OnDetection += OnDetection;
            }
            catch (Exception)
            {
                Detectors[decNum] = null;
            }
        }

        private static void OnDetection(object sender, int val)
        {
            // Faking calling the API for now
            Debug.WriteLine($"Detected {((Detector)sender).Id}: {val}");
            try
            {
                //_httpClient ??= new HttpClient();
                //HttpResponseMessage response = _httpClient.Get($"http://{_legoDiscovery.ServerAddress}:{_appConfiguration.ApiPort}/Api/detect?id={AppConfiguration.DeviceId}&de={((Detector)sender).Id}&va={val}");
                //response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
            }
        }

        private static void SetDiscovery()
        {
            Debug.WriteLine("Starting discovery service");
            // We strart the discovery service

            DeviceCapability capacities = DeviceCapability.None;
            if ((_appConfiguration.FirstDetectorActivated) && (_appConfiguration.SecondDetectorActivated))
            {
                capacities |= DeviceCapability.DoubleDetector;
            }
            else if ((_appConfiguration.FirstDetectorActivated) || (_appConfiguration.SecondDetectorActivated))
            {
                capacities |= DeviceCapability.Detector;
            }

            _legoDiscoToken?.Cancel();
            _legoDiscovery?.Dispose();
            _legoDiscovery = new LegoDiscovery(IPAddress.Parse(Wireless80211.GetCurrentIPAddress()), _appConfiguration.DeviceId, capacities);
            _legoDiscoToken?.Dispose();
            _legoDiscoToken = new CancellationTokenSource();
            _legoDiscovery.SendCapabilities(IPAddress.Parse("255.255.255.255"));
            _legoDiscovery.Run(_legoDiscoToken.Token);
        }

        private static void ServerCommandReceived(object obj, WebServerEventArgs e)
        {
            // Not enough memory to handle those!
            if (e.Context.Request.RawUrl.StartsWith("/style.css"))
            {
                //e.Context.Response.ContentType = "text/css";
                //WebServer.OutPutStream(e.Context.Response, ResourceWeb.GetString(ResourceWeb.StringResources.style));
                return;
            }
            //else if (e.Context.Request.RawUrl.StartsWith("/favicon.ico"))
            //{
            //    var ico = ResourceWeb.GetBytes(ResourceWeb.BinaryResources.favicon);
            //    e.Context.Response.ContentType = "image/ico";
            //    e.Context.Response.ContentLength64 = ico.Length;
            //    e.Context.Response.OutputStream.Write(ico, 0, ico.Length);
            //    return;
            //}

            if (e.Context.Request.RawUrl.StartsWith("/calibrate"))
            {
                string detectorStatus = string.Empty;
                bool isMin = false;
                if (e.Context.Request.RawUrl.Contains("min"))
                {
                    isMin = true;
                    if (Detectors[0] != null)
                    {
                        AppConfiguration.Detector1MinimumThreshold = Detectors[0].Value;
                        detectorStatus += $"First Detector: {AppConfiguration.Detector1MinimumThreshold} with light.<br/>";
                    }

                    if (Detectors[1] != null)
                    {
                        AppConfiguration.Detector2MinimumThreshold = Detectors[0].Value;
                        detectorStatus += $"Second Detector: {AppConfiguration.Detector2MinimumThreshold} with light.<br/>";
                    }

                    AppConfiguration.Save();

                }
                else if (e.Context.Request.RawUrl.Contains("max"))
                {
                    if (Detectors[0] != null)
                    {
                        AppConfiguration.Detector1MaximumThreshold = Detectors[0].Value;
                        detectorStatus += $"First Detector: {AppConfiguration.Detector1MaximumThreshold} with train.<br/>";
                    }

                    if (Detectors[1] != null)
                    {
                        AppConfiguration.Detector2MaximumThreshold = Detectors[0].Value;
                        detectorStatus += $"Second Detector: {AppConfiguration.Detector2MaximumThreshold} with train.<br/>";
                    }

                    AppConfiguration.Save();
                }

                string toOutput = "<html><head><title>Lego Train Detector</title><link rel=\"stylesheet\" href=\"style.css\"></head><body>";
                toOutput += "<h1>Calibration</h1>";
                toOutput += "<p>Leave the rails empty, ensure nothing is on the rails and you are at the normal light.</p>";
                if (isMin)
                {
                    toOutput += detectorStatus;
                }

                toOutput += "<form method='POST' action='/calibratemin'>";
                toOutput += "<input type='submit' value='Calibrate with light'>";
                toOutput += "</form>";
                toOutput += "<p>Place a train or a wagon that fully covers the two sensors.</p>";
                if (!isMin)
                {
                    toOutput += detectorStatus;
                }

                toOutput += "<form method='POST' action='/calibratemax'>";
                toOutput += "<input type='submit' value='Calibrate with train'>";
                toOutput += "</form>";
                toOutput += $"Return to the <a href=\"http://{Wireless80211.GetCurrentIPAddress()}\">home page</a>.";
                toOutput += "</body></html>";
                WebServer.OutPutStream(e.Context.Response, toOutput);
                return;
            }

            if (_wifiApMode)
            {
                WebServerCommon.SetupWifi(e);
            }
            else
            {
                string toOutput = "<html><head><title>Lego Train Detector</title><link rel=\"stylesheet\" href=\"style.css\"></head><body>";
                if (AppConfiguration.FirstDetectorActivated)
                {
                    toOutput += $"Your first detector is <b>{(Detectors[0] == null ? "incorrect" : "valid")}</b>. Configuration is <b>{(Detectors[0] != null ? "valid" : "not valid")}</b>.<br/>";
                }

                if (AppConfiguration.SecondDetectorActivated)
                {
                    toOutput += $"Your second detector is <b>{(Detectors[1] == null ? "incorrect" : "valid")}</b>. Configuration is <b>{(Detectors[1] != null ? "valid" : "not valid")}</b>.<br/>";
                }

                if (!AppConfiguration.FirstDetectorActivated && !AppConfiguration.SecondDetectorActivated)
                {
                    toOutput += "You haven't set a proper device type and no detector setup. Go to <a href=\"/config\">configuration</a><br/>";
                }
                else
                {
                    toOutput += "<p>Don't forget to <a href='calibrate'>calibrate</a> your detector.</p>";
                    toOutput += $"Your device ID is {(AppConfiguration.DeviceId < 0 ? "invalid, it must be more or equal to 1." : AppConfiguration.DeviceId)}.<br>";
                }

                toOutput += "To configure your device please go to <a href=\"/config\">configuration</a>.<br/>";
                toOutput += "Reset your wifi by cliking <a href=\"/resetwifi\">here</a>.<br>";
                toOutput += "</body></html>";
                WebServer.OutPutStream(e.Context.Response, toOutput);
                return;
            }
        }
    }
}
