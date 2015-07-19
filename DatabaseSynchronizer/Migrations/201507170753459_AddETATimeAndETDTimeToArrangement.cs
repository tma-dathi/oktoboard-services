namespace DatabaseSynchronizer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddETATimeAndETDTimeToArrangement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Arrangements", "ETATime", c => c.DateTime(nullable: true));
            AddColumn("dbo.Arrangements", "ETDTime", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Arrangements", "ETATime");
            DropColumn("dbo.Arrangements", "ETDTime");
        }
    }
}
