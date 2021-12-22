﻿#nullable enable
using CommunityToolkit.Mvvm.Input;
using MoneyFox.Uwp.ViewModels.Categories;
using MoneyFox.Uwp.ViewModels.DesignTime;
using MoneyFox.Uwp.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace MoneyFox.Uwp.ViewModels.Payments
{
    public class DesignTimePaymentListViewModel : IPaymentListViewModel
    {
        public IBalanceViewModel BalanceViewModel => new DesignTimeBalanceViewViewModel();

        public IPaymentListViewActionViewModel ViewActionViewModel { get; } = null!;

        public RelayCommand InitializeCommand { get; } = null!;

        public RelayCommand<PaymentViewModel> EditPaymentCommand { get; } = null!;

        public RelayCommand<PaymentViewModel> DeletePaymentCommand { get; } = null!;

        public CollectionViewSource GroupedPayments => new CollectionViewSource
        {
            IsSourceGrouped = true,
            Source = new List<PaymentViewModel>
            {
                new PaymentViewModel
                {
                    Amount = 123, Category = new CategoryViewModel {Name = "Beer"}, Date = DateTime.Now
                },
                new PaymentViewModel
                {
                    Amount = 123,
                    Category = new CategoryViewModel {Name = "Beer"},
                    Date = DateTime.Now.AddMonths(-1)
                },
                new PaymentViewModel
                {
                    Amount = 123,
                    Category = new CategoryViewModel {Name = "Beer"},
                    Date = DateTime.Now.AddMonths(-1)
                }
            }
        };

        public string Title => "Sparkonto";

        public int AccountId { get; } = 13;

        public RelayCommand LoadDataCommand => throw new NotSupportedException();
    }
}