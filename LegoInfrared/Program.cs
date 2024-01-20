using LegoElement.Controllers;
using LegoElement.Models;
using LegoElement.Services;
using LegoInfrared;
using nanoFramework.Hardware.Esp32;
using nanoFramework.WebServer;
using SharedServices.Controllers;
using SharedServices.Models;
using SharedServices.Services;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace LegoElement
{
    public class Application
    {
        private static AppConfiguration _appConfiguration;
        private static Lego.Infrared.LegoInfrared _legoInfrared;
        public static AppConfiguration AppConfiguration { get => _appConfiguration; }
        public static Lego.Infrared.LegoInfrared LegoInfrared { get => _legoInfrared; }
        private static WebServer _server;
        private static bool _wifiApMode = false;
        private static LegoDiscovery _legoDiscovery;
        private static CancellationTokenSource _legoDiscoToken;

        public static void Main()
        {
            Console.WriteLine("Lego Infrared REST API and Serial module");

            // Try to read the configuration
            _appConfiguration = AppConfiguration.Load();
            if (AppConfiguration != null)
            {
                SetLegoInfrared();
            }

            _appConfiguration = AppConfiguration ?? new AppConfiguration();

            _wifiApMode = Wireless80211.ConnectOrSetAp();

            // If we are in normal mode, advertize the service
            if (!_wifiApMode)
            {
                SetDiscovery();
            }

            Console.WriteLine($"Connected with wifi credentials. IP Address: {(_wifiApMode ? WirelessAP.GetIP() : Wireless80211.GetCurrentIPAddress())}");
            _server = new WebServer(80, HttpProtocol.Http, new Type[] { typeof(ApiController), typeof(ConfigurationController) });
            // Add a handler for commands that are received by the server.
            _server.CommandReceived += ServerCommandReceived;

            // Start the server.
            _server.Start();

            AppConfiguration.OnConfigurationUpdated += OnConfigurationUpdated;

            Thread.Sleep(Timeout.Infinite);
        }

        private static void OnConfigurationUpdated(object sender, ConfigurationEventArgs e)
        {
            Console.WriteLine($"Parameter updated: {e.ParamName}");
            if (e.ParamName.StartsWith("Spi"))
            {
                SetLegoInfrared();
            }
        }

        private static void ServerCommandReceived(object obj, WebServerEventArgs e)
        {
            if (e.Context.Request.RawUrl.StartsWith("/style.css"))
            {
                e.Context.Response.ContentType = "text/css";
                WebServer.OutPutStream(e.Context.Response, ResourceWeb.GetString(ResourceWeb.StringResources.style));
                return;
            }
            else if (e.Context.Request.RawUrl.StartsWith("/favicon.ico"))
            {
                var ico = ResourceWeb.GetBytes(ResourceWeb.BinaryResources.favicon);
                e.Context.Response.ContentType = "image/ico";
                e.Context.Response.ContentLength64 = ico.Length;
                e.Context.Response.OutputStream.Write(ico, 0, ico.Length);
                return;
            }

            if (_wifiApMode)
            {
                WebServerCommon.SetupWifi(e);
            }
            else
            {
                string toOutput = "<html><head>" +
                    $"<title>Lego Infrared</title></head><body>" +
                    $"Your Lego Infrared configuraiton is: {(LegoInfrared == null ? "Invalid" : "Valid")}<br/>";
                toOutput += "<a href='test'>Access</a> the Lego Infrared test page.<br>";
                toOutput += "To configure your device please go to <a href=\"config\">configuration</a><br/>";
                toOutput += "Reset your wifi by cliking <a href=\"resetwifi\">here</a>.";
                toOutput += "</body></html>";
                WebServer.OutPutStream(e.Context.Response, toOutput);
            }
        }

        private static void SetLegoInfrared()
        {
            if (_legoInfrared != null)
            {
                _legoInfrared.Dispose();
                _legoInfrared = null;
            }

            // On an ESP32, setup first the pins for the SPI
            if (AppConfiguration.SpiClock >= 0)
            {
                Configuration.SetPinFunction(AppConfiguration.SpiClock, DeviceFunction.SPI1_CLOCK);
            }

            if (AppConfiguration.SpiMiso >= 0)
            {
                Configuration.SetPinFunction(AppConfiguration.SpiMiso, DeviceFunction.SPI1_MISO);
            }

            if (AppConfiguration.SpiMosi >= 0)
            {
                Configuration.SetPinFunction(AppConfiguration.SpiMosi, DeviceFunction.SPI1_MOSI);
            }

            try
            {
                _legoInfrared = new Lego.Infrared.LegoInfrared(AppConfiguration.SpiBus, AppConfiguration.SpiChipSelect);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Invalid LegoInfrared configuration: {e.Message}");
            }

            LegoInfraredExecute.LegoInfrared = _legoInfrared;
        }

        private static void SetDiscovery()
        {
            Debug.WriteLine("Starting discovery service");
            // We strart the discovery service

            _legoDiscoToken?.Cancel();
            _legoDiscovery?.Dispose();
            // Device ID is 0 for the infrared module, you can and should only have 1 to avoid interference
            // It is anyway something easy to add later on.
            _legoDiscovery = new LegoDiscovery(IPAddress.Parse(Wireless80211.GetCurrentIPAddress()), 0, ":IR");
            _legoDiscoToken?.Dispose();
            _legoDiscoToken = new CancellationTokenSource();
            _legoDiscovery.SendCapabilities();
            _legoDiscovery.Run(_legoDiscoToken.Token);
        }
    }
}
