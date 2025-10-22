using LivestockManagementSystem.Services;
using LivestockManagementSystem.Views;

namespace LivestockManagementSystem
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HomePage), typeof(Views.HomePage));
            Routing.RegisterRoute(nameof(LivestockListPage), typeof(Views.LivestockListPage));
            Routing.RegisterRoute(nameof(QueryPage), typeof(Views.QueryPage));
            Routing.RegisterRoute(nameof(StatisticsPage), typeof(Views.StatisticsPage));
            Routing.RegisterRoute(nameof(ForecastPage), typeof(Views.ForecastPage));
            Routing.RegisterRoute(nameof(InsertPage), typeof(Views.InsertPage));
            Routing.RegisterRoute(nameof(UpdatePage), typeof(Views.UpdatePage));
            Routing.RegisterRoute(nameof(DeletePage), typeof(Views.DeletePage));
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}