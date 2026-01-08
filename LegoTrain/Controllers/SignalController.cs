// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using LegoTrain.Models;
using LegoTrain.Services;
using Iot.Device.FtCommon;
using LegoTrain.Models.Device;
using nanoDiscovery.Common;

namespace LegoTrain.Controllers
{
    /// <summary>
    /// Controller for managing signal devices on the Lego train circuit.
    /// </summary>
    public class SignalController : Controller
    {
        private readonly AppConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public SignalController(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Displays a list of all configured signals with their connection status.
        /// </summary>
        /// <returns>The index view with the list of signals.</returns>
        // GET: SignalController
        public ActionResult Index()
        {
            foreach (var device in _configuration.Signals)
            {
                var dev = _configuration.Discovery.DeviceDetails.Where(m => m.Id == device.Id).FirstOrDefault();
                if (dev != null
                    && dev.DeviceCapacity.HasFlag(DeviceCapability.Signal))
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

            return View(_configuration.Signals);
        }

        /// <summary>
        /// Displays detailed information about a specific signal.
        /// </summary>
        /// <param name="id">The signal identifier.</param>
        /// <returns>The details view for the signal, or NotFound if the signal doesn't exist.</returns>
        // GET: SignalController/Details/5
        public ActionResult Details(int id)
        {
            var sig = _configuration.Signals.Where(m => m.Id == id).FirstOrDefault();
            if (sig == null)
            {
                return NotFound();
            }

            var dev = _configuration.Discovery.DeviceDetails.Where(m => m.Id == id).FirstOrDefault();
            if (dev != null &&
                dev.DeviceCapacity.HasFlag(DeviceCapability.Signal))
            {
                sig.IPAddress = dev.IPAddress.ToString();
                sig.IsConnected = dev.DeviceStatus == DeviceStatus.Joining;
            }
            else
            {
                sig.IPAddress = string.Empty;
                sig.IsConnected = false;
            }

            return View(sig);
        }

        /// <summary>
        /// Displays the form for creating a new signal.
        /// </summary>
        /// <param name="id">The signal identifier.</param>
        /// <returns>The create view with a new signal.</returns>
        // GET: SignalController/Create
        public ActionResult Create(int id)
        {
            Signal sig = new Signal { Id = id };
            return View(sig);
        }

        /// <summary>
        /// Processes the creation of a new signal.
        /// </summary>
        /// <param name="collection">The signal data to create.</param>
        /// <returns>Redirects to Index on success, or returns the view with validation errors.</returns>
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

        /// <summary>
        /// Displays the form for editing an existing signal.
        /// </summary>
        /// <param name="id">The signal identifier.</param>
        /// <returns>The edit view for the signal, or NotFound if the signal doesn't exist.</returns>
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

        /// <summary>
        /// Processes the editing of an existing signal.
        /// </summary>
        /// <param name="id">The signal identifier.</param>
        /// <param name="collection">The updated signal data.</param>
        /// <returns>Redirects to Index on success, or returns the view with validation errors.</returns>
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

        /// <summary>
        /// Displays the confirmation page for deleting a signal.
        /// </summary>
        /// <param name="id">The signal identifier.</param>
        /// <returns>The delete view for the signal, or NotFound if the signal doesn't exist.</returns>
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

        /// <summary>
        /// Processes the deletion of a signal.
        /// </summary>
        /// <param name="id">The signal identifier.</param>
        /// <returns>Redirects to Index on success, or returns the view on error.</returns>
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
