using System.Linq;
using System.Web.Mvc;
using Garage2._0.DAL;
using Garage2._0.Models;

namespace Garage2._0.Controllers
{
    public class SetupController : Controller
    {
        private readonly GarageContext db = new GarageContext();

        // GET: Setup
        public ActionResult Index()
        {
            if (db.GarageConfiguration.IsConfigured)
                return RedirectToAction("Index", "Garage");
            return View(db.Configurations.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id,ParkingSpaces,PricePerMinute")] Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                configuration.IsConfigured = true;
                db.Configurations.Add(configuration);
                db.SaveChanges();
                return RedirectToAction("Index", "Garage");
            }
            return View(configuration);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
