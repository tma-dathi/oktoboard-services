using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OkToBoardServices.Models
{
    public class GenerateReport
    {
        public class BoardingInfo
        {
            public string origin { get; set; }
            public string time_arrive { get; set; }
            public string flight_code { get; set; }
            public string flight_number { get; set; }
            public string id { get; set; }
            public string contact_number { get; set; }
            public string vessel_name { get; set; }
            public string eta_time { get; set; }
            public string etd_time { get; set; }
            public string report_type { get; set; }
            public int user_id { get; set; }
            public string ship_id { get; set; }
            public string user_name { get; set; }
            public string create_time { get; set; }
            
        }

        public class CrewInfo
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string position { get; set; }
            public string birthday { get; set; }
            public string passport { get; set; }
            public int country_id { get; set; }
            public string gender { get; set; }
            public string id { get; set; }
            public string country { get; set; }
        }

        public class RootObject
        {
            public List<BoardingInfo> boarding_info { get; set; }
            public List<CrewInfo> crew_info { get; set; }
        }
    }
}