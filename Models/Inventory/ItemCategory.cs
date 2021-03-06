﻿using MVVMFramework;
using System.Collections.Generic;

namespace PUJASM.ERP.Models.Inventory
{
    #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class ItemCategory : ObservableObject
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        #pragma warning disable 659
        public override bool Equals(object obj)
        {
            var category = obj as ItemCategory;
            return category != null && ID.Equals(category.ID);
        }
    }
}
