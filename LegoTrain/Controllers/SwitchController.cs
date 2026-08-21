// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using LegoTrain.Models;

using nanoDiscovery.Common;
using LegoTrain.Models.Device;

namespace LegoTrain.Controllers
{
    /// <summary>
    /// Controller for managing track switch devices on the Lego train circuit.
    /// </summary>
    public class SwitchController : Controller
    {
        private readonly AppConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public SwitchController(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Displays a list of all configured switches with their connection status.
        /// </summary>
        /// <returns>The index view with the list of switches.</returns>
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

        /// <summary>
        /// Displays detailed information about a specific switch.
        /// </summary>
        /// <param name="id">The switch identifier.</param>
        /// <returns>The details view for the switch, or NotFound if the switch doesn't exist.</returns>
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

        /// <summary>
        /// Displays the form for creating a new switch.
        /// </summary>
        /// <param name="id">The switch identifier.</param>
        /// <returns>The create view with a new switch.</returns>
        // GET: SwitchController/Create
        public ActionResult Create(int id)
        {
            Switch swt = new Switch() { Id = id };
            return View(swt);
        }

        /// <summary>
        /// Processes the creation of a new switch.
        /// </summary>
        /// <param name="collection">The switch data to create.</param>
        /// <returns>Redirects to Index on success, or returns the view with validation errors.</returns>
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

        /// <summary>
        /// Displays the form for editing an existing switch.
        /// </summary>
        /// <param name="id">The switch identifier.</param>
        /// <returns>The edit view for the switch, or NotFound if the switch doesn't exist.</returns>
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

        /// <summary>
        /// Processes the editing of an existing switch.
        /// </summary>
        /// <param name="id">The switch identifier.</param>
        /// <param name="collection">The updated switch data.</param>
        /// <returns>Redirects to Index on success, or returns the view with validation errors.</returns>
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

        /// <summary>
        /// Displays the confirmation page for deleting a switch.
        /// </summary>
        /// <param name="id">The switch identifier.</param>
        /// <returns>The delete view for the switch, or NotFound if the switch doesn't exist.</returns>
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

        /// <summary>
        /// Processes the deletion of a switch.
        /// </summary>
        /// <param name="id">The switch identifier.</param>
        /// <returns>Redirects to Index on success, or returns the view on error.</returns>
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
