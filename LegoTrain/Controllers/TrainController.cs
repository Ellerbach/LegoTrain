// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LegoTrain.Models;

namespace LegoTrain.Controllers
{
    /// <summary>
    /// Controller for managing train configurations on the Lego train circuit.
    /// </summary>
    public class TrainController : Controller
    {
        private readonly AppConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainController"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public TrainController(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Displays a list of all configured trains.
        /// </summary>
        /// <returns>The index view with the list of trains.</returns>
        // GET: TrainController
        public ActionResult Index()
        {
            return View(_configuration.Trains);
        }

        /// <summary>
        /// Displays detailed information about a specific train.
        /// </summary>
        /// <param name="id">The train identifier.</param>
        /// <returns>The details view for the train, or NotFound if the train doesn't exist.</returns>
        // GET: TrainController/Details/5
        public ActionResult Details(int id)
        {
            var train = _configuration.Trains.Where(m => m.Id == id).FirstOrDefault();
            if (train == null)
            {
                return NotFound();
            }

            return View(train);
        }

        /// <summary>
        /// Displays the form for creating a new train.
        /// </summary>
        /// <returns>The create view.</returns>
        // GET: TrainController/Create
        public ActionResult Create()
        {
            // Convert the enumeration values to a SelectList
            ViewBag.ChannelOptions = new SelectList(Enum.GetValues(typeof(Lego.Infrared.Channel)));
            ViewBag.SpeedOptions = new SelectList(Enum.GetValues(typeof(Lego.Infrared.PwmSpeed)));
            ViewBag.OutputOptions = new SelectList(Enum.GetValues(typeof(Lego.Infrared.PwmOutput)));

            return View();
        }

        /// <summary>
        /// Processes the creation of a new train.
        /// </summary>
        /// <param name="collection">The train data to create.</param>
        /// <returns>Redirects to Index on success, or returns the view with validation errors.</returns>
        // POST: TrainController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Train collection)
        {
            try
            {
                // Find an available index
                int id = -1;
                for (int i = 0; i < Train.MaximumNumberOfTrains; i++)
                {
                    var train = _configuration.Trains.Find(m => m.Id == i);
                    if (train == null)
                    {
                        id = i;
                        break;
                    }
                }

                if (id != -1)
                {
                    collection.Id = id;
                    _configuration.Trains.Add(collection);
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
        /// Displays the form for editing an existing train.
        /// </summary>
        /// <param name="id">The train identifier.</param>
        /// <returns>The edit view for the train, or NotFound if the train doesn't exist.</returns>
        // GET: TrainController/Edit/5
        public ActionResult Edit(int id)
        {
            // Convert the enumeration values to a SelectList
            ViewBag.ChannelOptions = new SelectList(Enum.GetValues(typeof(Lego.Infrared.Channel)));
            ViewBag.SpeedOptions = new SelectList(Enum.GetValues(typeof(Lego.Infrared.PwmSpeed)));
            ViewBag.OutputOptions = new SelectList(Enum.GetValues(typeof(Lego.Infrared.PwmOutput)));

            var train = _configuration.Trains.Where(m => m.Id == id).FirstOrDefault();
            if (train == null)
            {
                return NotFound();
            }

            return View(train);
        }

        /// <summary>
        /// Processes the editing of an existing train.
        /// </summary>
        /// <param name="id">The train identifier.</param>
        /// <param name="collection">The updated train data.</param>
        /// <returns>Redirects to Index on success, or returns the view with validation errors.</returns>
        // POST: TrainController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Train collection)
        {
            try
            {
                var train = _configuration.Trains.Find(m => m.Id == id);
                if (train != null)
                {
                    _configuration.Trains.Remove(train);
                    _configuration.Trains.Add(collection);
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
        /// Displays the confirmation page for deleting a train.
        /// </summary>
        /// <param name="id">The train identifier.</param>
        /// <returns>The delete view for the train, or NotFound if the train doesn't exist.</returns>
        // GET: TrainController/Delete/5
        public ActionResult Delete(int id)
        {
            var train = _configuration.Trains.Where(m => m.Id == id).FirstOrDefault();
            if (train == null)
            {
                return NotFound();
            }

            return View(train);
        }

        /// <summary>
        /// Processes the deletion of a train.
        /// </summary>
        /// <param name="id">The train identifier.</param>
        /// <returns>Redirects to Index on success, or returns the view on error.</returns>
        // POST: TrainController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var train = _configuration.Trains.Find(m => m.Id == id);
                if (train != null)
                {
                    _configuration.Trains.Remove(train);
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
