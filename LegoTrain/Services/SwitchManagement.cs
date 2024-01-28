// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Iot.Device.ServoMotor;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Diagnostics;

namespace LegoTrain.Services
{
    public class SwitchManagement : ISwitchManagement
    {
        private static readonly HttpClient _client = new HttpClient();

        private readonly LegoDiscovery _disco;

        // create a new servo
        // Rotational Range: 203° 
        // Pulse Cycle: 20 ms 
        // Pulse Width: 800-2190 µs 

        public SwitchManagement(LegoDiscovery discovery)
        {
            _disco = discovery;
        }

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

        public void Dispose()
        {
            // Nothing
        }
    }
}
