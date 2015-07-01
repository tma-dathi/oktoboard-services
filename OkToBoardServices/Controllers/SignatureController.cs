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
                var userId = HttpContext.Current.Request.Headers.Get("otb-userid");
                var filename = HttpContext.Current.Request.Headers.Get("otb-filename");
                string dir = HttpContext.Current.Server.MapPath(String.Format(@"~\Images\Signatures\{0}", userId));
                string path = String.Format(@"{0}\{1}", dir, filename);
                var action = HttpContext.Current.Request.Headers.Get("action");
                foreach (var h in HttpContext.Current.Request.Headers.AllKeys)
                {
                    Logger.log.Debug(h);
                }
                if (int.Parse(action) == 1)
                {
                    if (filename != null)
                    {
                        Directory.CreateDirectory(dir);
                        using (FileStream output = File.OpenWrite(path))
                        {
                            input.CopyTo(output);
                        }
                    }
                    var items = new Report { Id = int.Parse(userId), Image = path };
                    db.Reports.Add(items);
                    db.SaveChanges();
                }
                if (int.Parse(action) == 2)
                {
                    
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
