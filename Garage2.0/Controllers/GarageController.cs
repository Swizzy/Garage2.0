using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Garage2._0.DAL;
using Garage2._0.Models;
using Garage2._0.ViewModels;
using PagedList;

namespace Garage2._0.Controllers
{
    public class GarageController : Controller
    {
        private VehiclesContext db = new VehiclesContext();

        // GET: Garage
        public ActionResult Index(int page = 1)
        {
            return View(db.Vehicles.OrderBy(v => v.Id).ToPagedList(page, 10));

        public ActionResult Statistics(string type = null)
        {
            switch (type)
            {
                case "typecolor":
                    var typeColorStats = new VehicleTypeColorStatistics();
                    foreach (var vehicle in db.Vehicles)
                    {
                        if (!typeColorStats.Statistics.ContainsKey(vehicle.Type))
                            typeColorStats.Statistics[vehicle.Type] = new Dictionary<Vehicle.VehicleColor, int>();
                        if (!typeColorStats.Statistics[vehicle.Type].ContainsKey(vehicle.Color))
                            typeColorStats.Statistics[vehicle.Type][vehicle.Color] = 0;
                        typeColorStats.Statistics[vehicle.Type][vehicle.Color] += 1;
                    }
                    return View("VehicleTypeColorStatistics", typeColorStats);
                case "color":
                    var colorStats = new VehicleColorStatistics();
                    foreach (var vehicle in db.Vehicles)
                    {
                        if (!colorStats.Statistics.ContainsKey(vehicle.Color))
                            colorStats.Statistics[vehicle.Color] = 0;
                        colorStats.Statistics[vehicle.Color] += 1;
                    }
                    return View("VehicleColorStatistics", colorStats);
                default:
                    var typeStats = new VehicleTypeStatistics();
                    foreach (var vehicle in db.Vehicles)
                    {
                        if (!typeStats.Statistics.ContainsKey(vehicle.Type))
                            typeStats.Statistics[vehicle.Type] = 0;
                        typeStats.Statistics[vehicle.Type] += 1;
                    }
                    return View("VehicleTypeStatistics", typeStats);
            }
        }

        // GET: Garage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // GET: Garage/Create
        public ActionResult Checkin()
        {
            return View();
        }

        // POST: Garage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkin([Bind(Include = "Id,Type,RegNumber,Color,Brand,Model,NumberOfWheels")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                vehicle.Timestamp = DateTime.Now;
                db.Vehicles.Add(vehicle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vehicle);
        }

        // GET: Garage/Delete/5
        public ActionResult Checkout(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        // POST: Garage/Delete/5
        [HttpPost, ActionName("Checkout")]
        [ValidateAntiForgeryToken]
        public ActionResult CheckoutConfirmed(int id)
        {
            Vehicle vehicle = db.Vehicles.Find(id);
            db.Vehicles.Remove(vehicle);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
