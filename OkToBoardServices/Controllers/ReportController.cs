using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace OkToBoardServices.Controllers
{
    public class ReportController : ApiController
    {
        // GET api/report
        public IEnumerable<string> Get()
        {
            var data = HttpContext.Current.Request.Headers.Get("otb-data-report");
            Logger.log.Debug("=======START========");
            Logger.log.Debug(data);
            Logger.log.Debug("=======END========");

            // generate report here

            return new string[] { "value1", "value2" };
        }

        // GET api/report/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/report
        public void Post([FromBody]string value)
        {
        }

        // PUT api/report/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/report/5
        public void Delete(int id)
        {
        }
    }
}
