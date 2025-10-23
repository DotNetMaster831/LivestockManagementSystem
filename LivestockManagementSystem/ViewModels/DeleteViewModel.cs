using LivestockManagementSystem.Data;
using LivestockManagementSystem.Models;
using LivestockManagementSystem.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;
using System.Windows.Input;

namespace LivestockManagementSystem.ViewModels
{
    public class DeleteViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _db;

        private string _recordId = string.Empty;
        public string RecordId
        {
            get => _recordId;
            set => SetProperty(ref _recordId, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _hasMessage;
        public bool HasMessage
        {
            get => _hasMessage;
            set => SetProperty(ref _hasMessage, value);
        }

        private Color? _statusColor;
        public Color? StatusColor
        {
            get => _statusColor;
            set => SetProperty(ref _statusColor, value);
        }

        public ICommand DeleteCommand { get; }

        public DeleteViewModel()
        {
            _db = new ApplicationDbContext();
            DeleteCommand = new Command(async () => await DeleteRecord());
        }

        private async Task DeleteRecord()
        {
            HasMessage = false;

            // ✅ Validate ID
            if (!ValidationHelper.IsValidId(RecordId, out int id))
            {
                await DialogService.ShowMessageAsync("Invalid ID", "Please enter a valid numeric ID greater than 0.");
                return;
            }

            try
            {
                // ✅ Ask for confirmation (modern dialog)
                bool confirm = await DialogService.ShowConfirmAsync(
                    "Confirm Deletion",
                    $"Are you sure you want to delete record ID {id}?",
                    "Yes", "No");

                if (!confirm)
                {
                    ShowMessage("❎ Deletion cancelled.", false);
                    return;
                }

                // ✅ Attempt to find record
                Animal? existing = await _db.Cow.FirstOrDefaultAsync(c => c.Id == id); if (existing == null) existing = await _db.Sheep.FirstOrDefaultAsync(s => s.Id == id);

                if (existing == null)
                {
                    await DialogService.ShowMessageAsync("Not Found", $"Record ID {id} does not exist.");
                    ShowMessage($"❌ Record ID {id} not found.", false);
                    return;
                }

                // ✅ Delete from DB
                _db.Remove(existing);
                await _db.SaveChangesAsync();

                ShowMessage($"✅ Record ID {id} deleted successfully.", true);
                await DialogService.ShowMessageAsync("Success", $"Record ID {id} deleted successfully!");
                RecordId = string.Empty;
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Error while deleting: {ex.Message}", false);
                await DialogService.ShowMessageAsync("Database Error", ex.Message);
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
