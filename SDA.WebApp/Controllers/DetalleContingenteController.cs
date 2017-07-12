using Ionic.Zip;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using MvcFlashMessages;
using SDA.Services;
using SDA.Services.Helpers;
using SDA.WebApp.Helpers;
using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class DetalleContingenteController : Controller
    {
        ApplicationDbContext _context;

        public DetalleContingenteController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: DetalleContingente
        public ActionResult Index(int contingenteId)
        {
            var contin = _context.Contingentes.Single(c => c.contingenteId == contingenteId);
            
            var model = (from t in _context.DetallesContingente
                         where t.contingenteId == contingenteId
                         select t).ToList();
            ViewBag.tipo = _context.TiposContingente.Single(t => t.tipoContingenteId == contin.tipoContingenteId);
            ViewBag.contingente = contin;
            ViewBag.tratado = _context.Tratados.Single(t => t.tratadoId == contin.tratadoId);
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _context.DetallesContingente
                .Include(d => d.Contingente.TipoContingente)
                .Include(d => d.Contingente.Tratado)
                .Single(d => d.detalleContingenteId == id);
            ViewBag.contingente = _context.Contingentes.Single(c => c.contingenteId == model.contingenteId);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _context.DetallesContingente.Single(d => d.detalleContingenteId == id);
            ViewBag.contingente = _context.Contingentes.Single(c => c.contingenteId == model.contingenteId);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(DetalleContingente model)
        {
            var m = _context.DetallesContingente.Single(d => d.detalleContingenteId == model.detalleContingenteId);
            if (m == null)
                return HttpNotFound();

            m.fechaFin = model.fechaFin;
            m.fechaFinSolicitudes = model.fechaFinSolicitudes;
            m.fechaInicio = model.fechaInicio;
            m.fechaInicioSolicitudes = model.fechaInicioSolicitudes;
            m.monto = model.monto;
            m.montoNuevo = model.montoNuevo;
            m.volumenNuevo = model.volumenNuevo;
            
            _context.SaveChanges();
            return RedirectToAction("index", new { contingenteId = model.contingenteId});
        }

        public ActionResult GenerateNote(int id)
        {

            var detalle = _context.DetallesContingente
                .Include(d => d.Contingente.TipoContingente.UnidadMedida)
                .SingleOrDefault(d => d.detalleContingenteId == id);

            if (detalle == null)
                return HttpNotFound();

            var plantilla = detalle.Contingente.TemplateNotificacion;

            List<CartaNotificacion> cartas = new List<CartaNotificacion>();

            var solicitudes = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Where(s => s.detalleContingenteId == id && s.estado == "A" && s.volumenAsignado > (decimal)0.00)
                .ToList();

            var sdaPath = Utils.GetParameter("SDA_URL", _context);

            plantilla = plantilla.Replace("{{logoMinecPng}}", sdaPath  + "/Content/img/logo_minec.png");
            plantilla = plantilla.Replace("{{logoMinecJpg}}", sdaPath + "/Content/img/logo_minec.jpg");
            plantilla = plantilla.Replace("{{logoDatcoJpg}}", sdaPath + "/Content/img/logo_datco.jpg");


            plantilla = plantilla.Replace("{{anio}}", detalle.anio.ToString())
                .Replace("{{contingente}}", detalle.Contingente.TipoContingente.nombre)
                .Replace("{{diaLetras}}", DateTime.Now.Day.ToString())
                .Replace("{{mesLetras}}", NumerosALetras.ToMes(DateTime.Now.Month).ToLower())
                .Replace("{{anioLetras}}", DateTime.Now.Year.ToString())
                .Replace("{{nombreDirector}}", Utils.GetParameter(Constant.NOMBRE_DIRECTOR, _context))
                .Replace("{{puestoDirector}}", Utils.GetParameter(Constant.PUESTO_DIRECTOR, _context));

            var correlativo = 1;

            foreach (var sol in solicitudes)
            {
                var plantillaToWork = plantilla;
                plantillaToWork = plantillaToWork.Replace("{{puestoNotificacion}}", 
                    sol.contribuyente.CargoNotificacion);
                plantillaToWork = plantillaToWork.Replace("{{representanteLegal}}",
                    sol.contribuyente.CargoRepresentanteLegal);
                plantillaToWork = plantillaToWork.Replace("{{nombreNotificacion}}", 
                    (sol.contribuyente.NombreNotificacion == null ? sol.contribuyente.nombre : 
                    sol.contribuyente.NombreNotificacion));
                plantillaToWork = plantillaToWork.Replace("{{categoria}}", 
                    (sol.esImportadorHistorico == "Y" ? "Importador Histórico" : "Nuevo Importador"))
                    .Replace("{{importador}}", sol.contribuyente.nombre);

                var fracciones = "";
                plantillaToWork = plantillaToWork.Replace("{{unidadMedida}}",
                    detalle.Contingente.TipoContingente.UnidadMedida.nombre);
                plantillaToWork = plantillaToWork.Replace("{{abreviaturaUnidadMedida}}",
                    detalle.Contingente.TipoContingente.UnidadMedida.Abreviatura);
                if (detalle.Contingente.TipoContingente.DistribuirPorFraccion)
                {
                    var detalleSolicitud = _context.DetallesSolicitud
                        .Include(d => d.Fraccion)
                        .Where(d => d.SolicitudId == sol.solicitudId)
                        .ToList();

                    foreach (var det in detalleSolicitud)
                    {
                        fracciones += "<tr><td>"+det.Fraccion.nombre+
                            "</td><td align=\"center\">"+det.Fraccion.codigo+
                            "</td><td align=\"center\">"+det.Asignado.ToString("###,##0.00")+"</td></tr>";
                    }
                    
                }
                else if(sol.fraccionId != null && sol.fraccionId > 0)
                {
                    var fraccion = _context.Fracciones
                        .SingleOrDefault(f => f.fraccionId == sol.fraccionId);

                    fracciones += "<tr><td>" + fraccion.nombre +
                            "</td><td align=\"center\">" + fraccion.codigo +
                            "</td><td align=\"center\">" + sol.volumenAsignado.ToString("###,##0.00") + "</td></tr>";
                }else
                {
                    var fraccionesContingente = _context.FraccionesTipoContingente
                        .Include(f => f.Fraccion)
                        .Where(f => f.tipoContingenteId == detalle.Contingente.tipoContingenteId)
                        .ToList();
                    var counter = fraccionesContingente.Count();
                    var idx = 0;
                    //
                    foreach(var frac in fraccionesContingente)
                    {
                        idx++;
                        if(idx == counter && counter > 1)
                        {
                            fracciones = fracciones.Substring(0, fracciones.Length - 2);
                            fracciones += " y ";
                        }
                            
                        fracciones += frac.Fraccion.codigo;
                        if (idx < counter && counter > 1)
                        {
                            fracciones += ", ";
                        }
                        
                    }
                    plantillaToWork = plantillaToWork.Replace("{{volumen}}",
                    sol.volumenAsignado.ToString("##,###,##0.00"));
                    //plantillaToWork = plantillaToWork.Replace("{{unidadMedida}}",
                    //detalle.Contingente.TipoContingente.UnidadMedida.nombre);

                }
                correlativo++;

                plantillaToWork = plantillaToWork.Replace("{{fracciones}}", fracciones)
                    .Replace("{{referencia}}", "DATCO/CARTA/" + sol.solicitudId.ToString() + "/" + detalle.anio.ToString());



                var nota = new CartaNotificacion { Carta = PDFServices.GetPDF(plantillaToWork), NombreArchivo = sol.solicitudId.ToString() };
                cartas.Add(nota);
            }

            var outputStream = new MemoryStream();

            
            using (var zip = new ZipFile())
            {
                foreach (var carta in cartas)
                {
                    var nombreArchivo = carta.NombreArchivo + ".pdf";
                    zip.AddEntry(nombreArchivo, carta.Carta);
                }
                zip.Save(outputStream);

            }

            outputStream.Position = 0;
            // Update Contingente
            detalle.FechaGeneracionNotificaciones = DateTime.Now;
            detalle.NotificacionesGeneradas = true;
            _context.SaveChanges();
            //
            return File(outputStream, "application/zip", "Notificaciones_" + detalle.Contingente.TipoContingente.nombre + 
                "_"  + detalle.anio.ToString() + ".zip");
        }

        struct CartaNotificacion
        {
            public string NombreArchivo { get; set; }
            public byte[] Carta { get; set; }
        }

        public ActionResult UploadNotificacionesFirmadas(int id)
        {
            var detalle = _context.DetallesContingente
                .Include(d => d.Contingente.TipoContingente)
                .SingleOrDefault(s => s.detalleContingenteId == id);

            if (detalle == null)
                return HttpNotFound();

            var vm = new UploadNotificacionesFirmadas
            {
                DetalleContingente = detalle
            };
            
            return View(vm);
        }
        [HttpPost]
        public ActionResult UploadNotificaciones(UploadNotificacionesFirmadas model)
        {
            var detalle = _context.DetallesContingente
                .SingleOrDefault(d => d.detalleContingenteId == model.DetalleContingente.detalleContingenteId);

            if (detalle == null)
                return HttpNotFound();

            if (Request != null)
            {

                HttpPostedFileBase fileToUpload = Request.Files["uploadedFile"];
                if ((fileToUpload != null) && (fileToUpload.ContentLength > 0) &&
                    !string.IsNullOrEmpty(fileToUpload.FileName))
                {
                    //string fileName = fileToUpload.FileName;

                    string fileContent = fileToUpload.ContentType;
                    byte[] fileBytes = new byte[fileToUpload.ContentLength];
                    var datos = fileToUpload.InputStream.Read(fileBytes, 0,
                        Convert.ToInt32(fileToUpload.ContentLength));

                    //

                    // Unpack file

                    
                    var fileName = Path.GetFileName(fileToUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/notificaciones"), fileName);
                    fileToUpload.SaveAs(path);
                    

                    //string extractPath = Server.MapPath("./upload/" + detalle.contingenteId.ToString() + "-notificaciones-firmadas");
                    Boolean flgStaus = true;
                    var zipfile = fileToUpload.FileName;

                    Boolean flgStatus = true;
                    //get the temporary directory name   
                    using (ZipInputStream s = new ZipInputStream(System.IO.File.OpenRead(path)))
                    {

                        ZipEntry theEntry;
                        while ((theEntry = s.GetNextEntry()) != null)
                        {

                            Console.WriteLine(theEntry.FileName);

                            string directoryName = Path.GetDirectoryName(theEntry.FileName);
                            fileName = Path.GetFileName(theEntry.FileName);

                            // create directory
                            if (directoryName.Length > 0)
                            {
                                Directory.CreateDirectory(directoryName);
                            }

                            if (fileName != String.Empty)
                            {
                                path = Path.Combine(Server.MapPath("~/App_Data/notificaciones"), theEntry.FileName);
                                //using (FileStream streamWriter = System.IO.File.Create(theEntry.FileName))
                                using (FileStream streamWriter = System.IO.File.Create(path))
                                {

                                    int size = 2048;
                                    byte[] data = new byte[2048];
                                    while (true)
                                    {
                                        size = s.Read(data, 0, data.Length);
                                        if (size > 0)
                                        {
                                            streamWriter.Write(data, 0, size);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    var idSolicitud = int.Parse(theEntry.FileName.Substring(0, 
                                        theEntry.FileName.IndexOf(".")));
                                    //
                                    var solicitud = _context.Solicitudes
                                    .SingleOrDefault(solic => solic.solicitudId == idSolicitud);

                                    if (solicitud != null)
                                    {


                                        //solicitud.NotificacionFirmada = data;
                                        solicitud.RutaArchivoNotificacion = path;
                                        _context.SaveChanges();

                                    }



                                }

                            }
                        }

                    }
                    detalle.FechaCargaNotificaciones = DateTime.Now;
                    detalle.NotificacionesCargadas = true;
                    _context.SaveChanges();
                    this.Flash("success", "Archivo " + fileToUpload.FileName + " ha sido cargado correctamente.");


                    //string extractPath = Server.MapPath("~/" + detalle.contingenteId.ToString() + "-notificaciones-firmadas/");
                    //using (ZipFile zip = ZipFile.Read(fileToUpload.InputStream))
                    //{
                    //    zip.ExtractAll(extractPath, ExtractExistingFileAction.DoNotOverwrite);

                    //    foreach (string e in Directory.GetFiles(extractPath))
                    //    {

                    //        var zipFileName = e;

                    //        var solicitud = _context.Solicitudes
                    //            .SingleOrDefault(s => s.solicitudId == 
                    //            int.Parse(zipFileName.Substring(1, zipFileName.IndexOf("."))));

                    //        if(solicitud != null){


                    //            solicitud.NotificacionFirmada = e.;
                    //            _context.SaveChanges();

                    //        }

                    //        //if (header)
                    //        //{
                    //        //    System.Console.WriteLine("Zipfile: {0}", zip.Name);
                    //        //    if ((zip.Comment != null) && (zip.Comment != ""))
                    //        //        System.Console.WriteLine("Comment: {0}", zip.Comment);
                    //        //    System.Console.WriteLine("\n{1,-22} {2,8}  {3,5}   {4,8}  {5,3} {0}",
                    //        //                         "Filename", "Modified", "Size", "Ratio", "Packed", "pw?");
                    //        //    System.Console.WriteLine(new System.String('-', 72));
                    //        //    header = false;
                    //        //}

                    //        //System.Console.WriteLine("{1,-22} {2,8} {3,5:F0}%   {4,8}  {5,3} {0}",
                    //        //                       e.FileName,
                    //        //                       e.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
                    //        //                       e.UncompressedSize,
                    //        //                       e.CompressionRatio,
                    //        //                       e.CompressedSize,
                    //        //                       (e.UsesEncryption) ? "Y" : "N");
                    //    }
                    //}
                }
                else
                {
                    this.Flash("error", 
                        "Archivo NO Cargado, ya existe un archivo con el mismo nombre.");
                    return RedirectToAction("Index");
                }


                
            }
            
            return RedirectToAction("Index", "Solicitud", new { detalleContingenteId = model.DetalleContingente.detalleContingenteId });
        }

        public ActionResult SendEmailNotificaciones(int id)
        {

            var detalle = _context.DetallesContingente
                .Include(d => d.Contingente.TipoContingente)
                .SingleOrDefault(d => d.detalleContingenteId == id);

            if (detalle == null)
                return HttpNotFound();

            return View(detalle);
        }

        [HttpPost]
        public ActionResult SendEmailNotificaciones(DetalleContingente model)
        {

            var detalle = _context.DetallesContingente
                .Include(d => d.Contingente.TipoContingente)
                .SingleOrDefault(d => d.detalleContingenteId == model.detalleContingenteId);

            if (detalle == null)
                return HttpNotFound();

            var solicitudes = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Where(s => s.detalleContingenteId == model.detalleContingenteId
                && s.RutaArchivoNotificacion != null
                && (s.contribuyente.email != null || s.contribuyente.EmailAlternativo != null))
                .OrderBy(s => s.solicitudId);

            foreach (var solicitud in solicitudes)
            {
                // Send Email
                if(solicitud.contribuyente.email != "")
                {
                    var email = new Email();
                    email.From = Constant.DEFAULT_EMAIL_ACCOUNT;
                    //email.To.Add("jorge.barrientos@gmail.com");

                    email.To.Add(solicitud.contribuyente.email);
                    if (solicitud.contribuyente.EmailAlternativo != "" 
                        && solicitud.contribuyente.EmailAlternativo != null)
                    {
                        email.To.Add(solicitud.contribuyente.EmailAlternativo);
                    }
                    email.Subject = "Ministerio de Economía - Notificación de Asignación";
                    email.Body = "<p>Estimado(a):</p><p>" + solicitud.contribuyente.nombre + "</p>";
                    email.Body += "<p>En atención a su solicitud No. " + solicitud.solicitudId.ToString() + 
                        "para el contingente " + detalle.Contingente.TipoContingente.nombre +
                        ", atentamente le comunicamos que su cuota para el presente año ha sido asignada.</p>";
                    email.Body += "<p>Por favor, descargue la nota de asignación en su cuenta en el Sistema de Gestión de Contingentes Arancelarios.</p>";
                    email.Body += "<p>Asi mismo, puede solicitar la nota de asignación en original en nuestras oficinas, ubicadas en Alameda Juan Pablo II y Calle Calle Guadalupe, Edificio C-2, 3° nivel, Centro de Gobierno, San Salvador.</p>";
                    email.Body += "<p>Le recordamos que a partir de este año, la solicitud de las licencias se realizará desde el Sistema de gestión de contingentes arancelarios, para conocer el proceso puede descargar la guía del usuario <a href='#'>aqui</a>.</p>";
                    email.Body += "<p>Para consultas, puede contactarnos a los teléfonos 2590 – 5788/ 5790 o al correo electrónico datco@minec.gob.sv.</p>";
                    email.Body += "<p>Atentamente</p>";
                    email.Body += "<p>Dirección de Administración de Tratados Comerciales<br />Ministerio de Economía.</p>";
                    
                    email.Send();
                }
            }

            detalle.NotificacionesEnviadas = true;
            detalle.FechaEnvioNotificaciones = DateTime.Now;
            _context.SaveChanges();

            this.Flash("success", "Envio de notificaciones a importadores realizado correctamente.");
            return RedirectToAction("Index","Solicitud",new { detalleContingenteId = detalle.detalleContingenteId });
        }
    }
}