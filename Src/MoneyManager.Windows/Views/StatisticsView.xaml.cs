﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using MoneyManager.Core.ViewModels;
using MoneyManager.Windows.Dialogs;
using MvvmCross.Platform;

namespace MoneyManager.Windows.Views
{
    public sealed partial class StatisticsView
    {
        public StatisticsView()
        {
            InitializeComponent();
            DataContext = Mvx.Resolve<StatisticViewModel>();
        }

        private async void SetDate(object sender, RoutedEventArgs e)
        {
            await new SelectDateRangeDialog().ShowAsync();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            CashFlowUserControl.Dispose();
            CategorySpreadingUserControl.Dispose();

            base.OnNavigatingFrom(e);
        }
    }
}