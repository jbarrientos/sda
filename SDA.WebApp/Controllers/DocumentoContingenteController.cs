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
    public class DocumentoContingenteController : Controller
    {
        ApplicationDbContext _context;

        public DocumentoContingenteController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult UploadDocumentoContingenteVario(int id)
        {
            var contingente = _context.ContingentesVarios
                    .SingleOrDefault(c => c.Id == id);
            //
            if (contingente == null)
                return HttpNotFound();

            
            var vm = new UploadDocumentoViewmodel
            {
                Contingente = contingente,
                Comentarios = ""
            };


            return View(vm);
        }

        [HttpPost]
        public ActionResult UploadDocumentoContingenteVario(UploadDocumentoViewmodel vm, 
            HttpPostedFileBase documento)
        {

            if (!ModelState.IsValid)
            {

                var viewModel = new UploadDocumentoViewmodel
                {
                    Contingente = 
                    _context.ContingentesVarios
                    .SingleOrDefault(c => c.Id == vm.Contingente.Id),
                    Comentarios = vm.Comentarios
                };


                return View(viewModel);

            }

            int documentSize = 0;
            byte[] documentData = null;
            if (documento != null)
            {
                //attach the uploaded image to the object before saving to Database
                documentSize = documento.ContentLength;
                documentData = new byte[documento.ContentLength];
                documento.InputStream.Read(documentData, 0, documento.ContentLength);
            }

            var req = new DocumentoContingente
            {
                Fecha = DateTime.Now,
                Comentarios = vm.Comentarios,
                Documento = documentData,
                MimeType = documento.ContentType,
                DocumentName = documento.FileName,
                DocumentSize = documentSize,
                ContingenteVarioId = vm.Contingente.Id
            };

            _context.DocumentosContingente.Add(req);
            _context.SaveChanges();

            this.Flash("success", "Documento para el tipo de contingente " +
                vm.Contingente.Descripcion +
                " ha sido cargado exitosamente.");

            return RedirectToAction("Index", "ContingenteVario", 
                new { });
        }


        // GET: RequisitoSolicitud
        public ActionResult Index(int id)
        {

            var contingente = _context.ContingentesVarios
                .SingleOrDefault(v => v.Id == id);
            
            if (contingente == null)
                return HttpNotFound();


            var documentos = _context.DocumentosContingente
                .Where(r => r.ContingenteVarioId == id).ToList();

            var vm = new DocumentoContingenteVarioIndexViewModel
            {
                Contingente = contingente,
                Documentos = documentos
            };

            return View(vm);
        }

        public FileContentResult GetDocument(int id)
        {
            var documento = _context.DocumentosContingente.FirstOrDefault(p => p.Id == id);
            if (documento != null)
            {
                if (documento.MimeType != null)
                    return File(documento.Documento, documento.MimeType);
                else return null;
            }
            else
            {
                return null;
            }
        }
    }
}