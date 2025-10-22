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
    public class LivestockQueryService
    {
        public async Task<IEnumerable<Animal>> GetLivestockAsync(string type, string colour)
        {
            using var db = new ApplicationDbContext();

            var cows = await db.Cow.ToListAsync();
            var sheep = await db.Sheep.ToListAsync();
            var all = cows.Cast<Animal>().Concat(sheep);

            IEnumerable<Animal> filtered = type.ToLower() switch
            {
                "cow" => cows,
                "sheep" => sheep,
                "all" => all,
                _ => Enumerable.Empty<Animal>()
            };

            if (colour.ToLower() != "all")
                filtered = filtered.Where(a => a.Colour.Equals(colour, StringComparison.OrdinalIgnoreCase));

            return filtered;
        }

        public async Task<(int count, double percentage, double profit, double avgWeight,
                           double dailyTax, double produce)>
            CalculateQuerySummaryAsync(string type, string colour)
        {
            var selected = await GetLivestockAsync(type, colour);
            var all = await GetLivestockAsync("all", "all");

            int count = selected.Count();
            int total = all.Count();
            double percentage = total > 0 ? (count * 100.0 / total) : 0;
            double profit = FarmCalculator.CalculateDailyProfit(selected);
            double avgWeight = FarmCalculator.CalculateAverageWeight(selected);
            double dailyTax = FarmCalculator.CalculateDailyTax(selected);
            double produce = FarmCalculator.CalculateProduceAmount(selected);

            return (count, percentage, profit, avgWeight, dailyTax, produce);
        }
    }
}
