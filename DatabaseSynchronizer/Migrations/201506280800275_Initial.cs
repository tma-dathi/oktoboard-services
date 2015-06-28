namespace DatabaseSynchronizer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Vessels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Arrangements",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ETADate = c.DateTime(nullable: false),
                        VesselId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vessels", t => t.VesselId, cascadeDelete: true)
                .Index(t => t.VesselId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Arrangements", new[] { "VesselId" });
            DropForeignKey("dbo.Arrangements", "VesselId", "dbo.Vessels");
            DropTable("dbo.Arrangements");
            DropTable("dbo.Vessels");
        }
    }
}
