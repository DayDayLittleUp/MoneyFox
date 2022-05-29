namespace MoneyFox.Views.Statistics;

using CommunityToolkit.Maui.Views;
using Popups;
using ViewModels.Statistics;

public partial class StatisticAccountMonthlyCashFlowPage
{
    public StatisticAccountMonthlyCashFlowPage()
    {
        InitializeComponent();
        BindingContext = ViewModelLocator.StatisticAccountMonthlyCashFlowViewModel;
    }

    private StatisticAccountMonthlyCashFlowViewModel ViewModel => (StatisticAccountMonthlyCashFlowViewModel)BindingContext;

    protected override void OnAppearing()
    {
        ViewModel.InitCommand.Execute(null);
    }

    private async void OpenFilterDialog(object sender, EventArgs e)
    {
        var popup = new DateSelectionPopup(dateFrom: ViewModel.StartDate, dateTo: ViewModel.EndDate);
        Shell.Current.ShowPopup(popup);
    }
}
