// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using LegoTrain.Models;
using LegoTrain.Services;
using Lego.Infrared;
using nanoDiscovery.Common;

namespace LegoTrain.Controllers
{
    /// <summary>
    /// API controller for controlling trains, signals, and switches on the Lego train system.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        /// <summary>
        /// Route for combo command.
        /// </summary>
        public const string PageCombo = "combo";
        /// <summary>
        /// Route for single PWM command.
        /// </summary>
        public const string PageSinglePwm = "singlepwm";
        /// <summary>
        /// Route for continuous command.
        /// </summary>
        public const string PageContinuous = "continuous";
        /// <summary>
        /// Route for single CST command.
        /// </summary>
        public const string PageSingleCst = "singlecst";
        /// <summary>
        /// Route for timeout command.
        /// </summary>
        public const string PageTimeout = "timeout";
        /// <summary>
        /// Route for combo all command.
        /// </summary>
        public const string PageComboAll = "comboall";
        /// <summary>
        /// Route for continuous all command.
        /// </summary>
        public const string PageContinuousAll = "continuousall";
        /// <summary>
        /// Route for single PWM all command.
        /// </summary>
        public const string PageSinglePwmAll = "singlepwmall";
        /// <summary>
        /// Route for combo PWM command.
        /// </summary>
        public const string PageComboPwm = "combopwm";
        /// <summary>
        /// Route for combo PWM all command.
        /// </summary>
        public const string PageComboPwmAll = "combopwmall";
        /// <summary>
        /// Route for detect command.
        /// </summary>
        public const string PageDetect = "detect";

        private readonly ILogger<ApiController> _logger;
        private readonly AppConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The application configuration.</param>
        public ApiController(ILogger<ApiController> logger, AppConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
        }

        /// <summary>
        /// Handles detector events.
        /// </summary>
        /// <param name="id">The detector ID.</param>
        /// <param name="de">The detector state.</param>
        /// <param name="va">The detector value.</param>
        /// <returns>An OK result.</returns>
        [HttpGet(PageDetect)]
        public IActionResult Detect(int id, int de, int va)
        {
            if (_config.DetectorManagement == null)
            {
                return BadRequest();
            }

            if (id < 0 || de < 0 || va < 0)
            {
                return BadRequest();
            }

            var detectorManager = (DetectorManagement)_config.DetectorManagement;
            detectorManager.UpdateDetectorState(id, de, va, true);
            return Ok();
        }

        /// <summary>
        /// Sends a combo command to control two motors on the specified channel.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="bl">The blue motor speed.</param>
        /// <param name="rd">The red motor speed.</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
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

        /// <summary>
        /// Sends a single PWM command to control a motor.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="pw">The PWM speed.</param>
        /// <param name="op">The output (Red or Blue).</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
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

        /// <summary>
        /// Sends a continuous function command.
        /// </summary>
        /// <param name="fc">The function.</param>
        /// <param name="op">The output.</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
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

        /// <summary>
        /// Sends a single clear/set/toggle command.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="pw">The clear/set/toggle value.</param>
        /// <param name="op">The output.</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
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

        /// <summary>
        /// Sends a timeout function command.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="fc">The function.</param>
        /// <param name="op">The output.</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
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

        /// <summary>
        /// Sends combo commands to all four channels simultaneously.
        /// </summary>
        /// <param name="rd0">Red motor speed for channel 0.</param>
        /// <param name="bl0">Blue motor speed for channel 0.</param>
        /// <param name="rd1">Red motor speed for channel 1.</param>
        /// <param name="bl1">Blue motor speed for channel 1.</param>
        /// <param name="rd2">Red motor speed for channel 2.</param>
        /// <param name="bl2">Blue motor speed for channel 2.</param>
        /// <param name="rd3">Red motor speed for channel 3.</param>
        /// <param name="bl3">Blue motor speed for channel 3.</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
        [HttpGet(PageComboAll)]
        public IActionResult ComboAll(Speed rd0, Speed bl0, Speed rd1, Speed bl1, Speed rd2, Speed bl2, Speed rd3, Speed bl3)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            Speed[] mComboBlue = [bl0, bl1, bl2, bl3];
            Speed[] mComboRed = [rd0, rd1, rd2, rd3];
            var res = _config.LegoInfraredExecutor.ComboAll(mComboBlue, mComboRed);
            return res ? Ok() : BadRequest();
        }

        /// <summary>
        /// Sends continuous function commands to all four channels simultaneously.
        /// </summary>
        /// <param name="fc0">Function for channel 0.</param>
        /// <param name="op0">Output for channel 0.</param>
        /// <param name="fc1">Function for channel 1.</param>
        /// <param name="op1">Output for channel 1.</param>
        /// <param name="fc2">Function for channel 2.</param>
        /// <param name="op2">Output for channel 2.</param>
        /// <param name="fc3">Function for channel 3.</param>
        /// <param name="op3">Output for channel 3.</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
        [HttpGet(PageContinuousAll)]
        public IActionResult ContinuousAll(Function fc0, Output op0, Function fc1, Output op1, Function fc2, Output op2, Function fc3, Output op3)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            Function[] mFunction = [fc0, fc1, fc2, fc3];
            Output[] mOutPut = [op0, op1, op2, op3];
            var res = _config.LegoInfraredExecutor.ContinuousAll(mFunction, mOutPut);
            return res ? Ok() : BadRequest();
        }

        /// <summary>
        /// Sends single PWM commands to all four channels simultaneously.
        /// </summary>
        /// <param name="pw0">PWM speed for channel 0.</param>
        /// <param name="op0">Output for channel 0.</param>
        /// <param name="pw1">PWM speed for channel 1.</param>
        /// <param name="op1">Output for channel 1.</param>
        /// <param name="pw2">PWM speed for channel 2.</param>
        /// <param name="op2">Output for channel 2.</param>
        /// <param name="pw3">PWM speed for channel 3.</param>
        /// <param name="op3">Output for channel 3.</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
        [HttpGet(PageSinglePwmAll)]
        public IActionResult SinglePwmAll(PwmSpeed pw0, PwmOutput op0, PwmSpeed pw1, PwmOutput op1, PwmSpeed pw2, PwmOutput op2, PwmSpeed pw3, PwmOutput op3)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            PwmSpeed[] mPWM = [pw0, pw1, pw2, pw3];
            PwmOutput[] mOutPut = [op0, op1, op2, op3];
            var res = _config.LegoInfraredExecutor.SinglePwmAll(mPWM, mOutPut);
            return res ? Ok() : BadRequest();
        }

        /// <summary>
        /// Sends combo PWM commands to all four channels simultaneously.
        /// </summary>
        /// <param name="pwr0">Red PWM speed for channel 0.</param>
        /// <param name="pwb0">Blue PWM speed for channel 0.</param>
        /// <param name="pwr1">Red PWM speed for channel 1.</param>
        /// <param name="pwb1">Blue PWM speed for channel 1.</param>
        /// <param name="pwr2">Red PWM speed for channel 2.</param>
        /// <param name="pwb2">Blue PWM speed for channel 2.</param>
        /// <param name="pwr3">Red PWM speed for channel 3.</param>
        /// <param name="pwb3">Blue PWM speed for channel 3.</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
        [HttpGet(PageComboPwmAll)]
        public IActionResult ComboPwmAll(PwmSpeed pwr0, PwmSpeed pwb0, PwmSpeed pwr1, PwmSpeed pwb1, PwmSpeed pwr2, PwmSpeed pwb2, PwmSpeed pwr3, PwmSpeed pwb3)
        {
            if (_config.LegoInfraredExecutor == null)
            {
                return BadRequest();
            }

            PwmSpeed[] mPWMR = [pwr0, pwr1, pwr2, pwr3];
            PwmSpeed[] mPWMB = [pwb0, pwb1, pwb2, pwb3];
            var res = _config.LegoInfraredExecutor.ComboPwmAll(mPWMR, mPWMB);
            return res ? Ok() : BadRequest();
        }

        /// <summary>
        /// Sends a combo PWM command to control two motors with PWM speeds.
        /// </summary>
        /// <param name="ch">The channel.</param>
        /// <param name="p1">PWM speed for first motor.</param>
        /// <param name="p2">PWM speed for second motor.</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
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

        /// <summary>
        /// Changes the state of a signal.
        /// </summary>
        /// <param name="si">The signal identifier.</param>
        /// <param name="md">The signal mode (state).</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
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

        /// <summary>
        /// Gets the status of all configured signals.
        /// </summary>
        /// <returns>OK with a string containing signal statuses, or BadRequest.</returns>
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
                if (device.DeviceCapacity.HasFlag(DeviceCapability.Signal))
                {
                    strResp += $"{device.Id}={(int)_config.SignalManagement.GetSignal((byte)device.Id)};";
                }
            }

            return Ok(strResp.TrimEnd(';'));
        }

        /// <summary>
        /// Gets the status of all configured switches.
        /// </summary>
        /// <returns>OK with a string containing switch statuses, or BadRequest.</returns>
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
                if (device.DeviceCapacity.HasFlag(DeviceCapability.Switch))
                {
                    strResp += $"{device.Id}={(_config.SwitchManagement.GetSwitch((byte)device.Id) ? "1" : "0")};";
                }
            }

            return Ok(strResp.TrimEnd(';'));
        }

        /// <summary>
        /// Changes the state of a switch.
        /// </summary>
        /// <param name="si">The switch identifier.</param>
        /// <param name="md">The switch mode (on/off).</param>
        /// <returns>OK if successful, BadRequest otherwise.</returns>
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