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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, AppConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
