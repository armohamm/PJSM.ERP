using PUJASM.ERP.Models.Sales;
using System.Collections.ObjectModel;

namespace PUJASM.ERP.Models.Salesman
{
    public class Salesman
    {
        public int ID { get; set; }

        public string Name { get; set; }
       
        public override bool Equals(object obj)
        {
            var salesman = obj as Salesman;
            if (salesman == null) return false;
            else return this.ID.Equals(salesman.ID);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
