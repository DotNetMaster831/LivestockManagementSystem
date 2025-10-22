using LivestockManagementSystem.Data;
using LivestockManagementSystem.Models;
using LivestockManagementSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestockManagementSystem.Services
{
    public class FarmStatisticsService
    {
        public async Task<(double monthlyTax, double dailyProfit, double avgWeight,
                           double avgCowProfit, double avgSheepProfit, string moreProfitable)>
            CalculateFarmStatisticsAsync()
        {
            try
            {
                using var db = new ApplicationDbContext();
                var cows = await db.Cow.ToListAsync();
                var sheep = await db.Sheep.ToListAsync();
                var all = cows.Cast<Animal>().Concat(sheep).ToList();

                double monthlyTax = FarmCalculator.CalculateMonthlyTax(all);

                //double monthlyTaxOfAnimals = all.Sum(a => a.GovernmentTax);


                double dailyProfit = FarmCalculator.CalculateDailyProfit(all);
                double avgWeight = FarmCalculator.CalculateAverageWeight(all);

                double avgCowProfit = cows.Any() ? cows.Average(c => c.Profit) : 0;
                double avgSheepProfit = sheep.Any() ? sheep.Average(s => s.Profit) : 0;

                string moreProfitable = avgCowProfit > avgSheepProfit
                    ? "Cows are more profitable than Sheep"
                    : avgCowProfit < avgSheepProfit
                        ? "Sheep are more profitable than Cows"
                        : "Both have equal profitability";

                return (monthlyTax, dailyProfit, avgWeight, avgCowProfit, avgSheepProfit, moreProfitable);
            }
            catch(Exception ex)
            {
                return (0, 0, 0, 0, 0, "");
            }
           
        }
    }
}
