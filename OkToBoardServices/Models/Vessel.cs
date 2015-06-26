using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OkToBoardServices.Models
{
    public class Vessel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<ETA> ETAs { get; set; }
    }
}