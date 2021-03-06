﻿using MVVMFramework;
using PUJASM.ERP.Models.Inventory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PUJASM.ERP.Models.Purchase
{
    public class PurchaseReturnTransactionLine : ObservableObject
    {
        int _quantity;

        [Key]
        [Column("PurchaseReturnTransactionID", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string PurchaseReturnTransactionID { get; set; }

        [Key]
        [Column("ItemID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ItemID { get; set; }

        [Key]
        [Column("WarehouseID", Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WarehouseID { get; set; }

        [Key]
        [Column("ReturnWarehouseID", Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ReturnWarehouseID { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal PurchasePrice { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal Discount { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public decimal ReturnPrice { get; set; }

        [Required]
        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value, "Quantity"); }
        }

        [Required]
        public decimal Total { get; set; }

        [ForeignKey("ItemID")]
        public virtual Item Item { get; set; }

        [ForeignKey("WarehouseID")]
        public virtual Warehouse Warehouse { get; set; }

        [ForeignKey("ReturnWarehouseID")]
        public virtual Warehouse ReturnWarehouse { get; set; }

        [ForeignKey("PurchaseReturnTransactionID")]
        public virtual PurchaseReturnTransaction PurchaseReturnTransaction { get; set; }
    }
}
