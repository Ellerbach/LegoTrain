// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using LegoTrain.Models;

namespace LegoTrain.Services
{
    /// <summary>
    /// Manages signal devices by sending HTTP commands to control their state.
    /// </summary>
    public class SignalManagement : ISignalManagement, IDisposable
    {
        private static readonly HttpClient _client = new HttpClient();
        private LegoDiscovery _disco;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalManagement"/> class.
        /// </summary>
        /// <param name="legoDiscovery">The device discovery service.</param>
        public SignalManagement(LegoDiscovery legoDiscovery)
        {
            _disco = legoDiscovery;
        }

        /// <summary>
        /// Changes the state of a signal by sending an HTTP request to the signal device.
        /// </summary>
        /// <param name="numSignal">The signal number.</param>
        /// <param name="value">The new signal state.</param>
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

        /// <summary>
        /// Gets the current state of a signal by querying the signal device.
        /// </summary>
        /// <param name="numSignal">The signal number.</param>
        /// <returns>The current signal state.</returns>
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

        /// <summary>
        /// Releases all resources used by the signal management service.
        /// </summary>
        public void Dispose()
        {
            // Nothing
        }
    }
}
