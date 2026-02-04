//==========================================================
// Student Number : S10275267
// Student Name   : Arri
// Partner Name   : yifei
//==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class OrderedFoodItem
    {
        private int qtyOrdered;
        private FoodItem orderedItem;
        private double subTotal;

        public OrderedFoodItem(FoodItem item, int qty)
        {
            orderedItem = item;
            qtyOrdered = qty;
            subTotal = CalculateSubtotal();
        }

        public int GetQtyOrdered() => qtyOrdered;
        public FoodItem GetOrderedItem() => orderedItem;
        public double GetSubTotal() => subTotal;

        public void SetQtyOrdered(int qty)
        {
            qtyOrdered = qty;
            subTotal = CalculateSubtotal();
        }

        public double CalculateSubtotal()
        {
            if (orderedItem == null) return 0;
            return orderedItem.GetItemPrice() * qtyOrdered;
        }

        public override string ToString()
        {
            return $"{orderedItem.GetItemName()} - {qtyOrdered}";
        }
    }
}
