﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using OkToBoardServices.Models;

namespace OkToBoardServices.Helper
{
    public static class GenerateSingleHelper
    {
        private static readonly DBContext Db = new DBContext();
        public static string GetFilePathReport(List<GenerateReport.CrewInfo> listCrew, List<GenerateReport.BoardingInfo> listBoarding)
        {
            var boardingInfomation = listBoarding[0];
            string etaTime = boardingInfomation.eta_time;
            string reportType = boardingInfomation.report_type;
            Vessel vessel = Db.Vessels.Find(new Guid(boardingInfomation.ship_id));
            Logger.log.Debug(String.Format("[Single] Vessel: Name {0} ----- Id {1}", vessel.Name, vessel.Id));
            try
            {
                var etd = vessel.Arrangements.Where(x => FormatDateTime.Populate(x.ETADate, x.ETATime) == etaTime)
                                             .Select(x => FormatDateTime.Populate(x.ETDDate, x.ETDTime)).FirstOrDefault();
                Logger.log.Debug("[Single] etd: " + etd);
                boardingInfomation.etd_time = (etd == "00:00") ? "" : etd;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.InnerException.ToString());
            }

            int userId = boardingInfomation.user_id;
            string dir = CheckExist.EnsurePathExist(HttpContext.Current.Server.MapPath(@"~\GenerateReport"));
            Logger.log.Debug("[Single] dir: " + dir);
            CheckExist.EnsurePathExist(HttpContext.Current.Server.MapPath(@"~\GenerateReport\GenerateSingle"));
            string filePath;
            var reportSignlePage = ConfigurationManager.AppSettings["ReportSignlePage"];
            var dataSetCrewInfo = ConfigurationManager.AppSettings["DataSetCrewInfo"];
            string imagePath = ConfigurationManager.AppSettings["ImagePathSignature"];
            imagePath = HttpContext.Current.Server.MapPath(String.Format(@"~\{0}", imagePath));
            Logger.log.Debug("[Single] imagePath: " + imagePath);
            
            string deviceInfo =
            "<DeviceInfo>" +
            "<OutputFormat>" + reportType + "</OutputFormat>" +
            "</DeviceInfo>";
            string mimeType;
            string fileNameExtension;
            string encoding;
            string[] streams;
            Warning[] warnings;
            const string fileType = "PDF";
            if (string.IsNullOrEmpty(reportType))
            {   
                filePath = CheckExist.EnsurePathExist(HttpContext.Current.Server.MapPath(@"~\GenerateReport\GenerateSingle\" + DateTime.Now.ToString("yyyyMMdd_HHmmssffff") + "_" + userId));
                int i = 1;
                foreach (GenerateReport.CrewInfo t in listCrew)
                {
                    Logger.log.Debug("[Single] LOOP CREWINFO - ROUND: " + i++);
                    var report1 = new LocalReport {ReportPath = reportSignlePage};
                    var listData = new GenerateReport.CrewInfo
                    {
                        first_name = t.first_name,
                        last_name = t.last_name,
                        birthday = t.birthday,
                        country = t.country,
                        gender = t.gender,
                        passport = t.passport,
                        position = t.position
                    };
                    var list = new List<GenerateReport.CrewInfo> { listData };
                    var fileName = t.first_name + " " + t.last_name +"_" + DateTime.Now.ToString("yyyyMMdd_HHmmssffff") + ".pdf";
                    string filePathGenerateSingly = String.Format(@"{0}\{1}", filePath, fileName);
                    var reportDataSourceCrews = new ReportDataSource { Name = dataSetCrewInfo, Value = list };
                    Logger.log.Debug("[Single] 1");
                    var reportDataSourceBoadings = new ReportDataSource { Name = "DataSetBoardingInfo", Value = listBoarding };
                    Logger.log.Debug("[Single] 2");
                    report1.EnableExternalImages = true;
                    var reportParameter = new ReportParameter("rpt_img", "file:/" + imagePath, true);
                    Logger.log.Debug("[Single] 3");
                    report1.SetParameters(reportParameter);
                    Logger.log.Debug("[Single] 4");
                    report1.DataSources.Add(reportDataSourceCrews);
                    Logger.log.Debug("[Single] 5");
                    report1.DataSources.Add(reportDataSourceBoadings);
                    Logger.log.Debug("[Single] 6");
                    var rendereBytes = report1.Render(fileType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                    Logger.log.Debug("[Single] 7");
                    using (FileStream fs = File.Create(filePathGenerateSingly))
                    {
                        fs.Write(rendereBytes, 0, rendereBytes.Length);
                    }
                    Logger.log.Debug("[Single] 8");
                }

                filePath = ZipHelper.ZipFolder(filePath, filePath + ".zip");
            }
            else
            {
                try
                {
                    Logger.log.Debug("[Single] 11");
                    var fileName = "Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmssffff") + "_" + userId + ".pdf";
                    dir = CheckExist.EnsurePathExist(HttpContext.Current.Server.MapPath(@"~\GenerateReport\GenerateSingle"));
                    Logger.log.Debug("[Single] 22");
                    filePath = String.Format(@"{0}\{1}", dir, fileName);
                    var report = new LocalReport { ReportPath = reportSignlePage };
                    Logger.log.Debug("[Single] 33");
                    var reportDataSourceCrews = new ReportDataSource { Name = dataSetCrewInfo, Value = listCrew };
                    Logger.log.Debug("[Single] 44");
                    var reportDataSourceBoardings = new ReportDataSource { Name = "DataSetBoardingInfo", Value = listBoarding };
                    Logger.log.Debug("[Single] 55");
                    report.EnableExternalImages = true;
                    var pr = new ReportParameter("rpt_img", "file:/" + imagePath, true);
                    Logger.log.Debug("[Single] 66");
                    report.SetParameters(pr);
                    Logger.log.Debug("[Single] 77");
                    report.DataSources.Add(reportDataSourceCrews);
                    Logger.log.Debug("[Single] 88");
                    report.DataSources.Add(reportDataSourceBoardings);
                    Logger.log.Debug("[Single] 99");
                    var rendereBytes = report.Render(fileType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
                    Logger.log.Debug("[Single] 100");
                    using (FileStream fs = File.Create(filePath))
                    {
                        fs.Write(rendereBytes, 0, rendereBytes.Length);
                    }
                }
                catch (Exception ex)
                {
                    Logger.log.Error("[Single] Exception: " + ex.InnerException.Message);
                    throw;
                }
                Logger.log.Debug("[Single] 111");
            }
            Logger.log.Debug("[Single] filePath: " + filePath);
            return filePath;
        }
    }
}
