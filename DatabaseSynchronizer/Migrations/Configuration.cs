namespace DatabaseSynchronizer.Migrations
{
    using DatabaseSynchronizer.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DatabaseSynchronizer.Models.DBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DatabaseSynchronizer.Models.DBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  This method will be called after migrating to the latest version.

            var vesselId_1 = Guid.NewGuid();
            var vesselId_2 = Guid.NewGuid();
            var vesselId_3 = Guid.NewGuid();

            context.Vessels.AddOrUpdate(
                v => v.Id,
                new Vessel { Id = vesselId_1, Name = "Nobita" },
                new Vessel { Id = vesselId_2, Name = "Xuka" },
                new Vessel { Id = vesselId_3, Name = "Chaien" }
            );
            context.SaveChanges();

            var arrangementId_1 = Guid.NewGuid();
            var arrangementId_2 = Guid.NewGuid();
            var arrangementId_3 = Guid.NewGuid();
            var arrangementId_4 = Guid.NewGuid();
            context.Arrangements.AddOrUpdate(
                v => v.Id,
                new Arrangement { Id = arrangementId_1, VesselId = vesselId_1, ETADate = DateTime.Today },
                new Arrangement { Id = arrangementId_2, VesselId = vesselId_1, ETADate = DateTime.Today.AddDays(5) },
                new Arrangement { Id = arrangementId_3, VesselId = vesselId_2, ETADate = DateTime.Today },
                new Arrangement { Id = arrangementId_4, VesselId = vesselId_3, ETADate = DateTime.Today.AddDays(5) }
            );
            context.SaveChanges();
        }
    }
}
