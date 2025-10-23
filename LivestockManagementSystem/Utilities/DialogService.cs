using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestockManagementSystem.Utilities
{
    public static class DialogService
    {
        private static Page CurrentPage =>
            Application.Current?.Windows.FirstOrDefault()?.Page
            ?? throw new InvalidOperationException("No active window");

        public static Task ShowMessageAsync(string title, string message, string cancel = "OK") =>
            CurrentPage.DisplayAlert(title, message, cancel);

        public static Task<bool> ShowConfirmAsync(string title, string message,
                                                  string accept = "Yes", string cancel = "No") =>
            CurrentPage.DisplayAlert(title, message, accept, cancel);
    }
}
