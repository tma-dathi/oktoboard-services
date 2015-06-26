namespace OkToBoardServices.Migrations
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
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ETAs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.DateTime(nullable: false),
                        VesselId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vessels", t => t.VesselId, cascadeDelete: true)
                .Index(t => t.VesselId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ETAs", new[] { "VesselId" });
            DropForeignKey("dbo.ETAs", "VesselId", "dbo.Vessels");
            DropTable("dbo.ETAs");
            DropTable("dbo.Vessels");
        }
    }
}
