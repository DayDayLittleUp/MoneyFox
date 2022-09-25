﻿namespace MoneyFox.Views.Budget;

using ViewModels.Budget;

public partial class BudgetListPage
{
    public BudgetListPage()
    {
        InitializeComponent();
        BindingContext = App.GetViewModel<BudgetListViewModel>();
    }

    public BudgetListViewModel ViewModel => (BudgetListViewModel)BindingContext;

    protected override async void OnAppearing()
    {
        await ViewModel.InitializeCommand.ExecuteAsync(null);
    }
}
