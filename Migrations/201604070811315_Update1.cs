namespace PUJASM.ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update1 : DbMigration
    {
        public override void Up()
        {
           
            CreateTable(
                "dbo.SalesTransactionLines",
                c => new
                    {
                        SalesTransactionID = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ItemID = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        WarehouseID = c.Int(nullable: false),
                        SalesPrice = c.Decimal(nullable: false, precision: 50, scale: 30),
                        Discount = c.Decimal(nullable: false, precision: 50, scale: 30),
                        Quantity = c.Int(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 50, scale: 30),
                    })
                .PrimaryKey(t => new { t.SalesTransactionID, t.ItemID, t.WarehouseID, t.SalesPrice, t.Discount })
                .ForeignKey("dbo.Inventory", t => t.ItemID, cascadeDelete: true)
                .ForeignKey("dbo.SalesTransactions", t => t.SalesTransactionID, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseID, cascadeDelete: true)
                .Index(t => t.SalesTransactionID)
                .Index(t => t.ItemID)
                .Index(t => t.WarehouseID);
            

        }
        
        public override void Down() { }
    }
}
