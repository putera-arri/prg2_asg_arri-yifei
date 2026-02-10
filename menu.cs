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
    internal class Menu
    {
        private string menuId;
        private string menuName;

        private List<FoodItem> foodItems = new List<FoodItem>();

        public Menu(string id, string name)
        {
            menuId = id;
            menuName = name;
        }

        public void AddFoodItem(FoodItem item)
        {
            if (item == null) return;
            foodItems.Add(item);
        }

        public bool RemoveFoodItem(FoodItem item)
        {
            if (item == null) return false;
            return foodItems.Remove(item);
        }

        public void DisplayFoodItems()
        {
            foreach (var f in foodItems)
                System.Console.WriteLine(f.ToString());
        }

        public override string ToString()
        {
            return $"{menuName} ({menuId}) - {foodItems.Count} items";
        }

        public List<FoodItem> GetFoodItems()
        {
            return foodItems;
        }
    }
}
