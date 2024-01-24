
using Lego.Infrared;
using System.Net;
using System;

namespace LegoTrain.Services
{
    public class LegoInfraredExecutor
    {
        public const int DeviceIDType = 0;
        private static readonly HttpClient _client = new HttpClient();
        private LegoDiscovery _disco;

        public LegoInfraredExecutor(LegoDiscovery legoDiscovery)
        {
            _disco = legoDiscovery;
        }

        public bool Combo(Channel ch, Speed bl, Speed rd)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/combo?rd=0&bl=0&ch=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/combo?rd={(int)rd}&bl={(int)bl}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        public bool SinglePwm(Channel ch, PwmSpeed pw, PwmOutput op)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/singlepwm?pw=0&op=0&ch=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/singlepwm?pw={(int)pw}&op={(int)op}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        public bool Continuous(Function fc, Output op)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/continuous?fc=0&op=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/continuous?fc={(int)fc}&op={(int)op}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        public bool SingleCst(Channel ch, ClearSetToggle pw, PwmOutput op)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/singlecst?pw=0&op=0&ch=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/singlecst?pw={(int)pw}&op={(int)op}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        public bool Timeout(Channel ch, Function fc, Output op)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/timeout?fc=0&op=0&ch=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/timeout?fc={(int)fc}&op={(int)op}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        public bool ComboAll(Speed[] comboBlue, Speed[] comboRed)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/comboall?rd0=0&bl0=0&rd1=0&bl1=0&rd2=0&bl2=0&rd3=0&bl3=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/comboall?rd0={(int)comboRed[0]}&bl0={(int)comboBlue[0]}&rd1={(int)comboRed[1]}&bl1={(int)comboBlue[1]}&rd2={(int)comboRed[2]}&bl2={(int)comboBlue[2]}&rd3={(int)comboRed[3]}&bl3={(int)comboBlue[3]}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        public bool ContinuousAll(Function[] function, Output[] output)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/continuousall?fc0=0&op0=0&fc1=0&op1=0&fc2=0&op2=0&fc3=0&op3=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/continuousall?fc0={(int)function[0]}&op0={(int)output[0]}&fc1={(int)function[1]}&op1={(int)output[1]}&fc2={(int)function[2]}&op2={(int)output[2]}&fc3={(int)function[3]}&op3={(int)output[3]}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        public bool SinglePwmAll(PwmSpeed[] pwm, PwmOutput[] output)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/singlepwmall?pw0=0&op0=0&pw1=0&op1=0&pw2=0&op2=0&pw3=0&op3=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/continuousall?pw0={(int)pwm[0]}&op0={(int)output[0]}&pw1={(int)pwm[1]}&op1={(int)output[1]}&pw2={(int)pwm[2]}&op2={(int)output[2]}&pw3={(int)pwm[3]}&op3={(int)output[3]}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        public bool ComboPwmAll(PwmSpeed[] pwmRed, PwmSpeed[] pwmBlue)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/combopwmall?pwr0=0&pwb0=0&pwr1=0&pwb1=0&pwr2=0&pwb2=0&pwr3=0&pwb3=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/continuousall?pwr0={(int)pwmRed[0]}&pwb0={(int)pwmBlue[0]}&pwr1={(int)pwmRed[1]}&pwb1={(int)pwmBlue[1]}&pwr2={(int)pwmRed[2]}&pwb2={(int)pwmBlue[2]}&pwr3={(int)pwmRed[3]}&pwb3={(int)pwmBlue[3]}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }

        public bool ComboPwm(Channel ch, PwmSpeed bl, PwmSpeed rd)
        {
            if (!_disco.DeviceDetails.ContainsKey(DeviceIDType))
            {
                return false;
            }

            // http://192.168.1.85/combopwm?rd=0&bl=0&ch=0
            var ip = _disco.DeviceDetails[DeviceIDType].IPAddress.ToString();
            var res = _client.GetAsync($"http://{ip}/combopwm?rd={(int)rd}&bl={(int)bl}&ch={(int)ch}").GetAwaiter().GetResult();
            return res.IsSuccessStatusCode;
        }
    }
}
