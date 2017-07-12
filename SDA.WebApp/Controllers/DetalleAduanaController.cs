using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class DetalleAduanaController : Controller
    {
        ApplicationDbContext _context;

        public DetalleAduanaController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: DetalleAduana
        public ActionResult Index(int headerAduanaId)
        {
            var model = (from t in _context.DetallesAduana
                         where t.headerAduanaId == headerAduanaId
                         select t).ToList();
            ViewBag.header = _context.HeadersAduana.Single(h => h.headerAduanaId == headerAduanaId);
            return View(model);
        }
    }
}