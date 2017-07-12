using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using Microsoft.AspNet.Identity.EntityFramework;
using SDA.WebApp.Models;

namespace SDA.WebApp.Controllers
{
    public class RolController : Controller
    {
        ApplicationDbContext _context;
        public RolController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Role
        public ActionResult Index()
        {
            var model = _context.Roles.ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new Rol();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(IdentityRole model)
        {
            _context.Roles.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}