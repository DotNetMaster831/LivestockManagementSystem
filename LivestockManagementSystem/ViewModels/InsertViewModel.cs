using LivestockManagementSystem.Data;
using LivestockManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Graphics; // Needed for Color

namespace LivestockManagementSystem.ViewModels
{
    public class InsertViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _db;

        public ObservableCollection<string> Types { get; } = new() { "Cow", "Sheep" };

        private string _selectedType;
        public string SelectedType { get => _selectedType; set => SetProperty(ref _selectedType, value); }

        private string _expense;
        public string Expense { get => _expense; set => SetProperty(ref _expense, value); }

        private string _weight;
        public string Weight { get => _weight; set => SetProperty(ref _weight, value); }

        private string _colour;
        public string Colour { get => _colour; set => SetProperty(ref _colour, value); }

        private string _produceAmount;
        public string ProduceAmount { get => _produceAmount; set => SetProperty(ref _produceAmount, value); }

        private string _statusMessage;
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        private bool _hasMessage;
        public bool HasMessage { get => _hasMessage; set => SetProperty(ref _hasMessage, value); }

        private Color _statusColor;
        public Color StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        public ICommand InsertCommand { get; }

        public InsertViewModel()
        {
            _db = new ApplicationDbContext();
            InsertCommand = new Command(async () => await InsertRecord());
        }

        private async Task InsertRecord()
        {
            HasMessage = false;

            // Basic validation
            if (string.IsNullOrWhiteSpace(SelectedType))
            {
                ShowMessage("⚠️ Please select a livestock type.", false);
                return;
            }

            if (!double.TryParse(Expense, out double expense) || expense <= 0)
            {
                ShowMessage("⚠️ Please enter a valid numeric expense.", false);
                return;
            }

            if (!double.TryParse(Weight, out double weight) || weight <= 0)
            {
                ShowMessage("⚠️ Please enter a valid weight value.", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(Colour))
            {
                ShowMessage("⚠️ Colour field cannot be empty.", false);
                return;
            }
            if (string.IsNullOrWhiteSpace(ProduceAmount))
            {
                ShowMessage("⚠️ Produce Amount field cannot be empty.", false);
                return;
            }
            try
            {
                Animal animal = SelectedType == "Cow" ? new Cow() : new Sheep();
                animal.Expense = expense;
                animal.Weight = weight;
                animal.Colour = Colour.Trim();

                if (animal is Cow cow)
                    cow.Milk = double.TryParse(ProduceAmount, out double milkVal) ? milkVal : 0;
                else if (animal is Sheep sheep)
                    sheep.Wool = double.TryParse(ProduceAmount, out double woolVal) ? woolVal : 0;

                _db.Add(animal);
                await _db.SaveChangesAsync();

                ShowMessage($"✅ New {SelectedType} added successfully!", true);
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Error adding record: {ex.Message}", false);
            }
        }

        private void ClearForm()
        {
            SelectedType = null;
            Expense = Weight = Colour = ProduceAmount = string.Empty;
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            StatusMessage = message;
            HasMessage = true;
            StatusColor = isSuccess ? Color.FromArgb("#4CAF50") : Color.FromArgb("#E53935");
        }
    }
}
