//==========================================================
// Student Number : S10273450
// Student Name   : Yifei
// Partner Name   : arri
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class Customer
    {
        private string emailAddress;
        private string customerName;

        private List<Order> orders = new List<Order>();

        public Customer(string name, string email)
        {
            customerName = name;
            emailAddress = email;
        }

        public string GetEmailAddress() => emailAddress;
        public string GetCustomerName() => customerName;

        public void AddOrder(Order order)
        {
            if (order == null) return;
            orders.Add(order);
        }

        public void DisplayAllOrders()
        {
            foreach (var o in orders)
                System.Console.WriteLine(o.ToString());
        }

        public bool RemoveOrder(Order order)
        {
            if (order == null) return false;
            return orders.Remove(order);
        }

        public override string ToString()
        {
            return $"{customerName} - {emailAddress}";
        }

        public List<Order> GetOrders()
        {
            return orders;
        }
    }
}
