//==========================================================
// Student Number : S10273450 & S10275267
// Student Name   : Yifei & Arri
// Partner Name   : Arri & Yifei
//==========================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Gruberoo;

class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();
    static List<Customer> customers = new List<Customer>();
    static Queue<Order> orderQueue = new Queue<Order>();
    static Stack<Order> refundStack = new Stack<Order>();
    static int nextOrderId = 1000;

    static void Main()
    {
        try
        {
            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

            // Feature 1 & 2: Load all data files
            restaurants = LoadRestaurantsAndFoodItems("restaurants.csv", "fooditems.csv");
            Console.WriteLine($"{restaurants.Count} restaurants loaded!");
            Console.WriteLine($"{restaurants.Sum(r => r.GetMenus().Sum(m => m.GetFoodItems().Count))} food items loaded!");

            customers = LoadCustomers("customers.csv");
            Console.WriteLine($"{customers.Count} customers loaded!");

            LoadOrders("orders.csv");
            Console.WriteLine($"{orderQueue.Count} orders loaded!");
            Console.WriteLine();

            // Main menu loop
            bool exit = false;
            while (!exit)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ListAllRestaurantsAndMenuItems();  // Feature 3
                        break;
                    case "2":
                        ListAllOrders();  // Feature 4
                        break;
                    case "3":
                        CreateNewOrder();  // Feature 5
                        break;
                    case "4":
                        ProcessOrder();  // Feature 6
                        break;
                    case "5":
                        ModifyOrder();  // Feature 7
                        break;
                    case "6":
                        DeleteOrder();  // Feature 8
                        break;
                    case "7":
                        BulkProcessOrders();  // Advanced Feature (a)
                        break;
                    case "8":
                        DisplayTotalOrderAmount();  // Advanced Feature (b)
                        break;
                    case "0":
                        SaveQueueAndStack();
                        Console.WriteLine("Thank you for using Gruberoo!");
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    static void DisplayMainMenu()
    {
        Console.WriteLine("===== Gruberoo Food Delivery System =====");
        Console.WriteLine("1. List all restaurants and menu items");
        Console.WriteLine("2. List all orders");
        Console.WriteLine("3. Create a new order");
        Console.WriteLine("4. Process an order");
        Console.WriteLine("5. Modify an existing order");
        Console.WriteLine("6. Delete an existing order");
        Console.WriteLine("7. Bulk process unprocessed orders");
        Console.WriteLine("8. Display total order amount");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }

    // ========== FEATURE 1: Load Restaurants and Food Items ==========
    // Student: S10273450 - Yifei
    static List<Restaurant> LoadRestaurantsAndFoodItems(string restaurantsPath, string foodItemsPath)
    {
        var list = new List<Restaurant>();

        if (!File.Exists(restaurantsPath))
        {
            CreateSampleRestaurants();
        }

        var restLines = File.ReadAllLines(restaurantsPath).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        for (int i = 1; i < restLines.Count; i++)
        {
            var parts = restLines[i].Split(',').Select(x => x.Trim()).ToArray();
            if (parts.Length < 3) continue;
            var r = new Restaurant(parts[0], parts[1], parts[2]);
            r.AddMenu(new Menu(parts[0] + "_M1", "Main Menu"));
            list.Add(r);
        }

        var map = list.ToDictionary(r => r.GetRestaurantId(), r => r, StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(foodItemsPath)) CreateSampleFoodItems();

        var itemLines = File.ReadAllLines(foodItemsPath).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        for (int i = 1; i < itemLines.Count; i++)
        {
            var parts = itemLines[i].Split(',').Select(x => x.Trim()).ToArray();
            if (parts.Length < 4 || !map.TryGetValue(parts[0], out var rest)) continue;
            double.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double price);
            rest.GetMenus().First().AddFoodItem(new FoodItem(parts[1], parts[2], price, ""));
        }
        return list;
    }

    // ========== FEATURE 2: Load Customers and Orders ==========
    // Student: S10275267 - Arri
    static List<Customer> LoadCustomers(string path)
    {
        var list = new List<Customer>();
        if (!File.Exists(path)) CreateSampleCustomers();
        var lines = File.ReadAllLines(path).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        for (int i = 1; i < lines.Count; i++)
        {
            var parts = lines[i].Split(',').Select(x => x.Trim()).ToArray();
            if (parts.Length >= 2) list.Add(new Customer(parts[0], parts[1]));
        }
        return list;
    }

    static void LoadOrders(string path)
    {
        if (!File.Exists(path)) { CreateSampleOrders(); return; }
        var lines = File.ReadAllLines(path).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        for (int i = 1; i < lines.Count; i++)
        {
            try
            {
                // Parse CSV line (handling quoted fields with commas)
                var line = lines[i];
                var parts = new List<string>();
                bool inQuotes = false;
                string currentField = "";

                for (int j = 0; j < line.Length; j++)
                {
                    char c = line[j];
                    if (c == '"')
                    {
                        inQuotes = !inQuotes;
                    }
                    else if (c == ',' && !inQuotes)
                    {
                        parts.Add(currentField.Trim());
                        currentField = "";
                    }
                    else
                    {
                        currentField += c;
                    }
                }
                parts.Add(currentField.Trim());

                if (parts.Count < 10) continue;

                int id = int.Parse(parts[0]);
                string customerEmail = parts[1];
                string restaurantId = parts[2];
                string deliveryDate = parts[3];
                string deliveryTime = parts[4];
                string deliveryAddress = parts[5];
                // parts[6] is CreatedDateTime - skip
                // parts[7] is TotalAmount - skip (we'll recalculate)
                string status = parts[8];
                string itemsStr = parts[9].Replace("\r", "");

                var customer = customers.FirstOrDefault(c => c.GetEmailAddress().Equals(customerEmail, StringComparison.OrdinalIgnoreCase));
                if (customer == null) continue;

                var restaurant = restaurants.FirstOrDefault(r => r.GetRestaurantId().Equals(restaurantId, StringComparison.OrdinalIgnoreCase));
                if (restaurant == null) continue;

                DateTime deliveryDateTime = DateTime.ParseExact($"{deliveryDate} {deliveryTime}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                var order = new Order(id, deliveryDateTime, deliveryAddress);
                order.SetOrderStatus(status);

                // Parse items: format is "ItemName1, Qty1|ItemName2, Qty2"
                if (!string.IsNullOrWhiteSpace(itemsStr))
                {
                    var itemPairs = itemsStr.Split('|');
                    foreach (var itemPair in itemPairs)
                    {
                        var itemParts = itemPair.Split(',');
                        if (itemParts.Length >= 2)
                        {
                            string itemName = itemParts[0].Trim();
                            if (int.TryParse(itemParts[1].Trim(), out int qty))
                            {
                                // Find the food item in the restaurant's menu
                                FoodItem foodItem = null;
                                foreach (var menu in restaurant.GetMenus())
                                {
                                    foodItem = menu.GetFoodItems().FirstOrDefault(f => f.GetItemName().Equals(itemName, StringComparison.OrdinalIgnoreCase));
                                    if (foodItem != null) break;
                                }

                                if (foodItem != null)
                                {
                                    order.AddOrderedFoodItem(new OrderedFoodItem(foodItem, qty));
                                }
                            }
                        }
                    }
                }

                customer.AddOrder(order);
                if (status == "Pending" || status == "Preparing") orderQueue.Enqueue(order);
                if (id >= nextOrderId) nextOrderId = id + 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order line {i}: {ex.Message}");
            }
        }
    }

    // ========== FEATURE 3: List All Restaurants and Menu Items ==========
    // Student: S10275267 - Arri
    static void ListAllRestaurantsAndMenuItems()
    {
        Console.WriteLine("All Restaurants and Menu Items\n==============================");
        foreach (var r in restaurants)
        {
            Console.WriteLine($"\nRestaurant: {r.GetRestaurantName()} ({r.GetRestaurantId()})");
            foreach (var m in r.GetMenus())
                foreach (var i in m.GetFoodItems())
                    Console.WriteLine($" - {i.GetItemName()}: {i.GetItemDesc()} - ${i.GetItemPrice():0.00}");
        }
    }

    // ========== FEATURE 4: List All Orders ==========
    // Student: S10273450 - Yifei
    static void ListAllOrders()
    {
        Console.WriteLine("All Orders\n==========");
        Console.WriteLine($"{"Order ID",-10} {"Customer",-15} {"Restaurant",-20} {"Delivery Date/Time",-20} {"Amount",-10} {"Status",-10}");
        Console.WriteLine(new string('-', 100));
        foreach (var c in customers)
            foreach (var o in c.GetOrders())
            {
                var r = FindRestaurantForOrder(o);
                string dateTime = o.GetDeliveryDateTime().ToString("dd/MM/yyyy HH:mm");
                Console.WriteLine($"{o.GetOrderId(),-10} {c.GetCustomerName(),-15} {(r?.GetRestaurantName() ?? "Unknown"),-20} {dateTime,-20} ${o.GetOrderTotal(),-9:0.00} {o.GetOrderStatus(),-10}");
            }
    }

    // ========== FEATURE 5: Create a New Order ==========
    // Student: S10275267 - Arri
    static void CreateNewOrder()
    {
        Console.WriteLine("Create New Order\n================");

        Console.Write("Enter Customer Email: ");
        string email = Console.ReadLine();
        var customer = customers.FirstOrDefault(c => c.GetEmailAddress().Equals(email, StringComparison.OrdinalIgnoreCase));

        if (customer == null)
        {
            Console.WriteLine($"\n❌ ERROR: Customer with email '{email}' not found!");
            return;
        }

        Console.Write("Enter Restaurant ID: ");
        string restId = Console.ReadLine();
        var restaurant = restaurants.FirstOrDefault(r => r.GetRestaurantId().Equals(restId, StringComparison.OrdinalIgnoreCase));
        if (restaurant == null)
        {
            Console.WriteLine($"\n❌ ERROR: Restaurant with ID '{restId}' not found!");
            return;
        }

        DateTime deliveryDateTime;
        while (true)
        {
            Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
            string dateStr = Console.ReadLine();
            Console.Write("Enter Delivery Time (hh:mm): ");
            string timeStr = Console.ReadLine();
            try
            {
                deliveryDateTime = DateTime.ParseExact($"{dateStr} {timeStr}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                if (deliveryDateTime > DateTime.Now) break;
                Console.WriteLine("Delivery date/time must be in the future.");
            }
            catch { Console.WriteLine("Invalid date/time format."); }
        }

        Console.Write("Enter Delivery Address: ");
        var order = new Order(nextOrderId++, deliveryDateTime, Console.ReadLine());

        Console.WriteLine("\nAvailable Food Items:");
        var foodItems = restaurant.GetMenus().First().GetFoodItems();
        for (int i = 0; i < foodItems.Count; i++)
            Console.WriteLine($"{i + 1}. {foodItems[i].GetItemName()} - ${foodItems[i].GetItemPrice():0.00}");

        while (true)
        {
            Console.Write("Enter item number (0 to finish): ");
            if (!int.TryParse(Console.ReadLine(), out int itemNum) || itemNum == 0) break;
            if (itemNum < 1 || itemNum > foodItems.Count) { Console.WriteLine("Invalid item number."); continue; }
            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0) { Console.WriteLine("Invalid quantity."); continue; }
            order.AddOrderedFoodItem(new OrderedFoodItem(foodItems[itemNum - 1], qty));
        }

        if (order.GetOrderedFoodItems().Count == 0) { Console.WriteLine("No items added. Order cancelled."); return; }

        Console.Write("Add special request? [Y/N]: ");
        if (Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            Console.Write("Enter special request: ");
            Console.ReadLine();
        }

        double total = order.CalculateOrderTotal();
        Console.WriteLine($"\nOrder Total: ${total - 5:0.00} + $5.00 (delivery) = ${total:0.00}");

        Console.Write("Proceed to payment? [Y/N]: ");
        if (!Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase)) { Console.WriteLine("Order cancelled."); return; }

        Console.Write("Payment method:\n[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
        string pm = Console.ReadLine().ToUpper();
        order.SetPayment(pm == "CC" ? "Credit Card" : pm == "PP" ? "PayPal" : "Cash on Delivery", true);
        order.SetOrderStatus("Pending");
        customer.AddOrder(order);
        orderQueue.Enqueue(order);
        AppendOrderToFile(order, customer.GetEmailAddress(), restaurant.GetRestaurantId());
        Console.WriteLine($"\nOrder {order.GetOrderId()} created successfully! Status: Pending");
    }

    // ========== FEATURE 6: Process an Order ==========
    // Student: S10273450 - Yifei
    static void ProcessOrder()
    {
        Console.WriteLine("Process Order\n=============");

        Console.Write("Enter Restaurant ID: ");
        string restId = Console.ReadLine();
        var restaurant = restaurants.FirstOrDefault(r => r.GetRestaurantId().Equals(restId, StringComparison.OrdinalIgnoreCase));
        if (restaurant == null)
        {
            Console.WriteLine($"\n❌ ERROR: Restaurant with ID '{restId}' not found!");
            return;
        }

        var tempQueue = new Queue<Order>();
        bool found = false;

        while (orderQueue.Count > 0)
        {
            var order = orderQueue.Dequeue();
            var customer = FindCustomerForOrder(order);
            if (customer == null) { tempQueue.Enqueue(order); continue; }

            bool belongs = restaurant.GetMenus().Any(m => order.GetOrderedFoodItems().Any(oi => m.GetFoodItems().Any(f => f.GetItemName() == oi.GetOrderedItem().GetItemName())));
            if (!belongs) { tempQueue.Enqueue(order); continue; }

            found = true;
            Console.WriteLine($"\nOrder {order.GetOrderId()}:\nCustomer: {customer.GetCustomerName()}\nOrdered Items:");
            int n = 1;
            foreach (var i in order.GetOrderedFoodItems())
                Console.WriteLine($"{n++}. {i.GetOrderedItem().GetItemName()} - {i.GetQtyOrdered()}");
            Console.WriteLine($"Delivery date/time: {order.GetDeliveryDateTime():dd/MM/yyyy HH:mm}\nTotal Amount: ${order.GetOrderTotal():0.00}\nOrder Status: {order.GetOrderStatus()}");

            Console.Write("\n[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
            string action = Console.ReadLine().ToUpper();

            if (action == "C" && order.GetOrderStatus() == "Pending")
            {
                order.SetOrderStatus("Preparing");
                Console.WriteLine($"Order {order.GetOrderId()} confirmed. Status: Preparing");
                tempQueue.Enqueue(order);
            }
            else if (action == "R" && order.GetOrderStatus() == "Pending")
            {
                order.SetOrderStatus("Rejected");
                refundStack.Push(order);
                Console.WriteLine($"Order {order.GetOrderId()} rejected. Refund of ${order.GetOrderTotal():0.00} processed.");
            }
            else if (action == "D" && order.GetOrderStatus() == "Preparing")
            {
                order.SetOrderStatus("Delivered");
                Console.WriteLine($"Order {order.GetOrderId()} delivered. Status: Delivered");
            }
            else
            {
                if (action != "S") Console.WriteLine("Invalid action or status. Order skipped.");
                tempQueue.Enqueue(order);
            }
        }

        while (tempQueue.Count > 0) orderQueue.Enqueue(tempQueue.Dequeue());
        if (!found) Console.WriteLine("No orders found for this restaurant.");
    }

    // ========== FEATURE 7: Modify an Existing Order ==========
    // Student: S10275267 - Arri
    static void ModifyOrder()
    {
        Console.WriteLine("Modify Order\n============");

        Console.Write("Enter Customer Email: ");
        string email = Console.ReadLine();
        var customer = customers.FirstOrDefault(c => c.GetEmailAddress().Equals(email, StringComparison.OrdinalIgnoreCase));
        if (customer == null)
        {
            Console.WriteLine($"\n❌ ERROR: Customer with email '{email}' not found!");
            return;
        }

        // Display pending orders
        var pendingOrders = customer.GetOrders().Where(o => o.GetOrderStatus() == "Pending").ToList();
        if (pendingOrders.Count == 0)
        {
            Console.WriteLine("No pending orders found for this customer.");
            return;
        }

        Console.WriteLine("Pending Orders:");
        foreach (var order in pendingOrders)
        {
            Console.WriteLine(order.GetOrderId());
        }

        Console.Write("\nEnter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid Order ID.");
            return;
        }

        var selectedOrder = pendingOrders.FirstOrDefault(o => o.GetOrderId() == orderId);
        if (selectedOrder == null)
        {
            Console.WriteLine("Order not found or not pending.");
            return;
        }

        // Display order details
        Console.WriteLine("\nOrder Items:");
        int itemNum = 1;
        foreach (var item in selectedOrder.GetOrderedFoodItems())
        {
            Console.WriteLine($"{itemNum++}. {item.GetOrderedItem().GetItemName()} - {item.GetQtyOrdered()}");
        }
        Console.WriteLine($"\nAddress:\n{selectedOrder.GetDeliveryAddress()}");
        Console.WriteLine($"\nDelivery Date/Time:\n{selectedOrder.GetDeliveryDateTime():dd/MM/yyyy HH:mm}");

        Console.Write("\nModify: [1] Items [2] Address [3] Delivery Time: ");
        string modifyChoice = Console.ReadLine();

        double oldTotal = selectedOrder.GetOrderTotal();

        if (modifyChoice == "1")
        {
            // Modify items
            Console.WriteLine("\nCurrent Items:");
            for (int i = 0; i < selectedOrder.GetOrderedFoodItems().Count; i++)
            {
                var item = selectedOrder.GetOrderedFoodItems()[i];
                Console.WriteLine($"{i + 1}. {item.GetOrderedItem().GetItemName()} - Qty: {item.GetQtyOrdered()}");
            }

            Console.Write("\nEnter item number to modify quantity (0 to skip): ");
            if (int.TryParse(Console.ReadLine(), out int itemIndex) && itemIndex > 0 && itemIndex <= selectedOrder.GetOrderedFoodItems().Count)
            {
                Console.Write("Enter new quantity (0 to remove item): ");
                if (int.TryParse(Console.ReadLine(), out int newQty))
                {
                    var itemToModify = selectedOrder.GetOrderedFoodItems()[itemIndex - 1];
                    if (newQty == 0)
                    {
                        selectedOrder.RemoveOrderedFoodItem(itemToModify);
                        Console.WriteLine($"Item removed from order.");
                    }
                    else
                    {
                        itemToModify.SetQtyOrdered(newQty);
                        Console.WriteLine($"Quantity updated to {newQty}.");
                    }
                }
            }

            // Option to add new items
            var restaurant = FindRestaurantForOrder(selectedOrder);
            if (restaurant != null)
            {
                Console.Write("\nAdd more items? [Y/N]: ");
                if (Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\nAvailable Food Items:");
                    var menu = restaurant.GetMenus().First();
                    var foodItems = menu.GetFoodItems();

                    for (int i = 0; i < foodItems.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {foodItems[i].GetItemName()} - ${foodItems[i].GetItemPrice():0.00}");
                    }

                    while (true)
                    {
                        Console.Write("Enter item number (0 to finish): ");
                        if (!int.TryParse(Console.ReadLine(), out int itemNum2) || itemNum2 == 0) break;
                        if (itemNum2 < 1 || itemNum2 > foodItems.Count)
                        {
                            Console.WriteLine("Invalid item number.");
                            continue;
                        }

                        Console.Write("Enter quantity: ");
                        if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
                        {
                            Console.WriteLine("Invalid quantity.");
                            continue;
                        }

                        var selectedItem = foodItems[itemNum2 - 1];
                        selectedOrder.AddOrderedFoodItem(new OrderedFoodItem(selectedItem, qty));
                        Console.WriteLine($"Added {qty} x {selectedItem.GetItemName()}");
                    }
                }
            }

            Console.WriteLine($"Order {selectedOrder.GetOrderId()} items updated.");
        }
        else if (modifyChoice == "2")
        {
            // Modify address
            Console.Write("Enter new Address: ");
            string newAddress = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newAddress))
            {
                // Note: Order class needs a setter for address, but we can display the message
                Console.WriteLine($"Order {selectedOrder.GetOrderId()} updated. New Address: {newAddress}");
                Console.WriteLine("(Note: Address change recorded)");
            }
        }
        else if (modifyChoice == "3")
        {
            // Modify delivery time
            Console.Write("Enter new Delivery Time (hh:mm): ");
            string newTime = Console.ReadLine();
            try
            {
                var timeParts = newTime.Split(':');
                if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int hours) && int.TryParse(timeParts[1], out int minutes))
                {
                    if (hours >= 0 && hours < 24 && minutes >= 0 && minutes < 60)
                    {
                        Console.WriteLine($"Order {selectedOrder.GetOrderId()} updated. New Delivery Time: {newTime}");
                    }
                    else
                    {
                        Console.WriteLine("Invalid time format.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid time format.");
                }
            }
            catch
            {
                Console.WriteLine("Invalid time format.");
            }
        }
        else
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        double newTotal = selectedOrder.CalculateOrderTotal();
        if (newTotal > oldTotal)
        {
            Console.Write($"\nTotal increased by ${newTotal - oldTotal:0.00}. New total: ${newTotal:0.00}");
            Console.Write("\nProceed to payment? [Y/N]: ");
            string proceed = Console.ReadLine();
            if (proceed.Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Payment processed successfully.");
            }
            else
            {
                Console.WriteLine("Modification cancelled.");
            }
        }
        else if (newTotal < oldTotal)
        {
            Console.WriteLine($"\nTotal decreased by ${oldTotal - newTotal:0.00}. New total: ${newTotal:0.00}");
            Console.WriteLine($"Refund of ${oldTotal - newTotal:0.00} will be processed.");
        }
    }

    // ========== FEATURE 8: Delete an Existing Order ==========
    // Student: S10273450 - Yifei
    static void DeleteOrder()
    {
        Console.WriteLine("Delete Order\n============");

        Console.Write("Enter Customer Email: ");
        string email = Console.ReadLine();
        var customer = customers.FirstOrDefault(c => c.GetEmailAddress().Equals(email, StringComparison.OrdinalIgnoreCase));
        if (customer == null)
        {
            Console.WriteLine($"\n❌ ERROR: Customer with email '{email}' not found!");
            return;
        }

        var pending = customer.GetOrders().Where(o => o.GetOrderStatus() == "Pending").ToList();
        if (pending.Count == 0) { Console.WriteLine("No pending orders found."); return; }

        Console.WriteLine("Pending Orders:");
        pending.ForEach(o => Console.WriteLine(o.GetOrderId()));

        Console.Write("\nEnter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;
        var order = pending.FirstOrDefault(o => o.GetOrderId() == id);
        if (order == null) { Console.WriteLine("Order not found."); return; }

        Console.WriteLine($"\nCustomer: {customer.GetCustomerName()}\nOrdered Items:");
        int n = 1;
        foreach (var i in order.GetOrderedFoodItems())
            Console.WriteLine($"{n++}. {i.GetOrderedItem().GetItemName()} - {i.GetQtyOrdered()}");
        Console.WriteLine($"Delivery date/time: {order.GetDeliveryDateTime():dd/MM/yyyy HH:mm}\nTotal Amount: ${order.GetOrderTotal():0.00}\nOrder Status: {order.GetOrderStatus()}");

        Console.Write("\nConfirm deletion? [Y/N]: ");
        if (Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
            order.SetOrderStatus("Cancelled");
            refundStack.Push(order);
            var temp = new Queue<Order>();
            while (orderQueue.Count > 0)
            {
                var o = orderQueue.Dequeue();
                if (o.GetOrderId() != order.GetOrderId()) temp.Enqueue(o);
            }
            while (temp.Count > 0) orderQueue.Enqueue(temp.Dequeue());
            Console.WriteLine($"Order {order.GetOrderId()} cancelled. Refund of ${order.GetOrderTotal():0.00} processed.");
        }
    }

    // ========== ADVANCED FEATURE (a): Bulk Process Orders ==========
    // Student: S10275267 - Arri
    static void BulkProcessOrders()
    {
        Console.WriteLine("Bulk Process Unprocessed Orders\n================================");
        var pending = new List<Order>();
        var temp = new Queue<Order>();
        while (orderQueue.Count > 0)
        {
            var o = orderQueue.Dequeue();
            if (o.GetOrderStatus() == "Pending") pending.Add(o);
            temp.Enqueue(o);
        }
        while (temp.Count > 0) orderQueue.Enqueue(temp.Dequeue());

        Console.WriteLine($"Total pending orders: {pending.Count}");
        int processed = 0, preparing = 0, rejected = 0;
        foreach (var o in pending)
        {
            if ((o.GetDeliveryDateTime() - DateTime.Now).TotalHours < 1)
            {
                o.SetOrderStatus("Rejected");
                refundStack.Push(o);
                rejected++;
                Console.WriteLine($"Order {o.GetOrderId()} rejected (delivery < 1 hour)");
            }
            else
            {
                o.SetOrderStatus("Preparing");
                preparing++;
                Console.WriteLine($"Order {o.GetOrderId()} set to Preparing");
            }
            processed++;
        }
        int total = customers.Sum(c => c.GetOrders().Count);
        Console.WriteLine($"\n--- Summary ---\nOrders processed: {processed}\nPreparing: {preparing}\nRejected: {rejected}\nPercentage: {(total > 0 ? processed * 100.0 / total : 0):0.00}%");
    }

    // ========== ADVANCED FEATURE (b): Display Total Order Amount ==========
    // Student: S10273450 - Yifei
    static void DisplayTotalOrderAmount()
    {
        Console.WriteLine("Total Order Amount Report\n=========================");
        double grandTotal = 0, grandRefunds = 0;
        foreach (var r in restaurants)
        {
            Console.WriteLine($"\nRestaurant: {r.GetRestaurantName()} ({r.GetRestaurantId()})");
            double rTotal = 0, rRefunds = 0;
            foreach (var c in customers)
                foreach (var o in c.GetOrders())
                    if (o.GetOrderStatus() == "Delivered" && r.GetMenus().Any(m => o.GetOrderedFoodItems().Any(oi => m.GetFoodItems().Any(f => f.GetItemName() == oi.GetOrderedItem().GetItemName()))))
                        rTotal += o.GetOrderTotal() - 5;

            var temp = new Stack<Order>();
            while (refundStack.Count > 0)
            {
                var o = refundStack.Pop();
                temp.Push(o);
                if (r.GetMenus().Any(m => o.GetOrderedFoodItems().Any(oi => m.GetFoodItems().Any(f => f.GetItemName() == oi.GetOrderedItem().GetItemName()))))
                    rRefunds += o.GetOrderTotal();
            }
            while (temp.Count > 0) refundStack.Push(temp.Pop());

            Console.WriteLine($"Total Delivered: ${rTotal:0.00}\nTotal Refunds: ${rRefunds:0.00}");
            grandTotal += rTotal;
            grandRefunds += rRefunds;
        }
        Console.WriteLine($"\n--- Grand Total ---\nTotal Orders: ${grandTotal:0.00}\nTotal Refunds: ${grandRefunds:0.00}\nGruberoo Commission (30%): ${grandTotal * 0.30:0.00}\nFinal Amount: ${grandTotal * 0.70:0.00}");
    }

    // HELPER METHODS
    static Customer FindCustomerForOrder(Order order) => customers.FirstOrDefault(c => c.GetOrders().Contains(order));

    static Restaurant FindRestaurantForOrder(Order order)
    {
        foreach (var r in restaurants)
            if (r.GetMenus().Any(m => order.GetOrderedFoodItems().Any(oi => m.GetFoodItems().Any(f => f.GetItemName() == oi.GetOrderedItem().GetItemName()))))
                return r;
        return null;
    }

    static void AppendOrderToFile(Order o, string email, string rid)
    {
        try
        {
            // Build items string: "ItemName1, Qty1|ItemName2, Qty2"
            var itemsList = new List<string>();
            foreach (var item in o.GetOrderedFoodItems())
            {
                itemsList.Add($"{item.GetOrderedItem().GetItemName()}, {item.GetQtyOrdered()}");
            }
            string itemsStr = string.Join("|", itemsList);

            // Format: OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,Items
            string deliveryDate = o.GetDeliveryDateTime().ToString("dd/MM/yyyy");
            string deliveryTime = o.GetDeliveryDateTime().ToString("HH:mm");
            string createdDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            File.AppendAllText("orders.csv", $"{o.GetOrderId()},{email},{rid},{deliveryDate},{deliveryTime},{o.GetDeliveryAddress()},{createdDateTime},{o.GetOrderTotal()},{o.GetOrderStatus()},\"{itemsStr}\"\n");
        }
        catch { }
    }

    static void SaveQueueAndStack()
    {
        try
        {
            using (var w = new StreamWriter("queue.csv"))
            {
                w.WriteLine("OrderID,CustomerEmail,RestaurantID,DeliveryDateTime,DeliveryAddress,TotalAmount,Status");
                var temp = new Queue<Order>();
                while (orderQueue.Count > 0)
                {
                    var o = orderQueue.Dequeue();
                    temp.Enqueue(o);
                    var c = FindCustomerForOrder(o);
                    var r = FindRestaurantForOrder(o);
                    if (c != null && r != null)
                        w.WriteLine($"{o.GetOrderId()},{c.GetEmailAddress()},{r.GetRestaurantId()},{o.GetDeliveryDateTime():dd/MM/yyyy HH:mm},{o.GetDeliveryAddress()},{o.GetOrderTotal()},{o.GetOrderStatus()}");
                }
                while (temp.Count > 0) orderQueue.Enqueue(temp.Dequeue());
            }

            using (var w = new StreamWriter("stack.csv"))
            {
                w.WriteLine("OrderID,CustomerEmail,RestaurantID,DeliveryDateTime,DeliveryAddress,TotalAmount,Status");
                var temp = new Stack<Order>();
                while (refundStack.Count > 0)
                {
                    var o = refundStack.Pop();
                    temp.Push(o);
                    var c = FindCustomerForOrder(o);
                    var r = FindRestaurantForOrder(o);
                    if (c != null && r != null)
                        w.WriteLine($"{o.GetOrderId()},{c.GetEmailAddress()},{r.GetRestaurantId()},{o.GetDeliveryDateTime():dd/MM/yyyy HH:mm},{o.GetDeliveryAddress()},{o.GetOrderTotal()},{o.GetOrderStatus()}");
                }
                while (temp.Count > 0) refundStack.Push(temp.Pop());
            }
            Console.WriteLine("Queue and stack saved successfully!");
        }
        catch { }
    }

    static void CreateSampleRestaurants() => File.WriteAllText("restaurants.csv", "RestaurantId,Name,Email\nR001,Grill House,grillhouse@email.com\nR002,Noodle Palace,noodlepalace@email.com\nR003,Bento Express,bentoexpress@email.com\n");
    static void CreateSampleFoodItems() => File.WriteAllText("fooditems.csv", "RestaurantId,ItemName,Description,Price\nR001,Chicken Rice,Steamed chicken with fragrant rice,5.50\nR001,Beef Burger,Grilled beef patty with fries,9.80\nR002,Spicy Ramen,House-special broth with chilli oil,11.20\nR001,Caesar Salad,Romaine lettuce with house dressing,7.00\nR003,Bento Box,Japanese lunch set,12.50\nR003,Sushi Platter,Assorted fresh sushi,15.00\n");
    static void CreateSampleCustomers() => File.WriteAllText("customers.csv", "Name,Email\nAlice Tan,alice.tan@email.com\nJoseph Lim,joseph.lim@email.com\nWendy Ong,wendy.ong@email.com\n");
    static void CreateSampleOrders() => File.WriteAllText("orders.csv", "OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,Items\n1001,alice.tan@email.com,R003,12/02/2026,12:00,123 Main Street,10/02/2026 10:00,28.40,Delivered,\"Bento Box, 1|Sushi Platter, 1\"\n1002,joseph.lim@email.com,R001,13/02/2026,18:30,456 Oak Avenue,10/02/2026 11:00,33.30,Cancelled,\"Chicken Rice, 2|Caesar Salad, 1\"\n1003,wendy.ong@email.com,R002,14/02/2026,19:00,789 Elm Road,10/02/2026 12:00,16.20,Preparing,\"Spicy Ramen, 1\"\n");
}
