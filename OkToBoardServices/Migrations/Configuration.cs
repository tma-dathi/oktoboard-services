namespace OkToBoardServices.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using OkToBoardServices.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<OkToBoardServices.Models.DBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(OkToBoardServices.Models.DBContext context)
        {
            //  This method will be called after migrating to the latest version.

            context.Vessels.AddOrUpdate(
                v => v.Id,
                new Vessel { Name = "Nobita" },
                new Vessel { Name = "Xuka" },
                new Vessel { Name = "Chaien" }
            );
            context.SaveChanges();

            context.ETAs.AddOrUpdate(
                p => p.Id,
                new ETA { Value = DateTime.Today, VesselId = 1 },
                new ETA { Value = DateTime.Today.AddDays(5), VesselId = 1 },
                new ETA { Value = DateTime.Today.AddDays(15), VesselId = 2 },
                new ETA { Value = DateTime.Today.AddDays(1), VesselId = 3 }
            );
        }
    }
}
