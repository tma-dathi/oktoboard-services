using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Ionic.Zip;

namespace OkToBoardServices.Models
{
    public class ZipHelper
    {
        //[DllImport("Ionic.Zip.Reduced.dll", CharSet = CharSet.Unicode)]
        public static string ZipFiles(List<string> files, string zipfile)
        {
            try
            {
                using (var zip = new ZipFile())
                {
                    for (int i = 0; i < files.Count; i++)
                        zip.AddFile(files.ElementAt(i));
                    zip.Save(zipfile);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return zipfile;
        }

        public static string ZipFolder(string srcFolder, string destZipfile)
        {
            try
            {
                using (var zip = new ZipFile())
                {
                    zip.AddDirectory(srcFolder);
                    zip.Save(destZipfile);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return destZipfile;
        }
    }
}