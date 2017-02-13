﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
        private GarageContext db = new GarageContext();

        // GET: Garage
        public ActionResult Index(string orderBy, string currentFilter, string searchString, int page = 1)
        {
            if (!db.GarageConfiguration.IsConfigured)
                return RedirectToAction("Index", "Setup");
            IQueryable<Vehicle> vehicles = db.Vehicles;

            if (searchString != null)
            {
                // If the search string is changed during paging, the page has to be reset to 1
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                vehicles = vehicles.Where(v => v.RegNumber.Contains(searchString));
            }

            switch (orderBy)
            {
                case "type":
                    vehicles = vehicles.OrderBy(v => v.Type);
                    break;
                case "regnumber":
                    vehicles = vehicles.OrderBy(v => v.RegNumber);
                    break;
                case "color":
                    vehicles = vehicles.OrderBy(v => v.Color);
                    break;
                default:
                    vehicles = vehicles.OrderBy(v => v.Id);
                    break;
            }
            HasVacantSpots();

            return View(vehicles.ToPagedList(page, 10));
        }

        private bool HasVacantSpots() {
            var total = db.GarageConfiguration.ParkingSpaces;
            var vacant = (int)Math.Ceiling((total * 3 - db.Vehicles.ToArray().Sum(v => v.Units)) / 3.0);
            ViewBag.Vacant = $"Vacant parking spots: {vacant}/{total}";
            ViewBag.HasVacantSpots = vacant > 0;
            return ViewBag.HasVacantSpots;
        }

        public ActionResult Statistics()
        {
            if (!db.GarageConfiguration.IsConfigured)
                return RedirectToAction("Index", "Setup");
            var now = DateTime.Now;
            var statistics = new Statistics();
            foreach (var vehicle in db.Vehicles)
            {
                statistics.Update(vehicle, now, db.GarageConfiguration.PricePerMinute);
            }
            return View(statistics);
        }

        // GET: Garage/Details/5
        public ActionResult Details(int? id)
        {
            if (!db.GarageConfiguration.IsConfigured)
                return RedirectToAction("Index", "Setup");
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

        private long FindNextFreeUnit(long lastFreeUnit, IEnumerator<Vehicle> enumerator) {
            if (lastFreeUnit < enumerator.Current.ParkingUnit)
                return lastFreeUnit;
            lastFreeUnit += enumerator.Current.Units;
            while (enumerator.MoveNext()) {
                if (lastFreeUnit < enumerator.Current.ParkingUnit)
                    return lastFreeUnit;
                lastFreeUnit = enumerator.Current.ParkingUnit + enumerator.Current.Units;
            }
            return lastFreeUnit;
        }

        private long FindFirstFreeUnit(int size) {
            using (var enumerator = db.Vehicles.OrderBy(v => v.ParkingUnit).GetEnumerator()) {
                var first = 0L;
                while (enumerator.MoveNext()) {
                    first = FindNextFreeUnit(first, enumerator);
                    if (size == 1)
                        return first;
                    if (first % 3 == 0) {
                        if (enumerator.Current == null)
                            return first;
                        if (enumerator.Current.ParkingUnit >= first + size)
                            return first;
                        first += 3; // Find next available spot
                    }
                    else {
                        first -= first % 3;
                        first += enumerator.Current?.Units ?? 3;
                        first += 3;
                    }
                }
                return first;
            }

        }
        // GET: Garage/Create
        public ActionResult Checkin()
        {
            if (!db.GarageConfiguration.IsConfigured)
                return RedirectToAction("Index", "Setup");
            if (!HasVacantSpots())
                return RedirectToAction("Index");
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

                var firstFreeUnit = FindFirstFreeUnit(vehicle.Units);
                if (firstFreeUnit + vehicle.Units >= db.GarageConfiguration.MaxUnits) {
                    //TODO: Handle this error
                }
                vehicle.ParkingUnit = firstFreeUnit;
                db.Vehicles.Add(vehicle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vehicle);
        }

        // GET: Garage/Delete/5
        public ActionResult Checkout(int? id)
        {
            if (!db.GarageConfiguration.IsConfigured)
                return RedirectToAction("Index", "Setup");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vehicle vehicle = db.Vehicles.Find(id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            vehicle.Cost = Math.Round((decimal) (DateTime.Now - vehicle.Timestamp).TotalMinutes) * db.GarageConfiguration.PricePerMinute;
            db.Vehicles.AddOrUpdate(v => v.Id, vehicle);
            db.SaveChanges();
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
            return View("Receipt", vehicle);
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

