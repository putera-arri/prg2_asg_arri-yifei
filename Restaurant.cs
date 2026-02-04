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
    internal class Restaurant
    {
        private string restaurantId;
        private string restaurantName;
        private string restaurantEmail;

        private List<Menu> menus = new List<Menu>();
        private List<SpecialOffer> specialOffers = new List<SpecialOffer>();

        public Restaurant(string id, string name, string email)
        {
            restaurantId = id;
            restaurantName = name;
            restaurantEmail = email;
        }

        public string GetRestaurantId() => restaurantId;
        public string GetRestaurantName() => restaurantName;
        public string GetRestaurantEmail() => restaurantEmail;

        public void DisplayOrders()
        {
            System.Console.WriteLine("Orders display is handled in program features.");
        }

        public void DisplaySpecialOffers()
        {
            foreach (var s in specialOffers)
                System.Console.WriteLine(s.ToString());
        }

        public void DisplayMenu()
        {
            foreach (var m in menus)
                System.Console.WriteLine(m.ToString());
        }

        public void AddMenu(Menu menu)
        {
            if (menu == null) return;
            menus.Add(menu);
        }

        public bool RemoveMenu(Menu menu)
        {
            if (menu == null) return false;
            return menus.Remove(menu);
        }

        public override string ToString()
        {
            return $"{restaurantName} ({restaurantId}) - {restaurantEmail}";
        }

        public List<Menu> GetMenus()
        {
            return menus;
        }

        public void AddSpecialOffer(SpecialOffer offer)
        {
            if (offer == null) return;
            specialOffers.Add(offer);
        }
    }
}
