using LivestockManagementSystem.Data;
using LivestockManagementSystem.Models;
using LivestockManagementSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LivestockManagementSystem.ViewModels
{
    public class UpdateViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _db;

        public ObservableCollection<string> Types { get; } = new() { "Cow", "Sheep" };

        private string? _recordId;
        public string? RecordId { get => _recordId; set => SetProperty(ref _recordId, value); }

        private string? _selectedType;
        public string? SelectedType { get => _selectedType; set => SetProperty(ref _selectedType, value); }

        private string? _expense;
        public string? Expense { get => _expense; set => SetProperty(ref _expense, value); }

        private string? _weight;
        public string? Weight { get => _weight; set => SetProperty(ref _weight, value); }

        private string? _colour;
        public string? Colour { get => _colour; set => SetProperty(ref _colour, value); }

        private string? _produceAmount;
        public string? ProduceAmount { get => _produceAmount; set => SetProperty(ref _produceAmount, value); }

        private string? _statusMessage = string.Empty;
        public string? StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        private bool _hasMessage;
        public bool HasMessage { get => _hasMessage; set => SetProperty(ref _hasMessage, value); }

        private Color? _statusColor;
        public Color? StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        public ICommand LoadCommand { get; }
        public ICommand UpdateCommand { get; }

        public UpdateViewModel()
        {
            _db = new ApplicationDbContext();
            LoadCommand = new Command(async () => await LoadRecord());
            UpdateCommand = new Command(async () => await SaveChanges());
        }

        // 🟦 Load existing record by ID
        private async Task LoadRecord()
        {
            HasMessage = false;

            if (!ValidationHelper.IsValidId(RecordId ?? string.Empty, out int id))
            {
                await DialogService.ShowMessageAsync("Invalid ID", "Please enter a valid numeric ID.");
                return;
            }

            try
            {
                Animal? a = await _db.Cow.FirstOrDefaultAsync(c => c.Id == id); if (a == null) a = await _db.Sheep.FirstOrDefaultAsync(s => s.Id == id);

                if (a == null)
                {
                    await DialogService.ShowMessageAsync("Not Found", $"No record found for ID {id}.");
                    return;
                }

                SelectedType = a.TypeName;
                Expense = a.Expense.ToString("F2");
                Weight = a.Weight.ToString("F2");
                Colour = a.Colour;
                ProduceAmount = a.ProduceAmount.ToString("F2");

                ShowMessage($"✅ Record ID {id} loaded successfully.", true);
            }
            catch (Exception ex)
            {
                await DialogService.ShowMessageAsync("Database Error", ex.Message);
            }
        }

        // 🟩 Save changes after editing
        private async Task SaveChanges()
        {
            HasMessage = false;

            if (!ValidationHelper.IsValidId(RecordId ?? string.Empty, out int id))
            {
                await DialogService.ShowMessageAsync("Invalid ID", "Please enter a valid numeric ID.");
                return;
            }

            try
            {
                Animal? a = await _db.Cow.FirstOrDefaultAsync(c => c.Id == id); if (a == null) a = await _db.Sheep.FirstOrDefaultAsync(s => s.Id == id);

                if (a == null)
                {
                    await DialogService.ShowMessageAsync("Not Found", $"Record ID {id} not found.");
                    return;
                }

                // Validate all user input
                if (!ValidationHelper.IsPositive(Expense ?? string.Empty, out double exp))
                {
                    await DialogService.ShowMessageAsync("Invalid Expense", "Expense must be a positive number.");
                    return;
                }

                if (!ValidationHelper.IsPositive(Weight ?? string.Empty, out double wgt))
                {
                    await DialogService.ShowMessageAsync("Invalid Weight", "Weight must be a positive number.");
                    return;
                }

                if (!ValidationHelper.IsValidColour(Colour ?? string.Empty))
                {
                    await DialogService.ShowMessageAsync("Invalid Colour", "Allowed colours: Red, Black, White.");
                    return;
                }

                if (!ValidationHelper.IsValidType(SelectedType ?? string.Empty))
                {
                    await DialogService.ShowMessageAsync("Invalid Type", "Type must be Cow or Sheep.");
                    return;
                }

                // Update base fields
                a.Expense = exp;
                a.Weight = wgt;
                a.Colour = Colour!.Trim();

                // Update derived fields
                if (a is Cow cow)
                    cow.Milk = double.TryParse(ProduceAmount, out double milkVal) ? milkVal : 0;
                else if (a is Sheep sheep)
                    sheep.Wool = double.TryParse(ProduceAmount, out double woolVal) ? woolVal : 0;

                bool confirm = await DialogService.ShowConfirmAsync("Confirm Update", $"Save changes to record ID {id}?");
                if (!confirm) return;

                _db.Update(a);
                await _db.SaveChangesAsync();

                ShowMessage($"💾 Record ID {id} updated successfully.", true);
            }
            catch (Exception ex)
            {
                await DialogService.ShowMessageAsync("Database Error", ex.Message);
                ShowMessage("❌ Error saving changes.", false);
            }
        }

        // 🔹 Local status label (on-page)
        private void ShowMessage(string message, bool isSuccess)
        {
            StatusMessage = message;
            HasMessage = true;
            StatusColor = isSuccess ? Color.FromArgb("#4CAF50") : Color.FromArgb("#E53935");
        }
    }
}
