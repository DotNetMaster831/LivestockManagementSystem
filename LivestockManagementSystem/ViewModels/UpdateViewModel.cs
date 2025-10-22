using LivestockManagementSystem.Data;
using LivestockManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LivestockManagementSystem.ViewModels
{
    public class UpdateViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _db;

        public ObservableCollection<string> Types { get; } = new() { "Cow", "Sheep" };

        private string _recordId;
        public string RecordId { get => _recordId; set => SetProperty(ref _recordId, value); }

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

        public ICommand LoadCommand { get; }
        public ICommand UpdateCommand { get; }

        public UpdateViewModel()
        {
            _db = new ApplicationDbContext();
            LoadCommand = new Command(async () => await LoadRecord());
            UpdateCommand = new Command(async () => await SaveChanges());
        }

        private async Task LoadRecord()
        {
            HasMessage = false;

            if (!int.TryParse(RecordId, out int id))
            {
                ShowMessage("⚠️ Please enter a valid numeric ID.", false);
                return;
            }

            Animal a = await _db.Cow.FirstOrDefaultAsync(c => c.Id == id);
            if (a == null)
                a = await _db.Sheep.FirstOrDefaultAsync(s => s.Id == id);

            if (a == null)
            {
                ShowMessage($"❌ No record found for ID {id}.", false);
                return;
            }

            SelectedType = a.TypeName;
            Expense = a.Expense.ToString();
            Weight = a.Weight.ToString();
            Colour = a.Colour;
            ProduceAmount = a.ProduceAmount.ToString();

            ShowMessage($"✅ Record ID {id} loaded successfully.", true);
        }

        private async Task SaveChanges()
        {
            HasMessage = false;

            if (!int.TryParse(RecordId, out int id))
            {
                ShowMessage("⚠️ Invalid ID.", false);
                return;
            }

            Animal a = await _db.Cow.FirstOrDefaultAsync(c => c.Id == id);
            if (a == null)
                a = await _db.Sheep.FirstOrDefaultAsync(s => s.Id == id);

            if (a == null)
            {
                ShowMessage($"❌ Record ID {id} not found.", false);
                return;
            }

            if (!double.TryParse(Expense, out double exp) || exp <= 0 ||
                !double.TryParse(Weight, out double wgt) || wgt <= 0 ||
                string.IsNullOrWhiteSpace(Colour))
            {
                ShowMessage("⚠️ Please provide valid numeric and text values.", false);
                return;
            }

            a.Expense = exp;
            a.Weight = wgt;
            a.Colour = Colour.Trim();

            try
            {
                _db.Update(a);
                await _db.SaveChangesAsync();
                ShowMessage($"💾 Record ID {id} updated successfully.", true);
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Error saving changes: {ex.Message}", false);
            }
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            StatusMessage = message;
            HasMessage = true;
            StatusColor = isSuccess ? Color.FromArgb("#4CAF50") : Color.FromArgb("#E53935");
        }
    }
}
