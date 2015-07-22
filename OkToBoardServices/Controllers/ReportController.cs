﻿using System;
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

        private string PopulateDateTime(DateTime date, DateTime time)
        {
            var result = DateTime.Now.ToString("dd-MMM-yyyy HH:mm");
            var d = String.Format("{0:d-MMM-yyyy}", date);
            var t = String.Format("{0:HH:mm}", time);
            result = String.Format("{0} {1}", d, t);
            return result;
        }

        // GET api/report
        public HttpResponseMessage Get()
        {
            Logger.log.Info("begin");
            string data;
            if (ConfigurationManager.AppSettings["UseFakeData"].ToLower() == "true")
            {
                data = "[{'boarding_info':[{'origin':'LHR','time_arrive':'2015-07-22T19:45:00.000Z','flight_code':'J7','flight_number':'10','id':null,'contact_number':'+84 0986 977 123','vessel_name':'Adonia','eta_time':'14-Mar-2014 08:00','report_type':'doc','user_id':1,'ship_id':'444016b8-cdfd-4620-a9cf-778e6b484349','user_name':'Nguyen Barry','create_time':'22-Jul-2015'}],'crew_info':[{'first_name':'crew 5','last_name':'last 5','position':'Rank','birthday':'2015-07-22T00:00:00.000Z','passport':'Pas','country_id':1,'gender':true,'id':null,'country':'Afghanistan'},{'first_name':'crew 4','last_name':'last 4','position':'Rank 4','birthday':'2015-07-22T00:00:00.000Z','passport':'Pas','country_id':1,'gender':true,'id':null,'country':'Afghanistan'},{'first_name':'crew 3','last_name':'last3 ','position':'Rank 3','birthday':'2015-07-22T00:00:00.000Z','passport':'Pas','country_id':1,'gender':true,'id':null,'country':'Afghanistan'},{'first_name':'crew 2','last_name':'las2','position':'Rank','birthday':'2015-07-22T00:00:00.000Z','passport':'Pass','country_id':1,'gender':true,'id':null,'country':'Afghanistan'},{'first_name':'crew 1','last_name':'last 1','position':'Rank 1','birthday':'2015-07-22T00:00:00.000Z','passport':'Pas','country_id':1,'gender':true,'id':null,'country':'Afghanistan'}]}]";
                Logger.log.Debug("=========================Data TEST===========================");
            }
            else
            {
                 data = "[" + HttpContext.Current.Request.Headers.Get("otb-data-report") + "]"; 
                 Logger.log.Debug("====Data from Ruby=====" + data);
            }
            var obj = JsonConvert.DeserializeObject<List<GenerateReport.RootObject>>(data);
            var listBoarding = obj[0].boarding_info;

            var boarding_info = listBoarding[0];
            var listCrew = obj[0].crew_info;
            string report_type = boarding_info.report_type;
            string eta_time = boarding_info.eta_time;
            string ship_id = boarding_info.ship_id;
            int user_id = boarding_info.user_id;

            Logger.log.Debug("=====report_type====" + report_type + "====eta_time=" + eta_time + "======");
            Logger.log.Debug("=========" + listBoarding + "=================");
            Logger.log.Debug("=========" + listCrew + "=================");
            Logger.log.Debug("=========" + obj + "=================");

            var repor_type = report_type;
            Logger.log.Debug("repor_type: " + repor_type);
            Vessel vessel = db.Vessels.Find(new Guid(ship_id));
            Logger.log.Debug("Vessel: " + vessel.ToString());
            var etd = vessel.Arrangements.Where(x => PopulateDateTime(x.ETADate, x.ETATime) == eta_time)
                                         .Select(x => PopulateDateTime(x.ETDDate, x.ETDTime)).FirstOrDefault();
            Logger.log.Debug("ETD: " + etd);
            boarding_info.etd_time = etd;
            var fileName = "Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + user_id + "." + repor_type;
            string dir = EnsurePathExist(HttpContext.Current.Server.MapPath(@"~\GenerateReport"));
            string filePath = String.Format(@"{0}\{1}", dir, fileName);
            string imagePath = (from rp in db.Reports
                                where rp.Id == user_id
                                select rp.Image).FirstOrDefault();
            Logger.log.Debug("===========get url signature from database================");
            //string vlGender = obj.gender == "True" ? "Male" : "Female";
            Logger.log.Debug("=========" + imagePath + "=================");
            var reportSignlePage = ConfigurationManager.AppSettings["ReportSignlePage"];
            var reportMultiplePage = ConfigurationManager.AppSettings["ReportMultiplePage"];
            var dataSetCrewInfo = ConfigurationManager.AppSettings["DataSetCrewInfo"];
            Logger.log.Debug("====Start add data to ===============");
            Logger.log.Debug("====Add data succsess full ===============");
            var pr = new ReportParameter("rpt_img", "file:/" + imagePath, true);
            var report = new LocalReport();
            report.ReportPath = listCrew.Count <= 4 ? reportSignlePage : reportMultiplePage;
            
            var rds = new ReportDataSource();
            rds.Name = dataSetCrewInfo;
            rds.Value = listCrew;
            var rds2 = new ReportDataSource { Name = "DataSetBoardingInfo", Value = listBoarding };
            report.EnableExternalImages = true;
            report.SetParameters(pr);
            report.DataSources.Add(rds);
            report.DataSources.Add(rds2);
            Logger.log.Debug("start generate report");
            string mimeType;
            string encoding;
            string fileNameExtension;
            string reportType;
            switch (report_type)
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
            string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>" + reportType + "</OutputFormat>" +
                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            var rendereBytes = report.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            using (FileStream fs = File.Create(filePath))
            {
                fs.Write(rendereBytes, 0, rendereBytes.Length);
                Logger.log.Debug(String.Format("8."));
            }
            Logger.log.Debug("=====================create file  ========================");
            var httpResponseMessage = new HttpResponseMessage();
            var memoryStream = new MemoryStream();
            var file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            file.Read(rendereBytes, 0, (int)file.Length);
            memoryStream.Write(rendereBytes, 0, (int)file.Length);
            Logger.log.Debug(String.Format("9."));
            file.Close();
            httpResponseMessage.Content = new ByteArrayContent(memoryStream.ToArray());
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            File.Delete(filePath);
            Logger.log.Debug("=====================Return file download ========================");
            Logger.log.Debug(String.Format("10."));
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
