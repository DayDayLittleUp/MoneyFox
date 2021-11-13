﻿using MoneyFox.Application.Common.Messages;
using MoneyFox.ViewModels.Dialogs;
using Rg.Plugins.Popup.Extensions;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace MoneyFox.Views.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterPopup
    {
        private SelectFilterDialogViewModel ViewModel => (SelectFilterDialogViewModel)BindingContext;

        public FilterPopup()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.SelectFilterDialogViewModel;
        }

        public FilterPopup(PaymentListFilterChangedMessage message)
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.SelectFilterDialogViewModel;

            ViewModel.Initialize(message);
        }

        public async Task ShowAsync() => await App.Current.MainPage.Navigation.PushPopupAsync(this);

        private static async Task DismissAsync() => await App.Current.MainPage.Navigation.PopPopupAsync();

        private async void Button_OnClicked(object sender, System.EventArgs e)
        {
            ViewModel.FilterSelectedCommand.Execute(null);
            await DismissAsync();
        }
    }
}
