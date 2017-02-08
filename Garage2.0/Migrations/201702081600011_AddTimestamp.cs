namespace Garage2._0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimestamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicles", "Timestamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vehicles", "Timestamp");
        }
    }
}
