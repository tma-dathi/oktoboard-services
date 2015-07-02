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
        public bool gender { get; set; }
        public string position { get; set; }
        public string passport { get; set; }
        public int country_id { get; set; }
        public int is_flight { get; set; }
        public string time_arrive { get; set; }
        public string flight_code { get; set; }
        public string flight_number { get; set; }
        public int state_id { get; set; }
        public int user_id { get; set; }
        public int batch_id { get; set; }
        public string origin { get; set; }
    }
}