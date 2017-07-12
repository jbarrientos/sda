using SDA.Services.Helpers;
using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class PreregistrosController : ApiController
    {

        private ApplicationDbContext _context;

        public PreregistrosController()
        {
            _context = new ApplicationDbContext();
        }

        
        [HttpPut]
        public void UpdatePreregistro(int id)
        {

            var model = _context.Preregistros.SingleOrDefault(m => m.Id == id);
            
            if (model == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            // Hash
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(model.Nit);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);

            string hash = BitConverter.ToString(encodedBytes).Replace("-", "").ToLower();
            //
            model.Status = "A";
            model.Token = hash;

            _context.SaveChanges();
            SendEmail(model);
        }

        private void SendEmail(Preregistro model)
        {
            // Send Email
            var request = HttpContext.Current.Request;
            var address = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);

            Email email = new Email();
            email.To.Add(model.Email);
            email.From = Constant.DEFAULT_EMAIL_ACCOUNT;
            email.Subject = "Ministerio de Economía - SDA - Aprobación de Solicitud";
            email.Body = "<h2>" + model.Nombre + "</h2>";
            email.Body += "<hr />";
            email.Body += "<p>Por medio de este correo le informamos que su solicitud para acceder al Sistema de Desgravación Arancelaria " +
                "ha sido aprobada, sin embargo se requiere que complete información en nuestros registros, para "+
                "ello es necesario que acceda al siguiente enlace y complete los datos solicitados:</p>";
            email.Body += "<p><a href='" +
                address + "/Preregistro/Completar/" + model.Id.ToString() + "?token=" + model.Token +
                "'>Completar información para registro en SDA</a></p>";
            email.Body += "<hr />Ministerio de Economía - " + DateTime.Now.Year.ToString();
            email.Send();
        }
    }
}
