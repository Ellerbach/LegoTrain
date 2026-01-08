// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Iot.Device.ServoMotor;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Diagnostics;

namespace LegoTrain.Services
{
    /// <summary>
    /// Manages track switch devices by sending HTTP commands to control their state.
    /// </summary>
    public class SwitchManagement : ISwitchManagement
    {
        private static readonly HttpClient _client = new HttpClient();

        private readonly LegoDiscovery _disco;

        // create a new servo
        // Rotational Range: 203° 
        // Pulse Cycle: 20 ms 
        // Pulse Width: 800-2190 µs 

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchManagement"/> class.
        /// </summary>
        /// <param name="discovery">The device discovery service.</param>
        public SwitchManagement(LegoDiscovery discovery)
        {
            _disco = discovery;
        }

        /// <summary>
        /// Changes the state of a switch by sending an HTTP request to the switch device.
        /// </summary>
        /// <param name="numSwitch">The switch number.</param>
        /// <param name="value">The new switch state (true for one position, false for the other).</param>
        public void ChangeSwitch(byte numSwitch, bool value)
        {
            try
            {
                var ip = _disco.DeviceDetails.Where(m => m.Id == numSwitch).First().IPAddress.ToString();
                _client.GetAsync($"http://{ip}/switch?md={(value ? "1" : "0")}");
            }
            catch
            {
                // Nothing on purpose
            }
        }

        /// <summary>
        /// Gets the current state of a switch by querying the switch device.
        /// </summary>
        /// <param name="numSwitch">The switch number.</param>
        /// <returns>The current switch state.</returns>
        public bool GetSwitch(byte numSwitch)
        {
            int state = 0;
            try
            {
                var ip = _disco.DeviceDetails.Where(m => m.Id == numSwitch).First().IPAddress.ToString();
                var res = _client.GetAsync($"http://{ip}/switchstatus").GetAwaiter().GetResult();

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    int.TryParse(res.Content.ReadAsStringAsync().Result, out state);
                }
            }
            catch
            {
                // Nothing on purpose
            }

            return state == 1;
        }

        /// <summary>
        /// Releases all resources used by the switch management service.
        /// </summary>
        public void Dispose()
        {
            // Nothing
        }
    }
}
