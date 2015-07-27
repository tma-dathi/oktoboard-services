using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OkToBoardServices.Helper
{
    public static class CheckExist
    {
        public static string EnsurePathExist(string path)
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
                var di = new DirectoryInfo(path);
                if (di.Parent != null) outputPath = String.Format(@"{0}\bin", di.Parent.FullName);
            }
            //WriteLog("output file path: " + outputPath);
            return outputPath;
        }
    }
}