using System;
using System.Collections.Generic;
using System.IO;
using Gruberoo;

class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();

    static void Main()
    {
        LoadRestaurants();
        LoadFoodItems();
        Feature3_ListRestaurantsAndMenus();
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }


    static void LoadRestaurants()
    {
        var lines = File.ReadAllLines("restaurants.csv");

        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');

            var restaurant = new Restaurant(
                parts[0],
                parts[1],
                parts[2]
            );

            restaurant.AddMenu(new Menu("M001", "Main Menu"));
            restaurants.Add(restaurant);
        }
    }

    static void LoadFoodItems()
    {
        var lines = File.ReadAllLines("fooditems - Copy.csv");

        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');

            string restaurantId = parts[0];
            string itemName = parts[1];
            string desc = parts[2];
            double price = double.Parse(parts[3]);

            var restaurant = restaurants.Find(r => r.GetRestaurantId() == restaurantId);
            if (restaurant == null) continue;

            var menu = restaurant.GetMenus()[0];
            menu.AddFoodItem(new FoodItem(itemName, desc, price, ""));
        }
    }

    static void Feature3_ListRestaurantsAndMenus()
    {
        Console.WriteLine("All Restaurants and Menu Items");
        Console.WriteLine("==============================");

        foreach (var r in restaurants)
        {
            Console.WriteLine(r.ToString());

            foreach (var m in r.GetMenus())
            {
                m.DisplayFoodItems();
            }

            Console.WriteLine();
        }
    }
}

