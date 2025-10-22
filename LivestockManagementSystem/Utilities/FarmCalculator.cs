using LivestockManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestockManagementSystem.Utilities
{
    public class FarmCalculator
    {
        // --- DAILY CALCULATIONS ---

        public static double CalculateDailyIncome(IEnumerable<Animal> animals)
            => animals.Sum(a => a.Income);

        public static double CalculateDailyCost(IEnumerable<Animal> animals)
            => animals.Sum(a => a.Cost);

        public static double CalculateDailyProfit(IEnumerable<Animal> animals)
            => animals.Sum(a => a.Profit);

        public static double CalculateDailyTax(IEnumerable<Animal> animals)
            => animals.Sum(a => a.GovernmentTax);

        public static double CalculateAverageWeight(IEnumerable<Animal> animals)
            => animals.Any() ? animals.Average(a => a.Weight) : 0;

        public static double CalculateProduceAmount(IEnumerable<Animal> animals)
        {
            if (!animals.Any()) return 0;

            var first = animals.First();
            if (first is Cow)
                return animals.OfType<Cow>().Sum(c => c.Milk);
            else
                return animals.OfType<Sheep>().Sum(s => s.Wool);
        }

        // --- MONTHLY FARM STATS ---

        public static double CalculateMonthlyTax(IEnumerable<Animal> animals)
            => CalculateDailyTax(animals) * 30;

        // --- PROFIT FORECAST ---

        public static double CalculateInvestmentProfit(double avgProfitPerAnimal, int quantity)
            => avgProfitPerAnimal * quantity;
    }
}
