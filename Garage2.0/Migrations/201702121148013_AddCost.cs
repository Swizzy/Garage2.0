namespace Garage2._0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicles", "Cost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vehicles", "Cost");
        }
    }
}
