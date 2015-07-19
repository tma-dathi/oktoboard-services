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
            if (ConfigurationManager.AppSettings["UseFakeData"].ToLower() == "true")
            {
                data = @"{'id':1,
                      'first_name':'test',
                      'last_name':'Nobita',
                      'crew_id':'12',
                      'gender':false,
                      'position':'OK',
                      'birthday':'2015-07-04T00:00:00.000Z',
                      'birthday_place':null,
                      'passport':'OK',
                      'country_id':8,
                      'time_arrive':'2015-07-04T00:00:00.000Z',
                      'flight_code':'ae',
                      'flight_number':'1212',
                      'state_id':4,
                      'user_id':3,
                      'batch_id':1,
                      'remark':null,
                      'created_at':'2015-07-04T04:49:48.774Z',
                      'updated_at':'2015-07-13T08:04:08.874Z',
                      'origin':'exq',
                      'expiry_date':null,
                      'user_name':'test@tma.com.vn',
                      'vessel_name':'AMSTERDAM',
                      'eta_time':'2014-02-25',
                      'ship_id':'444016b8-cdfd-4620-a9cf-778e6b484349',
                      'country':'Anguilla',
                      'phone_number':null,
                      'report_type':'doc',
                      'user_admin_id':1}";
            }
            Logger.log.Debug(data);
            var obj = JsonConvert.DeserializeObject<GenerateReport>(data);
            Logger.log.Debug("=========" + data + "=================");
            Logger.log.Debug("=========" + obj + "=================");
            var repor_type = obj.report_type;
            Logger.log.Debug("repor_type: " + repor_type);
            Vessel vessel = db.Vessels.Find(new Guid(obj.ship_id));
            Logger.log.Debug("Vessel: " + vessel.ToString());
            var etd = vessel.Arrangements.Where(x => x.ETADate == Convert.ToDateTime(obj.eta_time))
                                         .Select(x => x.ETDDate).FirstOrDefault();
            Logger.log.Debug("ETD: " + etd);

            var fileName = "Report_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_" + obj.id + "." + repor_type;
            string dir = EnsurePathExist(HttpContext.Current.Server.MapPath(@"~\GenerateReport"));
            string filePath = String.Format(@"{0}\{1}", dir, fileName);
            string  imagePath = (from rp in db.Reports
                                 where rp.Id == obj.user_admin_id
                                select rp.Image).FirstOrDefault();
            Logger.log.Debug("===========get url signature from database================");
            string vlGender = obj.gender == "True" ? "Male" : "Female";
            Logger.log.Debug("=========" + imagePath + "=================");
            var list = new List<GenerateReport>();
            var repportPath = ConfigurationManager.AppSettings["ReportPath"];
            var datasetName = ConfigurationManager.AppSettings["DatasetName"];
            Logger.log.Debug("====Start add data to ===============");
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
                time_arrive = obj.time_arrive,
                origin = obj.origin,
                flight_code = obj.flight_code,
                flight_number = obj.flight_number,
                user_name = obj.user_name,
                vessel_name = obj.vessel_name,
                eta_time =  obj.eta_time,
                country = obj.country,
                date_generate = DateTime.Now.ToString("dd MMMM yyyy"),
                phone_number = obj.phone_number,
                birthday = Convert.ToDateTime(obj.birthday).ToString("dd-MM-yyyy"),
                etd_time = etd.ToString("dd MMMM yyyy")
            };
            list.Add(listData);
            Logger.log.Debug("====Add data succsess full ===============");
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

        private string EnsurePathExist(string path)
        {
            // Set to folder path we must ensure exists.
            string outputPath = path;
            try
            {
                // If the directory doesn't exist, create it.
                if (!Directory.Exists(path))
                {
                    //WriteLog("Create folder: " + path);
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception)
            {
                // Set 'bin' folder if any exception
                DirectoryInfo di = new DirectoryInfo(path);
                outputPath = String.Format(@"{0}\bin", di.Parent.FullName);
            }
            //WriteLog("output file path: " + outputPath);
            return outputPath;
        }   
    }
}
