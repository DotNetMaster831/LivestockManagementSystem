using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using LivestockManagementSystem.Models;
using LivestockManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LivestockManagementSystem.ViewModels
{
    public class QueryPageViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _db;

        public ObservableCollection<string> LivestockTypes { get; } = new() { "All", "Cow", "Sheep" };
        public ObservableCollection<string> Colours { get; } = new() { "All", "Red", "Black", "White" };

        private string _selectedType = "All";
        public string SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

        private string _selectedColour = "All";
        public string SelectedColour
        {
            get => _selectedColour;
            set => SetProperty(ref _selectedColour, value);
        }

        private bool _hasResults;
        public bool HasResults
        {
            get => _hasResults;
            set => SetProperty(ref _hasResults, value);
        }

        private string _resultSummary;
        public string ResultSummary
        {
            get => _resultSummary;
            set => SetProperty(ref _resultSummary, value);
        }

        public ICommand RunQueryCommand { get; }

        public QueryPageViewModel()
        {
            _db = new ApplicationDbContext();
            RunQueryCommand = new Command(async () => await RunQuery());
        }

        private async Task RunQuery()
        {
            var cows = await _db.Cow.ToListAsync();
            var sheep = await _db.Sheep.ToListAsync();

            var all = new List<Animal>();
            all.AddRange(cows);
            all.AddRange(sheep);

            // Filter
            IEnumerable<Animal> filtered = all;

            if (SelectedType != "All")
                filtered = filtered.Where(a => a.TypeName.Equals(SelectedType, StringComparison.OrdinalIgnoreCase));

            if (SelectedColour != "All")
                filtered = filtered.Where(a => a.Colour.Equals(SelectedColour, StringComparison.OrdinalIgnoreCase));

            var list = filtered.ToList();
            if (!list.Any())
            {
                HasResults = true;
                ResultSummary = $"No records found for {SelectedType} in {SelectedColour} colour.";
                return;
            }

            // Calculations
            double cowMilkPrice = 9.4;
            double sheepWoolPrice = 6.2;
            double taxRate = 0.02;

            double income = 0, cost = 0, tax = 0, profit = 0, avgWeight = 0, produce = 0;

            foreach (var item in list)
            {
                double govTax = item.Weight * taxRate;
                tax += govTax;
                cost += item.Expense + govTax;

                if (item.TypeName == "Cow")
                    income += (item.ProduceAmount * cowMilkPrice);
                else
                    income += (item.ProduceAmount * sheepWoolPrice);

                produce += item.ProduceAmount;
                avgWeight += item.Weight;
            }

            avgWeight /= list.Count;
            profit = income - cost;

            double totalCount = list.Count;
            double percentage = (totalCount / all.Count) * 100;

            // Summary Formatting
            var sb = new StringBuilder();
            sb.AppendLine($"Total selected livestock: {totalCount}");
            sb.AppendLine($"Percentage of total: {percentage:F1}%");
            sb.AppendLine($"Average Weight: {avgWeight:F1} kg");
            sb.AppendLine($"Total Produce (Milk/Wool): {produce:F1} kg");
            sb.AppendLine($"Total Tax Paid: ${tax:F2} per day");
            sb.AppendLine($"Profit/Loss: ${profit:F2} per day");

            HasResults = true;
            ResultSummary = sb.ToString();
        }
    }
}
