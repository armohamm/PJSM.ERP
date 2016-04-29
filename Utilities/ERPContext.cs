namespace PUJASM.ERP.Utilities
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using Models;
    using Models.Accounting;
    using Models.Inventory;
    using Models.Promotion;
    using Models.Purchase;
    using Models.Sales;
    using Models.Salesman;
    using Models.StockCorrection;

    public class ERPContext : DbContext
    {
        public ERPContext()
            : base("name=ERPContext")
        {
            //Database.SetInitializer<ERPContext>(new MigrateDatabaseToLatestVersion<ERPContext, Migrations.Configuration>());
            //Database.SetInitializer<ERPContext>(new DropCreateDatabaseAlways<ERPContext>());
        }

        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<Item> Inventory { get; set; }
        public virtual DbSet<ItemCategory> Categories { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<StockBalance> StockBalances { get; set; }

        public virtual DbSet<Salesman> Salesmans { get; set; }
        public virtual DbSet<SalesCommission> SalesCommissions { get; set; }

        public virtual DbSet<StockAdjustmentTransaction> StockAdjustmentTransactions {get; set; }
        public virtual DbSet<StockAdjustmentTransactionLine> StockAdjustmentTransactionLines { get; set; }
        //public virtual DbSet<MoveStockTransaction> MoveStockTransactions { get; set; }
        //public virtual DbSet<MoveStockTransactionLine> MoveStockTransactionLines { get; set; }

        public virtual DbSet<SalesTransactionLine> SalesTransactionLines { get; set; }
        public virtual DbSet<SalesTransaction> SalesTransactions { get; set; }
        public virtual DbSet<SalesReturnTransaction> SalesReturnTransactions { get; set; }
        public virtual DbSet<SalesReturnTransactionLine> SalesReturnTransactionLines { get; set; }

        public virtual DbSet<PurchaseReturnTransaction> PurchaseReturnTransactions { get; set; }
        public virtual DbSet<PurchaseReturnTransactionLine> PurchaseReturnTransactionLines { get; set; }
        public virtual DbSet<PurchaseTransactionLine> PurchaseTransactionLines { get; set; }
        public virtual DbSet<PurchaseTransaction> PurchaseTransactions { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<LedgerAccount> Ledger_Accounts { get; set; }
        public virtual DbSet<LedgerTransaction> Ledger_Transactions { get; set; }
        public virtual DbSet<LedgerTransactionLine> Ledger_Transaction_Lines { get; set; }
        public virtual DbSet<LedgerGeneral> Ledger_General { get; set; }
        public virtual DbSet<LedgerAccountBalance> Ledger_Account_Balances { get; set; }

        public virtual DbSet<Promotion> Promotion { get; set; }

        public virtual DbSet<Date> Dates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(50, 30));
        }
    }
}
