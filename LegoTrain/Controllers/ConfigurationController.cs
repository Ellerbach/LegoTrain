// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using LegoTrain.Models;
using LegoTrain.Services;

namespace LegoTrain.Controllers
{
    [Route("[controller]")]
    public class ConfigurationController : Controller
    {
        private readonly AppConfiguration _configuration;

        public ConfigurationController(AppConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET: ConfigurationController
        [HttpGet()]
        public ActionResult Index()
        {

            foreach (var device in _configuration.Signals)
            {
                if (_configuration.Discovery.DeviceDetails.ContainsKey(device.Id)
                    && _configuration.Discovery.DeviceDetails[device.Id].DeviceCapacity.HasFlag(Models.Device.DeviceCapability.Signal))
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
    }
}
