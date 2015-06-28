using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DatabaseSynchronizer.Models
{
    public class DBContext : DbContext
    {
        public DBContext() : base("name=OTBService") { }
        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<Arrangement> Arrangements { get; set; }
    }
}