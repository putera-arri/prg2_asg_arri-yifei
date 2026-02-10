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
    internal class SpecialOffer
    {
        private string offerCode;
        private string offerDesc;
        private double discount;

        public SpecialOffer(string code, string desc, double discountPercent)
        {
            offerCode = code;
            offerDesc = desc;
            discount = discountPercent;
        }

        public override string ToString()
        {
            return $"{offerCode} - {offerDesc} ({discount:0.##}%)";
        }
    }
}
