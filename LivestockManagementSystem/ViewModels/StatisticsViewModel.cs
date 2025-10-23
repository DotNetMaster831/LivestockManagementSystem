using LivestockManagementSystem.Data;
using LivestockManagementSystem.Models;
using LivestockManagementSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LivestockManagementSystem.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _db;

        public ICommand RefreshStatsCommand { get; }

        private string _dailyProfitText = string.Empty;
        public string DailyProfitText { get => _dailyProfitText; set => SetProperty(ref _dailyProfitText, value); }

        private string _tax30DayText = string.Empty;
        public string Tax30DayText { get => _tax30DayText; set => SetProperty(ref _tax30DayText, value); }

        private string _avgWeightText = string.Empty;
        public string AvgWeightText { get => _avgWeightText; set => SetProperty(ref _avgWeightText, value); }

        private string _avgProfitText = string.Empty;
        public string AvgProfitText { get => _avgProfitText; set => SetProperty(ref _avgProfitText, value); }

        private string _cowProfitText = string.Empty;
        public string CowProfitText { get => _cowProfitText; set => SetProperty(ref _cowProfitText, value); }

        private string _sheepProfitText = string.Empty;
        public string SheepProfitText { get => _sheepProfitText; set => SetProperty(ref _sheepProfitText, value); }

        private string _moreProfitableText = string.Empty;
        public string MoreProfitableText { get => _moreProfitableText; set => SetProperty(ref _moreProfitableText, value); }

        private string _totalAnimalsText = string.Empty;
        public string TotalAnimalsText { get => _totalAnimalsText; set => SetProperty(ref _totalAnimalsText, value); }

        public StatisticsViewModel()
        {
            _db = new ApplicationDbContext();
            RefreshStatsCommand = new Command(async () => await LoadStatistics());
            _ = LoadStatistics(); // auto-load once
        }

        private async Task LoadStatistics()
        {
            try
            {
                var cows = await _db.Cow.ToListAsync();
                var sheep = await _db.Sheep.ToListAsync();

                var all = new List<Animal>();
                all.AddRange(cows);
                all.AddRange(sheep);

                if (!all.Any())
                {
                    await DialogService.ShowMessageAsync("No Data", "No livestock records found in the database.");
                    ResetTexts();
                    return;
                }

                double cowMilkPrice = 9.4;
                double sheepWoolPrice = 6.2;
                double taxRate = 0.02;

                double income = 0, cost = 0, tax = 0, cowProfit = 0, sheepProfit = 0, totalWeight = 0;

                foreach (var item in all)
                {
                    double govTax = item.Weight * taxRate;
                    tax += govTax;
                    cost += item.Expense + govTax;
                    totalWeight += item.Weight;

                    if (item.TypeName == "Cow")
                    {
                        var inc = item.ProduceAmount * cowMilkPrice;
                        income += inc;
                        cowProfit += inc - (item.Expense + govTax);
                    }
                    else
                    {
                        var inc = item.ProduceAmount * sheepWoolPrice;
                        income += inc;
                        sheepProfit += inc - (item.Expense + govTax);
                    }
                }

                double avgWeight = totalWeight / all.Count;
                double dailyProfit = income - cost;
                double avgProfit = dailyProfit / all.Count;
                double tax30day = tax * 30;

                DailyProfitText = $"Farm Daily Profit: ${dailyProfit:F2}";
                Tax30DayText = $"Estimated 30-Day Government Tax: ${tax30day:F2}";
                AvgProfitText = $"Average Daily Profit per Animal: ${avgProfit:F2}";
                CowProfitText = $"Average Profit per Cow: ${cowProfit / (cows.Count == 0 ? 1 : cows.Count):F2}";
                SheepProfitText = $"Average Profit per Sheep: ${sheepProfit / (sheep.Count == 0 ? 1 : sheep.Count):F2}";
                AvgWeightText = $"Average Weight (All Livestock): {avgWeight:F1} kg";
                TotalAnimalsText = $"Total Livestock Count: {all.Count}";

                MoreProfitableText = cowProfit > sheepProfit
                    ? "🐄 Cows are more profitable overall."
                    : "🐑 Sheep are more profitable overall.";
            }
            catch (Exception ex)
            {
                await DialogService.ShowMessageAsync("Database Error", ex.Message);
                ResetTexts();
            }
        }

        private void ResetTexts()
        {
            DailyProfitText = "—";
            Tax30DayText = "—";
            AvgProfitText = "—";
            CowProfitText = "—";
            SheepProfitText = "—";
            AvgWeightText = "—";
            TotalAnimalsText = "—";
            MoreProfitableText = "—";
        }
    }
}
