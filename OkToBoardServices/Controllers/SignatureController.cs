using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace OkToBoardServices.Controllers
{
    public class SignatureController : ApiController
    {
        // GET api/signature
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
                if (input != null)
                {
                    foreach (var h in HttpContext.Current.Request.Headers.AllKeys)
                    {
                        Logger.log.Debug(h.ToString());
                    }
                    var userId = HttpContext.Current.Request.Headers.Get("otb-userid");
                    var filename = HttpContext.Current.Request.Headers.Get("otb-filename");
                    string dir = HttpContext.Current.Server.MapPath(String.Format(@"~\Images\Signatures\{0}", userId));
                    Directory.CreateDirectory(dir);
                    string path = String.Format(@"{0}\{1}", dir, filename);
                    using (FileStream output = File.OpenWrite(path))
                    {
                        input.CopyTo(output);
                    }
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
