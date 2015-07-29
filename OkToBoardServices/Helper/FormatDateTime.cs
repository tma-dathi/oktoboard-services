using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OkToBoardServices.Helper
{
    public static class FormatDateTime
    {
        public static string Populate(DateTime? date, DateTime? time)
        {
            var result = DateTime.Now.ToString("dd-MMM-yyyy HH:mm");
            var d = String.Format("{0:dd-MMM-yyyy}", date);
            var t = String.Format("{0:HH:mm}", time);
            t = String.IsNullOrEmpty(t) ? "00:00" : t;
            result = String.Format("{0} {1}", d, t);
            return result;
        }
    }
}
