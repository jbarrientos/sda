using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                
                var importador =
                _context.Contribuyentes
                .SingleOrDefault(c => c.nit == User.Identity.Name);
                if (importador == null)
                {
                    if (User.IsInRole(RoleName.ADMINISTRADOR))
                    {
                        return RedirectToAction("SummaryContingentes", "Contingente", 
                            new { year = DateTime.Now.Year } );
                    }
                    return View();
                }
                    
                return RedirectToAction("Dashboard", "Contribuyente");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}