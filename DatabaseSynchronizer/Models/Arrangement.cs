using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DatabaseSynchronizer.Models
{
    public class Arrangement
    {
        public Guid Id { get; set; }
        public DateTime ETADate { get; set; }

        [Required]
        public Guid VesselId { get; set; }

        public virtual Vessel Vessel { get; set; }
    }
}