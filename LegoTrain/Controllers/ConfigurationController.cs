// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using LegoTrain.Models;
using LegoTrain.Services;
using nanoDiscovery.Common;
using LegoTrain.Models.Device;

namespace LegoTrain.Controllers
{
    /// <summary>
    /// Controller for managing the overall configuration of the Lego train system.
    /// </summary>
    [Route("[controller]")]
    public class ConfigurationController : Controller
    {
        private readonly AppConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public ConfigurationController(AppConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Displays the main configuration page showing all signals, switches, and infrared devices.
        /// </summary>
        /// <returns>The configuration index view.</returns>
        // GET: ConfigurationController
        [HttpGet()]
        public ActionResult Index()
        {

            foreach (var device in _configuration.Signals)
            {
                var dev = _configuration.Discovery.DeviceDetails.Where(m => m.Id == device.Id).FirstOrDefault();
                if ((dev != null)
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

            foreach (var device in _configuration.Switches)
            {
                var dev = _configuration.Discovery.DeviceDetails.Where(m => m.Id == device.Id).FirstOrDefault();
                if ((dev != null)
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

            var devI = _configuration.Discovery.DeviceDetails.Where(m => m.Id == LegoInfraredExecutor.DeviceIDType).FirstOrDefault();
            if ( devI!= null &&
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
    }
}
