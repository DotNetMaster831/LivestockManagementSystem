namespace LivestockManagementSystem.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnListClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(LivestockListPage));

    private async void OnQueryClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(QueryPage));

    private async void OnStatsClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(StatisticsPage));

    private async void OnForecastClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(ForecastPage));

    private async void OnInsertClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(InsertPage));

    private async void OnUpdateClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(UpdatePage));

    private async void OnDeleteClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(DeletePage));
}