// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LegoTrain.Models;
using LegoTrain.Services;

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
                if (_configuration.Discovery.DeviceDetails.ContainsKey(device.Id)
                    && _configuration.Discovery.DeviceDetails[device.Id].DeviceCapacity.HasFlag(Models.Device.DeviceCapability.Switch))
                {
                    device.IPAddress = _configuration.Discovery.DeviceDetails[device.Id].IPAddress.ToString();
                    device.IsConnected = _configuration.Discovery.DeviceDetails[device.Id].DeviceStatus == Models.Device.DeviceStatus.Joining;
                }
                else
                {
                    device.IPAddress = string.Empty;
                    device.IsConnected = false;
                }
            }

            foreach (var device in _configuration.Switches)
            {
                if (_configuration.Discovery.DeviceDetails.ContainsKey(device.Id)
                    && _configuration.Discovery.DeviceDetails[device.Id].DeviceCapacity.HasFlag(Models.Device.DeviceCapability.Switch))
                {
                    device.IPAddress = _configuration.Discovery.DeviceDetails[device.Id].IPAddress.ToString();
                    device.IsConnected = _configuration.Discovery.DeviceDetails[device.Id].DeviceStatus == Models.Device.DeviceStatus.Joining;
                }
                else
                {
                    device.IPAddress = string.Empty;
                    device.IsConnected = false;
                }
            }

            if (_configuration.Discovery.DeviceDetails.ContainsKey(LegoInfraredExecutor.DeviceIDType) &&
                _configuration.Discovery.DeviceDetails[LegoInfraredExecutor.DeviceIDType].DeviceCapacity.HasFlag(Models.Device.DeviceCapability.Infrared))
            {
                _configuration.Infrared.IPAddress = _configuration.Discovery.DeviceDetails[LegoInfraredExecutor.DeviceIDType].IPAddress.ToString();
                _configuration.Infrared.IsConnected = _configuration.Discovery.DeviceDetails[LegoInfraredExecutor.DeviceIDType].DeviceStatus == Models.Device.DeviceStatus.Joining;
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
