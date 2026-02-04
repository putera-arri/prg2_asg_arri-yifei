using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class FoodItem
    {
        private string itemName;
        private string itemDesc;
        private double itemPrice;
        private string customis;

        public FoodItem(string name, string desc, double price, string customisations)
        {
            itemName = name;
            itemDesc = desc;
            itemPrice = price;
            customis = customisations;
        }

        public string GetItemName() => itemName;
        public string GetItemDesc() => itemDesc;
        public double GetItemPrice() => itemPrice;
        public string GetCustomis() => customis;

        public override string ToString()
        {
            return $"{itemName}: {itemDesc} - ${itemPrice:0.00}";
        }
    }
}
