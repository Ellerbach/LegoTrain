// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LegoTrain.Models;

namespace LegoTrain.Controllers
{
    public class TrainController : Controller
    {
        private readonly AppConfiguration _configuration;

        public TrainController(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: TrainController
        public ActionResult Index()
        {
            return View(_configuration.Trains);
        }

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

        // GET: TrainController/Create
        public ActionResult Create()
        {
            // Convert the enumeration values to a SelectList
            ViewBag.ChannelOptions = new SelectList(Enum.GetValues(typeof(Lego.Infrared.Channel)));
            ViewBag.SpeedOptions = new SelectList(Enum.GetValues(typeof(Lego.Infrared.PwmSpeed)));
            ViewBag.OutputOptions = new SelectList(Enum.GetValues(typeof(Lego.Infrared.PwmOutput)));

            return View();
        }

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
