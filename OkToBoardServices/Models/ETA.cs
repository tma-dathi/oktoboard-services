using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OkToBoardServices.Models
{
    public class ETA
    {
        public int Id { get; set; }

        public System.DateTime Value { get; set; }
        public int VesselId { get; set; }

        public virtual Vessel Vessel { get; set; }
    }
}