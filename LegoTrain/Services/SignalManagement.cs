// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using LegoTrain.Models;

namespace LegoTrain.Services
{
    public class SignalManagement : ISignalManagement, IDisposable
    {
        private static readonly HttpClient _client = new HttpClient();
        private LegoDiscovery _disco;

        public SignalManagement(LegoDiscovery legoDiscovery)
        {
            _disco = legoDiscovery;
        }

        public void ChangeSignal(byte numSignal, SignalState value)
        {
            try
            {
                var ip = _disco.DeviceDetails.Where(m => m.Id  == numSignal).First().IPAddress.ToString();
                _client.GetAsync($"http://{ip}/signal?md={(int)value}");
            }
            catch
            {
                // Nothing on purpose
            }
        }

        public SignalState GetSignal(byte numSignal)
        {
            int state = 0;
            try
            {
                var ip = _disco.DeviceDetails.Where(m => m.Id == numSignal).First().IPAddress.ToString();
                var res = _client.GetAsync($"http://{ip}/signalstatus").GetAwaiter().GetResult();

                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    int.TryParse(res.Content.ReadAsStringAsync().Result, out state);
                }
            }
            catch
            {
                // Nothing on purpose
            }

            return (SignalState)state;
        }

        public void Dispose()
        {
            // Nothing
        }
    }
}
