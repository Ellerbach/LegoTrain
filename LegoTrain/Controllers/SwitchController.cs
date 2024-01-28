// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using LegoTrain.Models;

using nanoDiscovery.Common;
using LegoTrain.Models.Device;

namespace LegoTrain.Controllers
{
    public class SwitchController : Controller
    {
        private readonly AppConfiguration _configuration;

        public SwitchController(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: SwitchController
        public ActionResult Index()
        {
            foreach (var device in _configuration.Switches)
            {
                var dev = _configuration.Discovery.DeviceDetails.Where(m => m.Id == device.Id).FirstOrDefault();
                if (dev != null
                    && dev.DeviceCapacity.HasFlag(DeviceCapability.Switch))
                {
                    device.IPAddress = dev.IPAddress.ToString();
                    device.IsConnected = dev.DeviceStatus == DeviceStatus.Joining;
                }
                else
                {
                    device.IPAddress = string.Empty;
                    device.IsConnected = false;
                }
            }

            return View(_configuration.Switches);
        }

        // GET: SwitchController/Details/5
        public ActionResult Details(int id)
        {
            var swt = _configuration.Switches.Where(m => m.Id == id).FirstOrDefault();
            if (swt == null)
            {
                return NotFound();
            }

            var dev = _configuration.Discovery.DeviceDetails.Where(m => m.Id == id).FirstOrDefault();
            if (dev != null &&
                dev.DeviceCapacity.HasFlag(DeviceCapability.Switch))
            {
                swt.IPAddress = dev.IPAddress.ToString();
                swt.IsConnected = dev.DeviceStatus == DeviceStatus.Joining;
            }
            else
            {
                swt.IPAddress = string.Empty;
                swt.IsConnected = false;
            }

            return View(swt);
        }

        // GET: SwitchController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SwitchController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Switch collection)
        {
            try
            {
                if (_configuration.Switches.Where(m => m.Id == collection.Id).Any())
                {
                    return ValidationProblem("Singal ID already exists, it must be unique");
                }

                if (collection.Id != -1)
                {
                    _configuration.Switches.Add(collection);
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

        // GET: SwitchController/Edit/5
        public ActionResult Edit(int id)
        {
            var swt = _configuration.Switches.Where(m => m.Id == id).FirstOrDefault();
            if (swt == null)
            {
                return NotFound();
            }

            return View(swt);
        }

        // POST: SwitchController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Switch collection)
        {
            try
            {
                var swt = _configuration.Switches.Find(m => m.Id == id);
                if (swt != null)
                {
                    _configuration.Switches.Remove(swt);
                    _configuration.Switches.Add(collection);
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

        // GET: SwitchController/Delete/5
        public ActionResult Delete(int id)
        {
            var swt = _configuration.Switches.Where(m => m.Id == id).FirstOrDefault();
            if (swt == null)
            {
                return NotFound();
            }

            return View(swt);
        }

        // POST: SwitchController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var swt = _configuration.Switches.Find(m => m.Id == id);
                if (swt != null)
                {
                    _configuration.Switches.Remove(swt);
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
