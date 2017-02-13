namespace Garage2._0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCheckoutTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicles", "CheckoutTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vehicles", "CheckoutTime");
        }
    }
}
