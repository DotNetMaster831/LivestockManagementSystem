using LivestockManagementSystem.Data;
using LivestockManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LivestockManagementSystem.ViewModels
{
    public class DeleteViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _db;

        private string _recordId;
        public string RecordId { get => _recordId; set => SetProperty(ref _recordId, value); }

        private string _statusMessage;
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        private bool _hasMessage;
        public bool HasMessage { get => _hasMessage; set => SetProperty(ref _hasMessage, value); }

        private Color _statusColor;
        public Color StatusColor { get => _statusColor; set => SetProperty(ref _statusColor, value); }

        public ICommand DeleteCommand { get; }

        public DeleteViewModel()
        {
            _db = new ApplicationDbContext();
            DeleteCommand = new Command(async () => await DeleteRecord());
        }

        private async Task DeleteRecord()
        {
            HasMessage = false;

            if (!int.TryParse(RecordId, out int id))
            {
                ShowMessage("⚠️ Please enter a valid numeric ID.", false);
                return;
            }

            bool confirm = await App.Current.MainPage.DisplayAlert(
                "Confirm Deletion",
                $"Are you sure you want to delete record ID {id}?",
                "Yes", "No");

            if (!confirm)
            {
                ShowMessage("❎ Deletion cancelled.", false);
                return;
            }

            Animal existing = await _db.Cow.FirstOrDefaultAsync(c => c.Id == id);
            if (existing == null)
                existing = await _db.Sheep.FirstOrDefaultAsync(s => s.Id == id);

            if (existing == null)
            {
                ShowMessage($"❌ Record ID {id} not found.", false);
                return;
            }

            try
            {
                _db.Remove(existing);
                await _db.SaveChangesAsync();

                ShowMessage($"✅ Record ID {id} deleted successfully.", true);
                RecordId = string.Empty;
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Error while deleting: {ex.Message}", false);
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
