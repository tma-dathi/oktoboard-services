namespace DatabaseSynchronizer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddETDToArrangement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Arrangements", "ETDDate", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Arrangements", "ETDDate");
        }
    }
}
