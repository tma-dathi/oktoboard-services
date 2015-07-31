using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using OkToBoardServices.Models;

namespace OkToBoardServices.Helper
{
    public static class GenerateMultipleHelper
    {
        
        private static readonly DBContext Db = new DBContext();
        public static string GetFilePathReport(List<GenerateReport.CrewInfo> listCrew, List<GenerateReport.BoardingInfo> listBoarding)
        {
            var boardingInfomation = listBoarding[0];
            Logger.log.Debug("boardingInfomation: " + boardingInfomation);
            string etaTime = boardingInfomation.eta_time;
            Logger.log.Debug("etaTime: " + etaTime);
            string reportType = boardingInfomation.report_type;
            Vessel vessel = Db.Vessels.Find(new Guid(boardingInfomation.ship_id));
            Logger.log.Debug(String.Format("[Single] Vessel: Name {0} ----- Id {1}", vessel.Name, vessel.Id));
            try
            {
            var etd = vessel.Arrangements.Where(x => FormatDateTime.Populate(x.ETADate, x.ETATime) == etaTime)
                                         .Select(x => FormatDateTime.Populate(x.ETDDate, x.ETDTime)).FirstOrDefault();
                Logger.log.Debug("[Multiple] etd: " + etd);
                boardingInfomation.etd_time = (etd == "00:00") ? "" : etd;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.InnerException.ToString());
            }

            int userId = boardingInfomation.user_id;
            string dir = CheckExist.EnsurePathExist(HttpContext.Current.Server.MapPath(@"~\GenerateReport"));
            CheckExist.EnsurePathExist(HttpContext.Current.Server.MapPath(@"~\GenerateReport\GenerateMultiplePage"));
            Logger.log.Debug("dir: " + dir);
            string fileReport = listCrew.Count <= 4 ? ConfigurationManager.AppSettings["ReportSignlePage"] : ConfigurationManager.AppSettings["ReportMultiplePage"];
            Logger.log.Debug("fileReport: " + fileReport);
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
            string fileType;
            switch(reportType)
            {
                case "doc":
                    fileType = "WORD";
                    break;
                default:
                    fileType = "EXCEL";
                    break;
            }
            
            var fileName = "Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmssffff") + "_" + userId + ".doc";
            Logger.log.Debug("fileName: " + fileName);
            string filePath = String.Format(@"{0}\{1}", dir, fileName);
            Logger.log.Debug("filePath: " + filePath);
            var report = new LocalReport { ReportPath = fileReport };
            Logger.log.Debug("GetFilePathReport 1");
            var reportDataSourceCrews = new ReportDataSource { Name = dataSetCrewInfo, Value = listCrew };
            Logger.log.Debug("GetFilePathReport 2");
            var reportDataSourceBoardings = new ReportDataSource { Name = "DataSetBoardingInfo", Value = listBoarding };
            Logger.log.Debug("GetFilePathReport 3");
            report.EnableExternalImages = true;
            var pr = new ReportParameter("rpt_img", "file:/" + imagePath, true);
            report.SetParameters(pr);
            report.DataSources.Add(reportDataSourceCrews);
            Logger.log.Debug("GetFilePathReport 4");
            report.DataSources.Add(reportDataSourceBoardings);
            Logger.log.Debug("GetFilePathReport 5");
            byte[] rendereBytes = null;
            try
            {
                rendereBytes = report.Render(fileType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            }
            catch (Exception ex)
            {
                Logger.log.Error("GetFilePathReport error: " + ex.InnerException.ToString());
            }
            Logger.log.Debug("GetFilePathReport 6");
            using (FileStream fs = File.Create(filePath))
            {
                fs.Write(rendereBytes, 0, rendereBytes.Length);
                Logger.log.Debug("GetFilePathReport 7");
            }
            return filePath;
        }

    }
}
