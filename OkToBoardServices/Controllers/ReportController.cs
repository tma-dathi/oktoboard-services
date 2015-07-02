using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using OkToBoardServices.Models;

namespace OkToBoardServices.Controllers
{
    public class ReportController : ApiController
    {
        private DBContext db = new DBContext();

        // GET api/report
        public IEnumerable<string> Get()
        {
            var data = HttpContext.Current.Request.Headers.Get("otb-data-report");
            var obj = JsonConvert.DeserializeObject<GenerateReport>(data);
            var batchId = obj.batch_id;
            var dt = new DataTable();
            dt.Columns.Add("batch_id", typeof(int));
            dt.Columns.Add("first_name", typeof(string));
            dt.Columns.Add("last_name", typeof(string));
            dt.Columns.Add("gender", typeof(bool));
            var row = dt.NewRow();
            row["batch_id"] = obj.batch_id;
            row["first_name"] = obj.first_name;
            row["last_name"] = obj.last_name;
            row["gender"] = obj.gender;
            dt.Rows.Add(row);
            var ds = new DataSet();
            ds.Tables.Add(dt);



            Logger.log.Debug("=======START========");
            Logger.log.Debug(data);
            Logger.log.Debug("================================");
            Logger.log.Debug("batchId: " + batchId);
            Logger.log.Debug("=======END========");
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
