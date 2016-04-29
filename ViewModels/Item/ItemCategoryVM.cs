namespace PUJASM.ERP.ViewModels.Item
{
    using Models.Inventory;
    using MVVMFramework;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class ItemCategoryVM : ViewModelBase<ItemCategory>
    {
        public int ID => Model.ID;

        public string Name => Model.Name;

        #pragma warning disable 659
        public override bool Equals(object obj)
        {
            var category = obj as ItemCategoryVM;
            return category != null && ID.Equals(category.ID);
        }
    }
}
