namespace PUJASM.ERP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update300 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockAdjustmentTransactionLines",
                c => new
                    {
                        StockAdjustmentTransactionID = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ItemID = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        WarehouseID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StockAdjustmentTransactionID, t.ItemID, t.WarehouseID })
                .ForeignKey("dbo.Inventory", t => t.ItemID, cascadeDelete: true)
                .ForeignKey("dbo.StockAdjustmentTransactions", t => t.StockAdjustmentTransactionID, cascadeDelete: true)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseID, cascadeDelete: true)
                .Index(t => t.StockAdjustmentTransactionID)
                .Index(t => t.ItemID)
                .Index(t => t.WarehouseID);
            
            CreateTable(
                "dbo.StockAdjustmentTransactions",
                c => new
                    {
                        StockAdjustmentTransactionID = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Date = c.DateTime(nullable: false, precision: 0),
                        Description = c.String(unicode: false),
                        User_Username = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.StockAdjustmentTransactionID)
                .ForeignKey("dbo.Users", t => t.User_Username)
                .Index(t => t.User_Username);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockAdjustmentTransactionLines", "WarehouseID", "dbo.Warehouses");
            DropForeignKey("dbo.StockAdjustmentTransactions", "User_Username", "dbo.Users");
            DropForeignKey("dbo.StockAdjustmentTransactionLines", "StockAdjustmentTransactionID", "dbo.StockAdjustmentTransactions");
            DropForeignKey("dbo.StockAdjustmentTransactionLines", "ItemID", "dbo.Inventory");
            DropIndex("dbo.StockAdjustmentTransactions", new[] { "User_Username" });
            DropIndex("dbo.StockAdjustmentTransactionLines", new[] { "WarehouseID" });
            DropIndex("dbo.StockAdjustmentTransactionLines", new[] { "ItemID" });
            DropIndex("dbo.StockAdjustmentTransactionLines", new[] { "StockAdjustmentTransactionID" });
            DropTable("dbo.StockAdjustmentTransactions");
            DropTable("dbo.StockAdjustmentTransactionLines");
        }
    }
}
