
using Lego.Infrared;
using System.Net;
using System;

namespace LegoTrain.Services
{
    /// <summary>
    /// Executes Lego infrared commands by sending HTTP requests to an infrared transmitter device.
    /// </summary>
    public class LegoInfraredExecutor
    {
        /// <summary>
        /// The device ID used for infrared transmitter devices.
        /// </summary>
        public const int DeviceIDType = 0;
        private static readonly HttpClient _client = new HttpClient();
        private LegoDiscovery _disco;

        /// <summary>
        /// Initializes a new instance of the <see cref="LegoInfraredExecutor"/> class.
        /// </summary>
        /// <param name="legoDiscovery">The device discovery service.</param>
        public LegoInfraredExecutor(LegoDiscovery legoDiscovery)
        {
            _disco = legoDiscovery;
        }

        /// <summary>
        /// Sends a combo command to control two motors on the specified channel.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="bl">The blue motor speed.</param>
        /// <param name="rd">The red motor speed.</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool Combo(Channel ch, Speed bl, Speed rd)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/combo?rd=0&bl=0&ch=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/combo?rd={(int)rd}&bl={(int)bl}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a single PWM command to control a motor.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="pw">The PWM speed.</param>
        /// <param name="op">The output (Red or Blue).</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool SinglePwm(Channel ch, PwmSpeed pw, PwmOutput op)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/singlepwm?pw=0&op=0&ch=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/singlepwm?pw={(int)pw}&op={(int)op}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a continuous function command.
        /// </summary>
        /// <param name="fc">The function.</param>
        /// <param name="op">The output.</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool Continuous(Function fc, Output op)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/continuous?fc=0&op=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/continuous?fc={(int)fc}&op={(int)op}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a single clear/set/toggle command.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="pw">The clear/set/toggle value.</param>
        /// <param name="op">The output.</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool SingleCst(Channel ch, ClearSetToggle pw, PwmOutput op)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/singlecst?pw=0&op=0&ch=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/singlecst?pw={(int)pw}&op={(int)op}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a timeout function command.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="fc">The function.</param>
        /// <param name="op">The output.</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool Timeout(Channel ch, Function fc, Output op)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/timeout?fc=0&op=0&ch=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/timeout?fc={(int)fc}&op={(int)op}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends combo commands to all four channels simultaneously.
        /// </summary>
        /// <param name="comboBlue">Array of blue motor speeds for channels 0-3.</param>
        /// <param name="comboRed">Array of red motor speeds for channels 0-3.</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool ComboAll(Speed[] comboBlue, Speed[] comboRed)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/comboall?rd0=0&bl0=0&rd1=0&bl1=0&rd2=0&bl2=0&rd3=0&bl3=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/comboall?rd0={(int)comboRed[0]}&bl0={(int)comboBlue[0]}&rd1={(int)comboRed[1]}&bl1={(int)comboBlue[1]}&rd2={(int)comboRed[2]}&bl2={(int)comboBlue[2]}&rd3={(int)comboRed[3]}&bl3={(int)comboBlue[3]}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends continuous function commands to all four channels simultaneously.
        /// </summary>
        /// <param name="function">Array of functions for channels 0-3.</param>
        /// <param name="output">Array of outputs for channels 0-3.</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool ContinuousAll(Function[] function, Output[] output)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/continuousall?fc0=0&op0=0&fc1=0&op1=0&fc2=0&op2=0&fc3=0&op3=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/continuousall?fc0={(int)function[0]}&op0={(int)output[0]}&fc1={(int)function[1]}&op1={(int)output[1]}&fc2={(int)function[2]}&op2={(int)output[2]}&fc3={(int)function[3]}&op3={(int)output[3]}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends single PWM commands to all four channels simultaneously.
        /// </summary>
        /// <param name="pwm">Array of PWM speeds for channels 0-3.</param>
        /// <param name="output">Array of outputs for channels 0-3.</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool SinglePwmAll(PwmSpeed[] pwm, PwmOutput[] output)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/singlepwmall?pw0=0&op0=0&pw1=0&op1=0&pw2=0&op2=0&pw3=0&op3=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/continuousall?pw0={(int)pwm[0]}&op0={(int)output[0]}&pw1={(int)pwm[1]}&op1={(int)output[1]}&pw2={(int)pwm[2]}&op2={(int)output[2]}&pw3={(int)pwm[3]}&op3={(int)output[3]}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends combo PWM commands to all four channels simultaneously.
        /// </summary>
        /// <param name="pwmRed">Array of red PWM speeds for channels 0-3.</param>
        /// <param name="pwmBlue">Array of blue PWM speeds for channels 0-3.</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool ComboPwmAll(PwmSpeed[] pwmRed, PwmSpeed[] pwmBlue)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/combopwmall?pwr0=0&pwb0=0&pwr1=0&pwb1=0&pwr2=0&pwb2=0&pwr3=0&pwb3=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/continuousall?pwr0={(int)pwmRed[0]}&pwb0={(int)pwmBlue[0]}&pwr1={(int)pwmRed[1]}&pwb1={(int)pwmBlue[1]}&pwr2={(int)pwmRed[2]}&pwb2={(int)pwmBlue[2]}&pwr3={(int)pwmRed[3]}&pwb3={(int)pwmBlue[3]}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        /// <summary>
        /// Sends a combo PWM command to control two motors with PWM speeds.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="bl">PWM speed for blue motor.</param>
        /// <param name="rd">PWM speed for red motor.</param>
        /// <returns>True if the command was sent successfully, false otherwise.</returns>
        public bool ComboPwm(Channel ch, PwmSpeed bl, PwmSpeed rd)
        {
            var dev = _disco.DeviceDetails.Where(m => m.Id == DeviceIDType).FirstOrDefault();
            if (dev == null)
            {
                return false;
            }

            // http://192.168.1.85/combopwm?rd=0&bl=0&ch=0
            var ip = dev.IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/combopwm?rd={(int)rd}&bl={(int)bl}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }
    }
}
