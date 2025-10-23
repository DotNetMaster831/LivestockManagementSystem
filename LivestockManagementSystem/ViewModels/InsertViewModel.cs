using LivestockManagementSystem.Data;
using LivestockManagementSystem.Models;
using LivestockManagementSystem.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LivestockManagementSystem.ViewModels
{
    public class InsertViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _db;

        // 🟢 Dropdown data
        public ObservableCollection<string> Types { get; } = new() { "Cow", "Sheep" };

        // 🧩 Fields initialized to avoid nullable warnings
        private string _selectedType = string.Empty;
        public string SelectedType { get => _selectedType; set => SetProperty(ref _selectedType, value); }

        private string _expense = string.Empty;
        public string Expense { get => _expense; set => SetProperty(ref _expense, value); }

        private string _weight = string.Empty;
        public string Weight { get => _weight; set => SetProperty(ref _weight, value); }

        private string _colour = string.Empty;
        public string Colour { get => _colour; set => SetProperty(ref _colour, value); }

        private string _produceAmount = string.Empty;
        public string ProduceAmount { get => _produceAmount; set => SetProperty(ref _produceAmount, value); }

        private string _statusMessage = string.Empty;
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        private bool _hasMessage;
        public bool HasMessage { get => _hasMessage; set => SetProperty(ref _hasMessage, value); }

        private Color? _statusColor;
        public Color? StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        // Command binding
        public ICommand InsertCommand { get; }

        public InsertViewModel()
        {
            _db = new ApplicationDbContext();
            InsertCommand = new Command(async () => await InsertRecord());
        }

        // 🟨 Main Insert Logic with Validation + Error Handling
        private async Task InsertRecord()
        {
            HasMessage = false;

            try
            {
                // ✅ Type validation
                if (!ValidationHelper.IsValidType(SelectedType))
                {
                    await DialogService.ShowMessageAsync("Invalid Type", "Please select Cow or Sheep.");
                    return;
                }

                // ✅ Expense validation
                if (!ValidationHelper.IsPositive(Expense, out double expense))
                {
                    await DialogService.ShowMessageAsync("Invalid Expense", "Expense must be a positive numeric value.");
                    return;
                }

                // ✅ Weight validation
                if (!ValidationHelper.IsPositive(Weight, out double weight))
                {
                    await DialogService.ShowMessageAsync("Invalid Weight", "Weight must be a positive number.");
                    return;
                }

                // ✅ Colour validation
                if (!ValidationHelper.IsValidColour(Colour))
                {
                    await DialogService.ShowMessageAsync("Invalid Colour", "Allowed colours: Red, Black, White.");
                    return;
                }

                // ✅ Produce validation
                if (string.IsNullOrWhiteSpace(ProduceAmount))
                {
                    await DialogService.ShowMessageAsync("Invalid Produce Amount", "Please enter milk or wool quantity.");
                    return;
                }

                // 🟢 Build animal record
                Animal animal = SelectedType == "Cow" ? new Cow() : new Sheep();
                animal.Expense = expense;
                animal.Weight = weight;
                animal.Colour = Colour.Trim();

                if (animal is Cow cow)
                    cow.Milk = double.TryParse(ProduceAmount, out double milkVal) ? milkVal : 0;
                else if (animal is Sheep sheep)
                    sheep.Wool = double.TryParse(ProduceAmount, out double woolVal) ? woolVal : 0;

                // 🟩 Save record
                _db.Add(animal);
                await _db.SaveChangesAsync();

                ShowMessage($"✅ New {SelectedType} added successfully!", true);
                await DialogService.ShowMessageAsync("Success", $"{SelectedType} record added successfully!");
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Error adding record: {ex.Message}", false);
                await DialogService.ShowMessageAsync("Database Error", ex.Message);
            }
        }

        private void ClearForm()
        {
            SelectedType = string.Empty;
            Expense = string.Empty;
            Weight = string.Empty;
            Colour = string.Empty;
            ProduceAmount = string.Empty;
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            StatusMessage = message;
            HasMessage = true;
            StatusColor = isSuccess ? Color.FromArgb("#4CAF50") : Color.FromArgb("#E53935");
        }
    }
}
