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
    public class LivestockListViewModel : BindableObject
    {
        private ObservableCollection<Animal> _livestockList = new();
        public ObservableCollection<Animal> LivestockList
        {
            get => _livestockList;
            set
            {
                _livestockList = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public LivestockListViewModel()
        {
            RefreshCommand = new Command(async () => await LoadLivestockAsync());
            _ = LoadLivestockAsync();
        }

        public async Task LoadLivestockAsync()
        {
            try
            {
                using var db = new ApplicationDbContext();
                var cows = await db.Cow.ToListAsync();
                var sheep = await db.Sheep.ToListAsync();

                var combined = cows.Cast<Animal>()
                                   .Concat(sheep)
                                   .OrderBy(a => a.Id)
                                   .ToList();

                LivestockList = new ObservableCollection<Animal>(combined);
            }
            catch (Exception ex)
            {
                await App.Current!.Windows[0].Page!.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
