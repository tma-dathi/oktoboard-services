using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using OkToBoardServices.Models;
using System.Net.Http.Headers;

namespace OkToBoardServices.Controllers
{
    [RequireHttps]
    public class SignatureController : ApiController
    {
        DBContext db = new DBContext();
        
        // GET api/signature/5
        public HttpResponseMessage Get(int id)
        {
            string dir = HttpContext.Current.Server.MapPath(String.Format(@"~\Images\Signatures\{0}", id));
            var image = db.Reports.Where(x => x.Id == id).Select(y => y.Image).First();
            var httpResponseMessage = new HttpResponseMessage();
            var memoryStream = new MemoryStream();
            FileStream fileStream = null;
            try
            {
                using (fileStream = File.OpenRead(image))
                {
                    fileStream.CopyTo(memoryStream);
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error("Error while read zip file and write to stream: " + ex.InnerException.ToString());
            }
            finally
            {
                if (fileStream != null) fileStream.Close();
            }

            httpResponseMessage.Content = new ByteArrayContent(memoryStream.ToArray());
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = System.IO.Path.GetFileName(image);
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            return httpResponseMessage;
        }

        // POST api/signature
        public void Post([FromBody]string value)
        {
            try
            {
                Stream input = HttpContext.Current.Request.InputStream;
                var userId = int.Parse(HttpContext.Current.Request.Headers.Get("otb-userid"));
                var filename = HttpContext.Current.Request.Headers.Get("otb-filename");
                string dir = HttpContext.Current.Server.MapPath(String.Format(@"~\Images\Signatures\{0}", userId));
                string path = String.Format(@"{0}\{1}", dir, filename);
                if (filename != null)
                {
                    Directory.CreateDirectory(dir);
                    using (FileStream output = File.OpenWrite(path))
                    {
                        input.CopyTo(output);
                    }
                    var items = new Report { Id = userId, Image = path };
                    db.Reports.Add(items);
                    db.SaveChanges();
                    Logger.log.Debug("Save successful");
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                throw;
            }
        }

        // PUT api/signature/5
        public void Put([FromBody]string value)
        {
            Stream input = HttpContext.Current.Request.InputStream;
            var userId = int.Parse(HttpContext.Current.Request.Headers.Get("otb-userid"));
            var filename = HttpContext.Current.Request.Headers.Get("otb-filename");
            string dir = HttpContext.Current.Server.MapPath(String.Format(@"~\Images\Signatures\{0}", userId));
            if (File.Exists(dir))
            {
                var directoryInfor = new DirectoryInfo(dir);
                foreach (FileInfo file in directoryInfor.GetFiles()) file.Delete();
            }
            else
            {
                Directory.CreateDirectory(dir);
            }
            if (filename != null)
            {
                var report = (from rp in db.Reports
                              where rp.Id == userId
                              select rp).First();
                string path = String.Format(@"{0}\{1}", dir, filename);
                using (FileStream output = File.OpenWrite(path))
                {
                    input.CopyTo(output);
                }
                report.Image = path;
                db.SaveChanges();
            }
        }

        // DELETE api/signature/5
        public void Delete(int id)
        {
        }
    }
}
