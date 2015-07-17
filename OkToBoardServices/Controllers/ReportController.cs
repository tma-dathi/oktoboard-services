using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using OkToBoardServices.Models;
using Microsoft.Reporting.WebForms;

namespace OkToBoardServices.Controllers
{
    [RequireHttps]
    public class ReportController : ApiController
    {
        private DBContext db = new DBContext();

        // GET api/report
        public HttpResponseMessage Get()
        {
            Logger.log.Info("begin");
            var data = HttpContext.Current.Request.Headers.Get("otb-data-report");
            var obj = JsonConvert.DeserializeObject<GenerateReport>(data);
            Logger.log.Debug("=========" + data + "=================");
            Logger.log.Debug("=========" + obj + "=================");
            var repor_type = obj.report_type;
            var fileName = "Report_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_" + obj.id + "." + repor_type;
            string dir = HttpContext.Current.Server.MapPath(String.Format(@"~\GenerateReport\"));
            string filePath = String.Format(@"{0}\{1}", dir, fileName);
            string  imagePath = (from rp in db.Reports
                                 where rp.Id == obj.user_admin_id
                                select rp.Image).FirstOrDefault();
            string vlGender = obj.gender == "True" ? "Male" : "Female";
            Logger.log.Debug("string time arrive:" + obj.time_arrive + "=> " + Convert.ToDateTime(obj.time_arrive).ToString("dd-MM-yyyy 'at' HH:mm"));
            Logger.log.Debug("=========" + obj.user_admin_id + "=================");
            Logger.log.Debug("=========" + imagePath + "=================");
            string txGender = obj.gender == "True" ? "his" : "here";
            var list = new List<GenerateReport>();
            var repportPath = ConfigurationManager.AppSettings["ReportPath"];
            var datasetName = ConfigurationManager.AppSettings["DatasetName"];
            var listData = new GenerateReport
            {
                first_name = obj.first_name,
                last_name = obj.last_name,
                gender = vlGender,
                crew_id = obj.crew_id,
                country_id = obj.country_id,
                passport = obj.passport,
                position = obj.position,
                is_flight= obj.is_flight,
                time_arrive = Convert.ToDateTime(obj.time_arrive).ToString("dd-MM-yyyy 'at' HH:mm"),
                origin = obj.origin,
                text_gender = txGender,
                flight_code = obj.flight_code,
                flight_number = obj.flight_number,
                user_name = obj.user_name,
                vessel_name = obj.vessel_name,
                eta_time =  obj.eta_time,
                country = obj.country,
                created_at = Convert.ToDateTime(obj.created_at).ToString("dd MMMM yyyy"),
                phone_number = obj.phone_number,
                birthday = Convert.ToDateTime(obj.birthday).ToString("dd-MM-yyyy")
                
            };
            list.Add(listData);
            Logger.log.Debug(String.Format("1."));
            var pr = new ReportParameter("rpt_img", "file:/" + imagePath, true);
            var report = new LocalReport();
            report.ReportPath = repportPath;
            var rds = new ReportDataSource();
            rds.Name = datasetName;
            rds.Value = list;
            report.EnableExternalImages = true;
            report.SetParameters(pr);
            Logger.log.Debug(String.Format("2."));
            report.DataSources.Add(rds);
            Logger.log.Debug("start generate report");
            string mimeType;
            string encoding;
            string fileNameExtension;
            string reportType;
            switch (repor_type)
            {
                case "xls":
                    reportType = "EXCEL";
                    break;
                case "pdf":
                    reportType = "PDF";
                    break;
                default:
                    reportType = "WORD";
                    break;
            }
            if (repor_type == "doc")
            {
                reportType = "WORD";
            }
            string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>" + reportType + "</OutputFormat>" +
                "<PageWidth>8.5in</PageWidth>" +
                "<PageHeight>11in</PageHeight>" +
                "<MarginTop>0.25in</MarginTop>" +
                "<MarginLeft>0.25in</MarginLeft>" +
                "<MarginRight>0.25in</MarginRight>" +
                "<MarginBottom>0.25in</MarginBottom>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            var rendereBytes = report.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            using (FileStream fs = File.Create(filePath))
            {
                fs.Write(rendereBytes, 0, rendereBytes.Length);
                Logger.log.Debug(String.Format("3."));
            }
            Logger.log.Debug("=====================create file  ========================");
            var httpResponseMessage = new HttpResponseMessage();
            var memoryStream = new MemoryStream();
            var file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            file.Read(rendereBytes, 0, (int)file.Length);
            memoryStream.Write(rendereBytes, 0, (int)file.Length);
            Logger.log.Debug(String.Format("3."));
            file.Close();
            httpResponseMessage.Content = new ByteArrayContent(memoryStream.ToArray());
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            File.Delete(filePath);
            Logger.log.Debug("=====================Return file download ========================");
            Logger.log.Debug(String.Format("4."));
            return httpResponseMessage;
        }
        // GET api/report/5
        public string Get(int id)
        {
            Logger.log.Info("Nobita");
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
