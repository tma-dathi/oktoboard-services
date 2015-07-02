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

namespace OkToBoardServices.Controllers
{
    public class SignatureController : ApiController
    {
         DBContext db = new DBContext();
        // GET api/signature
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2"};
        }

        private void HandleNonHttpsRequest(HttpActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        // GET api/signature/5
        public string Get(int id)
        {
            return "value";
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
                var action = int.Parse(HttpContext.Current.Request.Headers.Get("action"));
                foreach (var h in HttpContext.Current.Request.Headers.AllKeys)
                {
                    Logger.log.Debug(h);
                }
                if (action == 1)
                {
                    if (filename != null)
                    {
                        Directory.CreateDirectory(dir);
                        using (FileStream output = File.OpenWrite(path))
                        {
                            input.CopyTo(output);
                        }
                    }
                    var items = new Report { Id = userId, Image = path };
                    db.Reports.Add(items);
                    db.SaveChanges();
                    Logger.log.Debug("Save successful");
                }
                if (action == 2)
                {
                    if (File.Exists(dir))
                    {
                        File.Delete(dir);
                    }
                    Directory.CreateDirectory(dir);
                    using (FileStream output = File.OpenWrite(path))
                    {
                        input.CopyTo(output);
                    }
                    var report = (from rp in db.Reports
                                  where rp.Id == userId
                                  select rp).First();
                    report.Image = path;
                    db.SaveChanges();
                    Logger.log.Debug("Update image successful");
                }
            }
            catch (Exception ex)
            {
                Logger.log.Debug(ex);
                throw;
            }
        }

        // PUT api/signature/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/signature/5
        public void Delete(int id)
        {
        }
    }
}
