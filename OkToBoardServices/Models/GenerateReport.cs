using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OkToBoardServices.Models
{
    public class GenerateReport
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string crew_id { get; set; }
        public string gender { get; set; }
        public string position { get; set; }
        public string birthday { get; set; }
        public string birthday_place { get; set; }
        public string passport { get; set; }
        public int country_id { get; set; }
        public int is_flight { get; set; }
        public string time_arrive { get; set; }
        public string flight_code { get; set; }
        public string flight_number { get; set; }
        public int state_id { get; set; }
        public int user_id { get; set; }
        public int batch_id { get; set; }
        public string remark { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string origin { get; set; }
        public string report_type { get; set; }
        public string image { get; set; }
        public int user_admin_id { get; set; }
        public string user_name { get; set; }
        public string vessel_name { get; set; }
        public string eta_time { get; set; }
        public string country { get; set; }
        public string phone_number { get; set; }
        public string date_generate { get; set; }
        public string text_gender { get; set; }
        public string ship_id { get; set; }
        public string etd_time { get; set; }
    }
}