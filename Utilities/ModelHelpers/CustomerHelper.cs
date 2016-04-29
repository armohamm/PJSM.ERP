namespace PUJASM.ERP.Utilities.ModelHelpers
{
    using System.Transactions;
    using Models;
    using System.Linq;

    public static class CustomerHelper
    {
        public static void AddCustomerAlongWithItsLedgerToDatabase(Customer customer)
        {
            using (var context = new ERPContext())
            {
                context.Customers.Add(customer);
                context.SaveChanges();
            }
        }

        public static void SaveCustomerEditsToDatabase(Customer editingCustomer, Customer editedCustomer)
        {
            using (var ts = new TransactionScope())
            {
                var context = new ERPContext();
                SaveCustomerEditsToDatabaseContext(context, editingCustomer, editedCustomer);
                ts.Complete();
            }
        }

        private static void SaveCustomerEditsToDatabaseContext(ERPContext context, Customer editingCustomer, Customer editedCustomer)
        {
            editingCustomer = context.Customers.Single(customer => customer.ID.Equals(editingCustomer.ID));
            DeepCopyCustomerProperties(editedCustomer, ref editingCustomer);
            context.SaveChanges();
        }

        public static void DeepCopyCustomerProperties(Customer fromCustomer, ref Customer toCustomer)
        {
            toCustomer.FirstName = fromCustomer.FirstName;
            toCustomer.LastName = fromCustomer.LastName;
            toCustomer.Address = fromCustomer.Address;
            toCustomer.Telephone = fromCustomer.Telephone;
            toCustomer.Email = fromCustomer.Email;
        }
    }
}
