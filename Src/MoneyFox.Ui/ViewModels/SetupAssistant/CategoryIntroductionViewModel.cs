﻿namespace MoneyFox.Ui.ViewModels.SetupAssistant;

using Common.Extensions;
using CommunityToolkit.Mvvm.Input;

internal sealed class CategoryIntroductionViewModel : BaseViewModel
{
    public AsyncRelayCommand GoToAddCategoryCommand => new(async () => await Shell.Current.GoToModalAsync(Routes.AddCategoryRoute));

    public AsyncRelayCommand NextStepCommand => new(async () => await Shell.Current.GoToAsync(Routes.SetupCompletionRoute));

    public AsyncRelayCommand BackCommand => new(async () => await Shell.Current.Navigation.PopAsync());
}
