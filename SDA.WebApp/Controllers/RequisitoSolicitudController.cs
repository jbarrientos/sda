using MvcFlashMessages;
using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class RequisitoSolicitudController : Controller
    {

        ApplicationDbContext _context;

        public RequisitoSolicitudController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Upload(int id, int tipoId = 0)
        {

            var solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Single(s => s.solicitudId == id);


            if (solicitud == null)
                return HttpNotFound();

         
            var requisitos = _context.RequisitosSolicitud.Include(r => r.Requisito)
                .Where(r => r.SolicitudId == id).ToList();

            var reqs = _context.Requisitos
                .Where(r => r.TipoContingenteId == solicitud.DetalleContingente
                .Contingente.tipoContingenteId);

            List<Requisito> pendientes = new List<Requisito>();

            ApplicationDbContext ctx = new ApplicationDbContext();

            foreach (var req in reqs)
            {
                var existe = ctx.RequisitosSolicitud
                    .SingleOrDefault(r => r.RequisitoId == req.Id && r.SolicitudId == solicitud.solicitudId);
                if (existe == null)
                    pendientes.Add(req);
            }


            var vm = new UploadRequisitoViewmodel
            {
                Solicitud = solicitud,
                Requisitos = pendientes,
                RequisitoId = tipoId
            };


            return View(vm);
        }

        [HttpPost]
        public ActionResult Upload(UploadRequisitoViewmodel vm, HttpPostedFileBase requisito)
        {

            if (!ModelState.IsValid)
            {

                var solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Single(s => s.solicitudId == vm.Solicitud.solicitudId);

                var requisitos = _context.RequisitosSolicitud.Include(r => r.Requisito)
                .Where(r => r.SolicitudId == solicitud.solicitudId).ToList();

                var reqs = _context.Requisitos
                    .Where(r => r.TipoContingenteId == solicitud.DetalleContingente
                    .Contingente.tipoContingenteId);

                List<Requisito> pendientes = new List<Requisito>();

                ApplicationDbContext ctx = new ApplicationDbContext();

                foreach (var requi in reqs)
                {
                    var existe = ctx.RequisitosSolicitud
                        .SingleOrDefault(r => r.RequisitoId == requi.Id);
                    if (existe == null)
                        pendientes.Add(requi);
                }


                var viewModel = new UploadRequisitoViewmodel
                {
                    Solicitud = vm.Solicitud,
                    Requisitos = pendientes,
                    Comentarios = vm.Comentarios,
                    RequisitoId = vm.RequisitoId
                };


                return View(viewModel);

            }

            int documentSize = 0;
            byte[] documentData = null;
            if (requisito != null)
            {
                //attach the uploaded image to the object before saving to Database
                documentSize = requisito.ContentLength;
                documentData = new byte[requisito.ContentLength];
                requisito.InputStream.Read(documentData, 0, requisito.ContentLength);


            }

            var req = new RequisitoSolicitud
            {
                Fecha = DateTime.Now,
                Comentarios = vm.Comentarios,
                Documento = documentData,
                MimeType = requisito.ContentType,
                PictureName = requisito.FileName,
                RequisitoId = vm.RequisitoId,
                PictureSize = documentSize,
                SolicitudId = vm.Solicitud.solicitudId
            };

            _context.RequisitosSolicitud.Add(req);
            _context.SaveChanges();

            this.Flash("success", "Requisito para la solicitud No. " + 
                vm.Solicitud.solicitudId.ToString() +
                " ha sido cargado exitosamente.");

            return RedirectToAction("Index", "RequisitoSolicitud", new { id = vm.Solicitud.solicitudId } );
        }


        // GET: RequisitoSolicitud
        public ActionResult Index(int id)
        {

            var solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Single(s => s.solicitudId == id);

            
            if (solicitud == null)
                return HttpNotFound();


            var requisitos = _context.RequisitosSolicitud.Include(r => r.Requisito)
                .Where(r => r.SolicitudId == id).ToList();

            var reqs = _context.Requisitos
                .Where(r => r.TipoContingenteId == solicitud.DetalleContingente.Contingente.tipoContingenteId);

            List<Requisito> pendientes = new List<Requisito>();

            ApplicationDbContext ctx = new ApplicationDbContext();

            foreach (var req in reqs)
            {
                var existe = ctx.RequisitosSolicitud
                    .SingleOrDefault(r => r.RequisitoId == req.Id && r.SolicitudId == solicitud.solicitudId);
                if (existe == null)
                    pendientes.Add(req);
            }

            var vm = new RequisitoSolicitudIndexViewModel
            {
                Solicitud = solicitud,
                Requisitos = requisitos, 
                Pendientes = pendientes
            };

            return View(vm);
        }
    }
}