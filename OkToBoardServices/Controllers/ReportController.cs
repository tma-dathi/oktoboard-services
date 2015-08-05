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
    [RequireHttps]
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
                data = "[{'boarding_info':[{'origin':'LHR','flight_code':'J7','flight_number':'100','id':null,'contact_number':null,'vessel_name':'SUN PRINCESS - BHCC18','eta_time':'14-Aug-2015 00:00','time_arrive':'12-AUG-2015 at 12:00AM','report_type':'pdf','user_id':24,'ship_id':'af839043-5cb5-4d49-a7b8-5fff228fee7e','user_name':null,'create_time':' 4-Aug-2015'}],'crew_info':[{'first_name':'Nickolas','last_name':'Bove','position':'Captain','birthday':' 1-JAN-1900','passport':'H0000006','country_id':101,'gender':'Male','id':null,'country':'INDIAN'}]}]";
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
