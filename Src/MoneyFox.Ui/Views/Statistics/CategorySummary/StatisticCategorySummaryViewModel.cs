namespace MoneyFox.Ui.Views.Statistics.CategorySummary;

using System.Collections.ObjectModel;
using Common.Extensions;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.ApplicationCore.Queries.Statistics.GetCategorySummary;
using Core.Common.Interfaces;
using MediatR;
using Serilog;
using ViewModels.Statistics;

internal sealed class StatisticCategorySummaryViewModel : StatisticViewModel
{
    private readonly IDialogService dialogService;

    private ObservableCollection<CategoryOverviewViewModel> categorySummary = new();

    public StatisticCategorySummaryViewModel(IMediator mediator, IDialogService dialogService) : base(mediator)
    {
        this.dialogService = dialogService;
        CategorySummary = new();
    }

    public ObservableCollection<CategoryOverviewViewModel> CategorySummary
    {
        get => categorySummary;

        private set
        {
            categorySummary = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasData));
        }
    }

    public bool HasData => CategorySummary.Any();

    public RelayCommand<CategoryOverviewViewModel> ShowCategoryPaymentsCommand => new(async vm => await ShowCategoryPaymentsAsync(vm));

    protected override async Task LoadAsync()
    {
        try
        {
            var categorySummaryModel = await Mediator.Send(new GetCategorySummaryQuery { EndDate = EndDate, StartDate = StartDate });
            CategorySummary = new(
                categorySummaryModel.CategoryOverviewItems.Select(
                    x => new CategoryOverviewViewModel
                    {
                        CategoryId = x.CategoryId,
                        Value = x.Value,
                        Average = x.Average,
                        Label = x.Label,
                        Percentage = x.Percentage
                    }));
        }
        catch (Exception ex)
        {
            Log.Warning(exception: ex, messageTemplate: "Error during loading");
            await dialogService.ShowMessageAsync(title: "Error", message: ex.ToString());
        }
    }

    private async Task ShowCategoryPaymentsAsync(CategoryOverviewViewModel categoryOverviewModel)
    {
        await Shell.Current.GoToModalAsync(Routes.PaymentForCategoryListRoute);
        Messenger.Send(new PaymentsForCategoryMessage(categoryId: categoryOverviewModel.CategoryId, startDate: StartDate, endDate: EndDate));
    }
}
