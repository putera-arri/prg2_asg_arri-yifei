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
    internal class Order
    {
        private int orderId;
        private DateTime orderDateTime;
        private double orderTotal;
        private string orderStatus;
        private DateTime deliveryDateTime;
        private string deliveryAddress;
        private string orderPaymentMethod;
        private bool orderPaid;

        private List<OrderedFoodItem> orderedFoodItems = new List<OrderedFoodItem>();

        public Order(int id, DateTime deliveryDT, string address)
        {
            orderId = id;
            orderDateTime = DateTime.Now;
            deliveryDateTime = deliveryDT;
            deliveryAddress = address;
            orderStatus = "Pending";
            orderPaid = false;
            orderPaymentMethod = "";
            orderTotal = 0;
        }

        public int GetOrderId() => orderId;
        public string GetOrderStatus() => orderStatus;
        public DateTime GetDeliveryDateTime() => deliveryDateTime;
        public string GetDeliveryAddress() => deliveryAddress;
        public double GetOrderTotal() => orderTotal;

        public void SetOrderStatus(string status) => orderStatus = status;
        public void SetPayment(string method, bool paid)
        {
            orderPaymentMethod = method;
            orderPaid = paid;
        }

        public double CalculateOrderTotal()
        {
            double items = orderedFoodItems.Sum(i => i.CalculateSubtotal());
            orderTotal = items + 5.00;
            return orderTotal;
        }

        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            if (item == null) return;
            orderedFoodItems.Add(item);
            CalculateOrderTotal();
        }

        public bool RemoveOrderedFoodItem(OrderedFoodItem item)
        {
            if (item == null) return false;
            var ok = orderedFoodItems.Remove(item);
            CalculateOrderTotal();
            return ok;
        }

        public void DisplayOrderedFoodItems()
        {
            foreach (var i in orderedFoodItems)
                System.Console.WriteLine(i.ToString());
        }

        public override string ToString()
        {
            return $"Order {orderId} | {deliveryDateTime:dd/MM/yyyy HH:mm} | ${orderTotal:0.00} | {orderStatus}";
        }

        public List<OrderedFoodItem> GetOrderedFoodItems()
        {
            return orderedFoodItems;
        }
    }
}