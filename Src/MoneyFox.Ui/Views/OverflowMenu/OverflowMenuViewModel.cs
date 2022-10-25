namespace MoneyFox.Ui.Views.OverflowMenu;

using About;
using Backup;
using Budget;
using Categories;
using CommunityToolkit.Mvvm.Input;
using Core.Interfaces;
using Core.Resources;
using JetBrains.Annotations;
using Settings;
using ViewModels;

[UsedImplicitly]
internal sealed class OverflowMenuViewModel : BaseViewModel
{
    private readonly INavigationService navigationService;

    public OverflowMenuViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }

    public AsyncRelayCommand<OverflowItemViewModel> GoToSelectedItemCommand => new(async s => await GoToSelectedItem(s.Type));

    public List<OverflowItemViewModel> OverflowEntries
        => new()
        {
            new() { IconGlyph = IconFont.PiggyBankOutline, Name = Strings.BudgetsTitle, Type = OverflowMenuItemType.Budgets },
            new() { IconGlyph = IconFont.TagOutline, Name = Strings.CategoriesTitle, Type = OverflowMenuItemType.Categories },
            new() { IconGlyph = IconFont.CloudUploadOutline, Name = Strings.BackupTitle, Type = OverflowMenuItemType.Backup },
            new() { IconGlyph = IconFont.CogOutline, Name = Strings.SettingsTitle, Type = OverflowMenuItemType.Settings },
            new() { IconGlyph = IconFont.InformationOutline, Name = Strings.AboutTitle, Type = OverflowMenuItemType.About }
        };

    private async Task GoToSelectedItem(OverflowMenuItemType menuType)
    {
        switch (menuType)
        {
            case OverflowMenuItemType.Budgets:
                await navigationService.NavigateToAsync<BudgetListPage>();

                break;
            case OverflowMenuItemType.Categories:
                await navigationService.NavigateToAsync<CategoryListPage>();

                break;
            case OverflowMenuItemType.Backup:
                await navigationService.NavigateToAsync<BackupPage>();

                break;
            case OverflowMenuItemType.Settings:
                await navigationService.NavigateToAsync<SettingsPage>();

                break;
            case OverflowMenuItemType.About:
                await navigationService.NavigateToAsync<AboutPage>();

                break;
        }
    }
}



