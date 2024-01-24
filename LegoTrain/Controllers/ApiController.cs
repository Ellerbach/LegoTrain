// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Text;
using LegoTrain.Models;
using LegoTrain.Services;
using Lego.Infrared;

namespace LegoTrain.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        public const string PageCombo = "combo";
        public const string PageSinglePwm = "singlepwm";
        public const string PageContinuous = "continuous";
        public const string PageSingleCst = "singlecst";
        public const string PageTimeout = "timeout";
        public const string PageComboAll = "comboall";
        public const string PageContinuousAll = "continuousall";
        public const string PageSinglePwmAll = "singlepwmall";
        public const string PageComboPwm = "combopwm";
        public const string PageComboPwmAll = "combopwmall";

        private readonly ILogger<ApiController> _logger;
        private readonly AppConfiguration _config;

        public ApiController(ILogger<ApiController> logger, AppConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
        }

        [HttpGet(PageCombo)]
        public IActionResult Combo(Channel ch, Speed bl, Speed rd)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            var res = _config.LegoInfraredExecutor.Combo(ch, bl, rd);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(PageSinglePwm)]
        public IActionResult SinglePwm(Channel ch, PwmSpeed pw, PwmOutput op)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            var res = _config.LegoInfraredExecutor.SinglePwm(ch, pw, op);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(PageContinuous)]
        public IActionResult Continuous(Function fc, Output op)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            var res = _config.LegoInfraredExecutor.Continuous(fc, op);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(PageSingleCst)]
        public IActionResult SingleCst(Channel ch, ClearSetToggle pw, PwmOutput op)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            var res = _config.LegoInfraredExecutor.SingleCst(ch, pw, op);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(PageTimeout)]
        public IActionResult SingleTimeout(Channel ch, Function fc, Output op)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            var res = _config.LegoInfraredExecutor.Timeout(ch, fc, op);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(PageComboAll)]
        public IActionResult ComboAll(Speed rd0, Speed bl0, Speed rd1, Speed bl1, Speed rd2, Speed bl2, Speed rd3, Speed bl3)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            Speed[] mComboBlue = new Speed[4] { bl0, bl1, bl2, bl3 };
            Speed[] mComboRed = new Speed[4] { rd0, rd1, rd2, rd3 };
            var res = _config.LegoInfraredExecutor.ComboAll(mComboBlue, mComboRed);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(PageContinuousAll)]
        public IActionResult ContinuousAll(Function fc0, Output op0, Function fc1, Output op1, Function fc2, Output op2, Function fc3, Output op3)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            Function[] mFunction = new Function[4] { fc0, fc1, fc2, fc3 };
            Output[] mOutPut = new Output[4] { op0, op1, op2, op3 };
            var res = _config.LegoInfraredExecutor.ContinuousAll(mFunction, mOutPut);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(PageSinglePwmAll)]
        public IActionResult SinglePwmAll(PwmSpeed pw0, PwmOutput op0, PwmSpeed pw1, PwmOutput op1, PwmSpeed pw2, PwmOutput op2, PwmSpeed pw3, PwmOutput op3)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            PwmSpeed[] mPWM = new PwmSpeed[4] { pw0, pw1, pw2, pw3 };
            PwmOutput[] mOutPut = new PwmOutput[4] { op0, op1, op2, op3 };
            var res = _config.LegoInfraredExecutor.SinglePwmAll(mPWM, mOutPut);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(PageComboPwmAll)]
        public IActionResult ComboPwmAll(PwmSpeed pwr0, PwmSpeed pwb0, PwmSpeed pwr1, PwmSpeed pwb1, PwmSpeed pwr2, PwmSpeed pwb2, PwmSpeed pwr3, PwmSpeed pwb3)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            PwmSpeed[] mPWMR = new PwmSpeed[4] { pwr0, pwr1, pwr2, pwr3 };
            PwmSpeed[] mPWMB = new PwmSpeed[4] { pwb0, pwb1, pwb2, pwb3 };
            var res = _config.LegoInfraredExecutor.ComboPwmAll(mPWMR, mPWMB);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(PageComboPwm)]
        public IActionResult ComboPwm(Channel ch, PwmSpeed p1, PwmSpeed p2)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            var res = _config.LegoInfraredExecutor.ComboPwm(ch, p1, p2);
            return res ? Ok() : BadRequest();
        }

        [HttpGet(nameof(Signal))]
        public IActionResult Signal(byte si, int md)
        {
            if (_config.SignalManagement == null)
            {
                return BadRequest();
            }

            try
            {
                _config.SignalManagement.ChangeSignal(si, (SignalState)md);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet(nameof(SignalStatus))]
        public IActionResult SignalStatus()
        {
            if (_config.SignalManagement == null)
            {
                return BadRequest();
            }

            string strResp = string.Empty;
            foreach (var device in _config.Discovery.DeviceDetails)
            {
                if (device.Value.DeviceCapacity.HasFlag(Models.Device.DeviceCapability.Signal))
                {
                    strResp += $"{device.Key}={(int)_config.SignalManagement.GetSignal((byte)device.Key)};";
                }
            }

            return Ok(strResp.TrimEnd(';'));
        }

        [HttpGet(nameof(SwitchStatus))]
        public IActionResult SwitchStatus()
        {
            if (_config.SwitchManagement == null)
            {
                return BadRequest();
            }

            string strResp = string.Empty;
            foreach (var device in _config.Discovery.DeviceDetails)
            {
                if (device.Value.DeviceCapacity.HasFlag(Models.Device.DeviceCapability.Switch))
                {
                    strResp += $"{device.Key}={(_config.SwitchManagement.GetSwitch((byte)device.Key) ? "1" : "0")};";
                }
            }

            return Ok(strResp.TrimEnd(';'));
        }

        [HttpGet(nameof(Switch))]
        public IActionResult Switch(byte si, bool md)
        {
            if (_config.SwitchManagement == null)
            {
                return BadRequest();
            }

            try
            {
                _config.SwitchManagement.ChangeSwitch(si, md);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}