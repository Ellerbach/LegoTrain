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
using System.Net.WebSockets.Server;
using System.Net.WebSockets;
using System.Net.WebSockets.WebSocketFrame;
using System.Text;

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
        private static WebSocketServer _wsServer;

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
                _appConfiguration.Detector2MinimumThreshold = 100;
                _appConfiguration.Detector2MaximumThreshold = 200;
                _appConfiguration.ApiPort = 8080;
                _appConfiguration.Save();
            }

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

            //Initialize WebsocketServer with Webserver intergration
            _wsServer = new WebSocketServer(new WebSocketServerOptions()
            {
                MaxClients = 10,
                IsStandAlone = false
            });

            _wsServer.MessageReceived += WsServerMessageReceived;
            _wsServer.Start();

            WebServerCommon.PopulateFiles();

            _server = new WebServer(80, HttpProtocol.Http, new Type[] { typeof(ApiController), typeof(ConfigurationController) });
            // Add a handler for commands that are received by the server.
            _server.CommandReceived += ServerCommandReceived;
            _server.WebServerStatusChanged += WebServerStatusChanged;

            // Start the server.
            _server.Start();

            AppConfiguration.OnConfigurationUpdated += OnConfigurationUpdated;

            Thread.Sleep(Timeout.Infinite);
        }

        private static void WsServerMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var wsServer = (WebSocketServer)sender;
            if (e.Frame.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(e.Frame.Buffer, 0, e.Frame.Buffer.Length);
                if (message == "status")
                {
                    wsServer.BroadCast(Encoding.UTF8.GetBytes(GetDetectorStatus()));
                }
                else if (message == "start")
                {
                    if (Application.Detectors[0] != null)
                    {
                        Application.Detectors[0].Detect = true;
                    }

                    if (Application.Detectors[1] != null)
                    {
                        Application.Detectors[1].Detect = true;
                    }

                    wsServer.BroadCast(e.Frame.Buffer);
                }
            }
        }

        private static string GetDetectorStatus()
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

            return resp;
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

            if (e.ParamName == nameof(AppConfiguration.Detector1Pin))
            {
                SetDetector(0);
                SetThreashold();
            }
            else if (e.ParamName == nameof(AppConfiguration.Detector2Pin))
            {
                SetDetector(1);
                SetThreashold();
            }
            else if (e.ParamName.EndsWith("Threshold"))
            {
                SetThreashold();
            }

            if (e.ParamName.StartsWith("Device") || e.ParamName.EndsWith("Activated"))
            {
                SetDiscovery();
            }

            if (e.ParamName == nameof(AppConfiguration.LedGpio))
            {
                if (_blinky != null)
                {
                    _blinky.Dispose();
                }

                _blinky = new Blinky(_appConfiguration.LedGpio);
                if (_wifiApMode)
                {
                    _blinky.BlinkWaiWifi();
                }
                else
                {
                    _blinky.BlinkNormal();
                }
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
            Detectors[decNum]?.Dispose();
            Detectors[decNum] = null;
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

        private static void OnDetection(object sender, int val, bool detected)
        {
            // Faking calling the API for now
            Debug.WriteLine($"Detected {((Detector)sender).Id}: {val}");
            try
            {
                if (_wsServer.ClientsCount > 0)
                {
                    string url = $"id={AppConfiguration.DeviceId}&de={((Detector)sender).Id}&va={val}&on={(detected ? "1" : "0")}";
                    _wsServer.BroadCast(Encoding.UTF8.GetBytes(url));
                }
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
            //check the path of the request
            if (e.Context.Request.RawUrl == "/")
            {
                //check if this is a websocket request or a page request 
                if (e.Context.Request.Headers["Upgrade"] == "websocket")
                {
                    //Upgrade to a websocket
                    _wsServer.AddWebSocket(e.Context);
                }
            }

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
                        AppConfiguration.Detector2MinimumThreshold = Detectors[1].Value;
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
                        AppConfiguration.Detector2MaximumThreshold = Detectors[1].Value;
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
                WebServer.OutputAsStream(e.Context.Response, toOutput);
                return;
            }

            if (_wifiApMode)
            {
                WebServerCommon.SetupWifi(e);
                return;
            }
            else if (e.Context.Request.RawUrl == "/")
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
                WebServer.OutputAsStream(e.Context.Response, toOutput);
                return;
            }

            WebServerCommon.ServeStaticFiles(e);
        }
    }
}
