using MvcFlashMessages;
using OfficeOpenXml;
using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class HeaderAduanaController : Controller
    {
        ApplicationDbContext _context;

        public HeaderAduanaController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: HeaderAduana
        public ActionResult Index()
        {
            //
            var model = _context.HeadersAduana
                .OrderByDescending(h => h.headerAduanaId)
                .ToList();
            List<HeaderAduanaIndex> headers = new List<HeaderAduanaIndex>();
            //
            
            foreach (var header in model)
            {
                headers.Add(
                    new HeaderAduanaIndex
                    {
                        FechaCarga = header.fechaCarga,
                        NombreArchivo = header.NombreArchivo,
                        Id = header.headerAduanaId,
                        NumLineas = _context.DetallesAduana
                        .Where(d => d.headerAduanaId == header.headerAduanaId)
                        .Count(), 
                        Status = header.Aplicado ? "Procesado" : "Pendiente de Procesar"
                    }
                    );
            }
            var vm = new HeaderAduanaIndexViewModel
            {
                Headers = headers
            };
            return View(vm);
        }

        public ActionResult Create()
        {

            var vm = new HeaderAduanaFormViewModel
            {
                HeaderAduana = new HeaderAduana
                {
                    headerAduanaId = 0,
                    status = "L"
                }
            };
            return View("FormHeaderAduana",vm);
        }

        public ActionResult Save(HeaderAduanaFormViewModel model)
        {
            return View();
        }

        public ActionResult Upload(HeaderAduanaFormViewModel model)
        {
            if(Request != null)
            {
                
                HttpPostedFileBase fileToUpload = Request.Files["uploadedFile"];
                if((fileToUpload != null) && (fileToUpload.ContentLength > 0) &&
                    !string.IsNullOrEmpty(fileToUpload.FileName))
                {
                    string fileName = fileToUpload.FileName;

                    if (!FileExists(fileName))
                    {
                        string fileContent = fileToUpload.ContentType;
                        byte[] fileBytes = new byte[fileToUpload.ContentLength];
                        var datos = fileToUpload.InputStream.Read(fileBytes, 0,
                            Convert.ToInt32(fileToUpload.ContentLength));
                        using (var package = new ExcelPackage(fileToUpload.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var numOfColumns = workSheet.Dimension.End.Column;
                            var numOfRows = workSheet.Dimension.End.Row;
                            // Insert header
                            var header = new HeaderAduana
                            {
                                Comentarios = model.HeaderAduana.Comentarios,
                                fechaCarga = DateTime.Now,
                                NombreArchivo = fileName,
                                status = "L"
                            };
                            _context.HeadersAduana.Add(header);

                            //
                            for (int rowIterator = 2; rowIterator <= numOfRows; rowIterator++)
                            {
                                var detail = new DetalleAduana();
                                detail.headerAduanaId = header.headerAduanaId;
                                detail.aduana = workSheet.Cells[rowIterator, 1].Value.ToString();
                                detail.agenteAduana = workSheet.Cells[rowIterator, 2].Value.ToString();
                                detail.serial = workSheet.Cells[rowIterator, 3].Value.ToString();
                                detail.numeroDM = workSheet.Cells[rowIterator, 4].Value.ToString();
                                detail.empresa = workSheet.Cells[rowIterator, 5].Value.ToString();
                                detail.nombreEmpresa = workSheet.Cells[rowIterator, 6].Value.ToString();
                                detail.consignatario = ""; // workSheet.Cells[rowIterator, 7].Value.ToString(),
                                detail.exportador = workSheet.Cells[rowIterator, 8].Value == null ?
                                    "" : workSheet.Cells[rowIterator, 8].Value.ToString();
                                detail.fechaRegistro = DateTime.FromOADate(long.Parse(workSheet.Cells[rowIterator, 9].Value.ToString()));
                                detail.fechaLiquidacion = DateTime.FromOADate(long.Parse(workSheet.Cells[rowIterator, 10].Value.ToString()));
                                detail.regimen = workSheet.Cells[rowIterator, 11].Value.ToString();
                                detail.totalItems = int.Parse(workSheet.Cells[rowIterator, 12].Value.ToString());
                                detail.preferencia = workSheet.Cells[rowIterator, 13].Value.ToString();
                                detail.cuota = workSheet.Cells[rowIterator, 14].Value.ToString();
                                detail.licencia = workSheet.Cells[rowIterator, 14].Value.ToString();
                                detail.subpartida = workSheet.Cells[rowIterator, 16].Value.ToString();
                                detail.descMercancia = workSheet.Cells[rowIterator, 17].Value.ToString();
                                detail.paisDestino = workSheet.Cells[rowIterator, 18].Value.ToString();
                                detail.paisProcedencia = workSheet.Cells[rowIterator, 19].Value.ToString();
                                detail.paisOrigen = workSheet.Cells[rowIterator, 20].Value.ToString();
                                detail.cuantia = (double)workSheet.Cells[rowIterator, 21].Value;
                                detail.pesoNeto = (double)workSheet.Cells[rowIterator, 22].Value;
                                detail.pesoBruto = (double)workSheet.Cells[rowIterator, 23].Value;
                                detail.fobPartida = (double)workSheet.Cells[rowIterator, 24].Value;
                                detail.cifPartida = (double)workSheet.Cells[rowIterator, 25].Value;
                                detail.porcentajeDAI = (double)workSheet.Cells[rowIterator, 26].Value;
                                detail.dai = (double)workSheet.Cells[rowIterator, 27].Value;
                                detail.porcentajeIVA = (double)workSheet.Cells[rowIterator, 28].Value;
                                detail.iva = (double)workSheet.Cells[rowIterator, 29].Value;
                                //
                                _context.DetallesAduana.Add(detail);
                                //

                            }
                            _context.SaveChanges();
                        }

                    }else
                    {
                        this.Flash("error", "Archivo '" + fileName + "' NO Cargado, ya existe un archivo con el mismo nombre.");
                        return RedirectToAction("Index");
                    }

                    
                }
            }
            this.Flash("success", "Archivo ha sido cargado correctamente.");
            return RedirectToAction("Index");
        }

        public bool FileExists(string fileName)
        {
            var header = _context.HeadersAduana
                .Where(h => h.NombreArchivo == fileName)
                .SingleOrDefault();
            return header != null;
        }
    }
}