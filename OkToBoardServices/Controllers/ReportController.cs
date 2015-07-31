using System;
using System.Collections;
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
using OkToBoardServices.Helper;
using OkToBoardServices.Models;
using Microsoft.Reporting.WebForms;

namespace OkToBoardServices.Controllers
{
    //[RequireHttps]
    public class ReportController : ApiController
    {
        private DBContext db = new DBContext();
        // GET api/report
        public HttpResponseMessage Get()
        {
            Logger.log.Info("begin");
            string data;
            if (ConfigurationManager.AppSettings["UseFakeData"].ToLower() == "true")
            {
                data = "[{'boarding_info':[{'origin':'','flight_code':'','flight_number':'','id':null,'contact_number':null,'vessel_name':'Europa 2','eta_time':'07-Apr-2012 00:00','time_arrive':'15-JUL-2015 at 11:11AM','report_type':null,'user_id':1,'ship_id':'0858F942-B8C8-49E6-9025-5499A45180F1','user_name':null,'create_time':'27-Jul-2015'}],'crew_info':[{'id':12,'first_name':'Test','last_name':'Test','crew_id':'1','gender':'Male','position':'1','birthday':' 6-JUL-2015','birthday_place':null,'passport':'1','country_id':6,'time_arrive':'2015-07-14T11:11:11.000Z','flight_code':'11','flight_number':'1111','state_id':1,'user_id':3,'batch_id':9,'remark':null,'created_at':'2015-07-13T07:27:07.672Z','updated_at':'2015-07-13T07:27:07.672Z','origin':'111','expiry_date':null,'user_approved':1,'country':'Andorra'}]}]";
                Logger.log.Debug("=========================Data TEST===========================");
            }
            else
            {
                data = "[" + HttpContext.Current.Request.Headers.Get("otb-data-report") + "]";
                Logger.log.Debug("====Data from Ruby=====" + data);
            }
            string filePath;
            var obj = JsonConvert.DeserializeObject<List<GenerateReport.RootObject>>(data);
            var listBoarding = obj[0].boarding_info;
            var boardingInfo = listBoarding[0];
            var listCrew = obj[0].crew_info;
            string reportType = boardingInfo.report_type;
            Logger.log.Debug("====================" + reportType + "======================");
            if (string.IsNullOrEmpty(reportType) || reportType == "pdf")
            {
                filePath = GenerateSingleHelper.GetFilePathReport(listCrew, listBoarding);
            }
            else
            {
                filePath = GenerateMultipleHelper.GetFilePathReport(listCrew, listBoarding);
            }
            var httpResponseMessage = new HttpResponseMessage();
            var memoryStream = new MemoryStream();
            FileStream fileStream = null;
            try
            {
                using (fileStream = File.OpenRead(filePath))
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
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = System.IO.Path.GetFileName(filePath);
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            if (ConfigurationManager.AppSettings["KeepGeneratedReport"].ToLower() == "false")
            {
                File.Delete(filePath);
            }
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
