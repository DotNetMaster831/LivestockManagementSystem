using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestockManagementSystem.Models
{
    public abstract class Animal
    {
        public int Id { get; set; }
        public double Expense { get; set; }
        public double Weight { get; set; }
        public string Colour { get; set; } = string.Empty;

        public abstract double Income { get; }

        public double GovernmentTax => Weight * 0.02;
        public double Cost => Expense + GovernmentTax;
        public double Profit => Income - Cost;

        public bool IsProfit => Profit > 0;
        public bool IsLoss => Profit < 0;
        public virtual string TypeName => GetType().Name;
        public virtual double ProduceAmount => 0;

    }
}
