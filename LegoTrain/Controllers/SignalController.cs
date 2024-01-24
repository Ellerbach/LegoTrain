// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using LegoTrain.Models;
using LegoTrain.Services;
using Iot.Device.FtCommon;

namespace LegoTrain.Controllers
{
    public class SignalController : Controller
    {
        private readonly AppConfiguration _configuration;

        public SignalController(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: SignalController
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

            return View(_configuration.Signals);
        }

        // GET: SignalController/Details/5
        public ActionResult Details(int id)
        {
            var sig = _configuration.Signals.Where(m => m.Id == id).FirstOrDefault();
            if (sig == null)
            {
                return NotFound();
            }

            if (_configuration.Discovery.DeviceDetails.ContainsKey(id) && 
                _configuration.Discovery.DeviceDetails[id].DeviceCapacity.HasFlag(Models.Device.DeviceCapability.Signal))
            {                
                sig.IPAddress = _configuration.Discovery.DeviceDetails[id].IPAddress.ToString();
                sig.IsConnected = _configuration.Discovery.DeviceDetails[id].DeviceStatus == Models.Device.DeviceStatus.Joining;
            }
            else
            {
                sig.IPAddress = string.Empty;
                sig.IsConnected = false;
            }

            return View(sig);
        }

        // GET: SignalController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SignalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Signal collection)
        {
            try
            {
                if (_configuration.Signals.Where(m => m.Id == collection.Id).Any())
                {
                    return ValidationProblem("Singal ID already exists, it must be unique");
                }

                if (collection.Id != -1)
                {
                    _configuration.Signals.Add(collection);
                    _configuration.Save();
                    return RedirectToAction(nameof(Index));
                }

                return NotFound();
            }
            catch
            {
            }

            return View();
        }

        // GET: SignalController/Edit/5
        public ActionResult Edit(int id)
        {
            var sig = _configuration.Signals.Where(m => m.Id == id).FirstOrDefault();
            if (sig == null)
            {
                return NotFound();
            }

            return View(sig);
        }

        // POST: SignalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Signal collection)
        {
            try
            {
                var sig = _configuration.Signals.Find(m => m.Id == id);
                if (sig != null)
                {
                    _configuration.Signals.Remove(sig);
                    _configuration.Signals.Add(collection);
                    _configuration.Save();
                    return RedirectToAction(nameof(Index));
                }

                return NotFound();
            }
            catch
            {
            }

            return View();
        }

        // GET: SignalController/Delete/5
        public ActionResult Delete(int id)
        {
            var sig = _configuration.Signals.Where(m => m.Id == id).FirstOrDefault();
            if (sig == null)
            {
                return NotFound();
            }

            return View(sig);
        }

        // POST: SignalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var sig = _configuration.Signals.Find(m => m.Id == id);
                if (sig != null)
                {
                    _configuration.Signals.Remove(sig);
                    _configuration.Save();
                    return RedirectToAction(nameof(Index));
                }

                return NotFound();
            }
            catch
            {
            }

            return View();
        }
    }
}
