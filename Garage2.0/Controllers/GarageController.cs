using System;
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
        public ActionResult Index(string orderBy, string currentFilter, string searchString, string selectedvehicletype, int page = 1)
        {
            if (!db.GarageConfiguration.IsConfigured)
                return RedirectToAction("Index", "Setup");
            if (!db.Vehicles.Any())
                return RedirectToAction("Checkin");
            IQueryable<Vehicle> vehicles = db.Vehicles;

            if (!String.IsNullOrEmpty(selectedvehicletype))
            {
                Vehicle.VehicleType resulttype;
                if (Enum.TryParse(selectedvehicletype, out resulttype))
                {
                    vehicles = vehicles.Where(v => v.Type == resulttype);
                    ViewBag.selectedvehicletype = selectedvehicletype;
                }
            }

            if (searchString != null)
            {
                page = 1; // If the search string is changed during paging, the page has to be reset to 1
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
                case "type_dec":
                    vehicles = vehicles.OrderByDescending(v => v.Type);
                    break;
                case "regnumber":
                    vehicles = vehicles.OrderBy(v => v.RegNumber);
                    break;
                case "regnumber_dec":
                    vehicles = vehicles.OrderByDescending(v => v.RegNumber);
                    break;
                case "color":
                    vehicles = vehicles.OrderBy(v => v.Color);
                    break;
                case "color_dec":
                    vehicles = vehicles.OrderByDescending(v => v.Color);
                    break;
                case "spot":
                    vehicles = vehicles.OrderBy(v => v.ParkingUnit);
                    break;
                case "spot_dec":
                    vehicles = vehicles.OrderByDescending(v => v.ParkingUnit);
                    break;
                case "checkintime_dec":
                    vehicles = vehicles.OrderByDescending(v => v.Timestamp);
                    break;
                default:
                    vehicles = vehicles.OrderBy(v => v.Timestamp);
                    break;
            }
            ViewBag.CurrentSort = orderBy;

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
            var statistics = new Statistics(db);
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

        private IEnumerable<SelectListItem> GetSupportedList()
        {
            var maxUnits = db.GarageConfiguration.MaxUnits;
            var sizes = Enum.GetValues(typeof(Vehicle.VehicleType))
                            .Cast<Vehicle.VehicleType>()
                            .Select(Vehicle.GetUnitSpace)
                            .Distinct();
            var supportedSize = sizes.Where(s => FindFirstFreeUnit(s) + s <= maxUnits);
            var selectList = Enum.GetValues(typeof(Vehicle.VehicleType))
                                 .Cast<Vehicle.VehicleType>()
                                 .Where(t => supportedSize.Contains(Vehicle.GetUnitSpace(t)))
                                 .Select(e => new SelectListItem
                                 {
                                    Value = ((int) e).ToString(),
                                    Text = e.ToString()
                                 });
            return selectList;
        }

        // GET: Garage/Create
        public ActionResult Checkin()
        {
            if (!db.GarageConfiguration.IsConfigured)
                return RedirectToAction("Index", "Setup");
            if (!HasVacantSpots())
                return RedirectToAction("Index");
            ViewBag.SupportedList = GetSupportedList();
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
                vehicle.CheckoutTime = DateTime.Now;//Test

                var firstFreeUnit = FindFirstFreeUnit(vehicle.Units);
                if (firstFreeUnit + vehicle.Units > db.GarageConfiguration.MaxUnits) {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
            vehicle.CheckoutTime = DateTime.Now;
            //vehicle.Cost = Math.Round((decimal) (DateTime.Now - vehicle.Timestamp).TotalMinutes) * db.GarageConfiguration.PricePerMinute;
            vehicle.Cost = Math.Round((decimal)(vehicle.CheckoutTime - vehicle.Timestamp).TotalMinutes) * db.GarageConfiguration.PricePerMinute;
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

            var receiptViewModel = new ReceiptViewModel();

            //receiptViewModel.Update(vehicle, DateTime.Now, db.GarageConfiguration.PricePerMinute);
            receiptViewModel.Update(vehicle, db.GarageConfiguration.PricePerMinute);

            db.Vehicles.Remove(vehicle);
            db.SaveChanges();

            return View("Receipt", receiptViewModel);;
        }

        public ActionResult Overview(int page = 1)
        {
            if (!db.GarageConfiguration.IsConfigured)
                return RedirectToAction("Index", "Setup");
            var spots = new Overview[db.GarageConfiguration.ParkingSpaces];
            var orderedVehicles = db.Vehicles.OrderBy(v => v.ParkingUnit).AsEnumerable();
            for (int i = 0; i < spots.Length; i++)
            {
                if (spots[i] != null)
                    continue;
                var vehicle = orderedVehicles.FirstOrDefault(v => v.ParkingSpotId == i + 1);
                if (vehicle != null)
                {
                    if (vehicle.Units > 3)
                    {
                        for (int j = 0; j < vehicle.Units / 3; j++)
                            spots[i + j] = new Overview(i + j + 1, vehicle.Id);
                    }
                    else
                    {
                        spots[i] = new Overview(i + 1, vehicle.Id);
                    }
                }
                else
                {
                    spots[i] = new Overview(i + 1);
                }
            }
            
            return View(new PagedList<Overview>(spots, page, 100));
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

