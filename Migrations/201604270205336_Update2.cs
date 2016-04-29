namespace PUJASM.ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SalesTransactions", "Paid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalesTransactions", "Paid", c => c.Decimal(nullable: false, precision: 50, scale: 30));
        }
    }
}
