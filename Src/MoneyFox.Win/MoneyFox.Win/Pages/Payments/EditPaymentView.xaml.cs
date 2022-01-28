﻿using Microsoft.UI.Xaml.Navigation;
using MoneyFox.Core.Resources;
using MoneyFox.Win.ViewModels.Payments;

namespace MoneyFox.Win.Pages.Payments
{
    public sealed partial class EditPaymentView
    {
        public override string Header => Strings.EditPaymentTitle;

        private EditPaymentViewModel ViewModel => (EditPaymentViewModel)DataContext;

        public EditPaymentView()
        {
            InitializeComponent();
            DataContext = ViewModelLocator.EditPaymentVm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var vm = (PaymentViewModel)e.Parameter;
            ViewModel.InitializeCommand.Execute(vm.Id);
        }
    }
}