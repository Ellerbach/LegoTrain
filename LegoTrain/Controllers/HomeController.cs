// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LegoTrain.Models;
using LegoTrain.Services;
using nanoDiscovery.Common;
using LegoTrain.Models.Device;

namespace LegoTrain.Controllers
{
    /// <summary>
    /// Controller for the home page and main navigation.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The application configuration.</param>
        public HomeController(ILogger<HomeController> logger, AppConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Displays the home page with an overview of all switches, signals, and infrared devices.
        /// </summary>
        /// <returns>The home page view.</returns>
        public IActionResult Index()
        {
            foreach (var device in _configuration.Switches)
            {
                var dev = _configuration.Discovery.DeviceDetails.Where(m => m.Id == device.Id).FirstOrDefault();
                if (dev != null
                    && dev.DeviceCapacity.HasFlag(DeviceCapability.Switch))
                {
                    device.IPAddress = dev.IPAddress.ToString();
                    device.IsConnected = dev.DeviceStatus == DeviceStatus.Joining;
                }
                else
                {
                    device.IPAddress = string.Empty;
                    device.IsConnected = false;
                }
            }

            foreach (var device in _configuration.Signals)
            {
                var dev = _configuration.Discovery.DeviceDetails.Where(m => m.Id == device.Id).FirstOrDefault();
                if (dev != null
                    && dev.DeviceCapacity.HasFlag(DeviceCapability.Signal))
                {
                    device.IPAddress = dev.IPAddress.ToString();
                    device.IsConnected = dev.DeviceStatus == DeviceStatus.Joining;
                }
                else
                {
                    device.IPAddress = string.Empty;
                    device.IsConnected = false;
                }
            }

            var devI = _configuration.Discovery.DeviceDetails.Where(m => m.Id == LegoInfraredExecutor.DeviceIDType).FirstOrDefault();
            if (devI!=null &&
                devI.DeviceCapacity.HasFlag(DeviceCapability.Infrared))
            {
                _configuration.Infrared.IPAddress = devI.IPAddress.ToString();
                _configuration.Infrared.IsConnected = devI.DeviceStatus == DeviceStatus.Joining;
            }
            else
            {
                _configuration.Infrared.IPAddress = string.Empty;
                _configuration.Infrared.IsConnected = false;
            }

            return View(_configuration);
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// </summary>
        /// <returns>The privacy policy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page.
        /// </summary>
        /// <returns>The error view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
