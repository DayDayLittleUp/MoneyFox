﻿using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace MoneyFox.Uwp.ViewModels.Statistic.StatisticCategorySummary
{
    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Design Time")]
    public class DesignTimeStatisticCategorySummaryViewModel : IStatisticCategorySummaryViewModel
    {
        /// <inheritdoc/>
        public IncomeExpenseBalanceViewModel IncomeExpenseBalance { get; set; }
            = new IncomeExpenseBalanceViewModel { TotalEarned = 400, TotalSpent = 600 };

        public ObservableCollection<CategoryOverviewViewModel> CategorySummary
                                                               => new ObservableCollection<CategoryOverviewViewModel>
        {
            new CategoryOverviewViewModel
            {
                Label = "Einkaufen",
                Value = 745,
                Percentage = 30
            },
            new CategoryOverviewViewModel
            {
                Label = "Beeeeer",
                Value = 666,
                Percentage = 70
            }
        };

        public bool HasData { get; } = true;

        public RelayCommand LoadedCommand { get; } = null!;
        public RelayCommand<CategoryOverviewViewModel> SummaryEntrySelectedCommand { get; } = null!;
    }
}
