using LivestockManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LivestockManagementSystem.ViewModels
{
    public class ForecastViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _db;

        public List<string> LivestockTypes { get; } = new() { "Cow", "Sheep" };

        private string? _selectedType;
        public string? SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

        private string? _quantity;
        public string? Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        private bool? _hasResult;
        public bool? HasResult
        {
            get => _hasResult;
            set => SetProperty(ref _hasResult, value);
        }

        private string? _forecastResult;
        public string? ForecastResult
        {
            get => _forecastResult;
            set => SetProperty(ref _forecastResult, value);
        }

        public ICommand CalculateCommand { get; }

        public ForecastViewModel()
        {
            _db = new ApplicationDbContext();
            CalculateCommand = new Command(async () => await CalculateForecast());
        }

        private async Task CalculateForecast()
        {
            HasResult = false;

            if (string.IsNullOrWhiteSpace(SelectedType))
            {
                ForecastResult = "⚠️ Please select a livestock type.";
                HasResult = true;
                return;
            }

            if (!int.TryParse(Quantity, out int qty) || qty <= 0)
            {
                ForecastResult = "⚠️ Please enter a valid positive quantity.";
                HasResult = true;
                return;
            }

            var cows = await _db.Cow.ToListAsync();
            var sheep = await _db.Sheep.ToListAsync();

            double cowMilkPrice = 9.4;
            double sheepWoolPrice = 6.2;
            double taxRate = 0.02;

            double cowProfit = 0;
            foreach (var c in cows)
            {
                var tax = c.Weight * taxRate;
                var income = c.ProduceAmount * cowMilkPrice;
                cowProfit += income - (c.Expense + tax);
            }
            cowProfit /= cows.Count;

            double sheepProfit = 0;
            foreach (var s in sheep)
            {
                var tax = s.Weight * taxRate;
                var income = s.ProduceAmount * sheepWoolPrice;
                sheepProfit += income - (s.Expense + tax);
            }
            sheepProfit /= sheep.Count;

            double avgProfit = SelectedType == "Cow" ? cowProfit : sheepProfit;
            double forecast = avgProfit * qty;

            ForecastResult = $"🐄🐑 For {qty} {SelectedType}(s), the estimated daily profit is ${forecast:F2}.\n\n" +
                             $"(Based on average profit per {SelectedType}: ${avgProfit:F2})";

            HasResult = true;
        }
    }
}
