﻿using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MoneyFox.Application.Payments.Queries.GetPaymentsForCategory;
using MoneyFox.Groups;
using MoneyFox.ViewModels.Payments;
using MoneyFox.Views.Payments;
using NLog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MoneyFox.ViewModels.Statistics
{
    public class PaymentForCategoryListViewModel : ViewModelBase
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IMapper mapper;

        private readonly IMediator mediator;

        private ObservableCollection<DateListGroupCollection<PaymentViewModel>> paymentList =
            new ObservableCollection<DateListGroupCollection<PaymentViewModel>>();

        private PaymentsForCategoryMessage? receivedMessage;

        public PaymentForCategoryListViewModel(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;

            PaymentList = new ObservableCollection<DateListGroupCollection<PaymentViewModel>>();
            MessengerInstance.Register<PaymentsForCategoryMessage>(
                this,
                async m =>
                {
                    receivedMessage = m;
                    await InitializeAsync();
                });
        }

        public ObservableCollection<DateListGroupCollection<PaymentViewModel>> PaymentList
        {
            get => paymentList;
            private set
            {
                paymentList = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<PaymentViewModel> GoToEditPaymentCommand
            => new RelayCommand<PaymentViewModel>(
                async paymentViewModel
                    => await Shell.Current.Navigation.PushModalAsync(
                        new NavigationPage(new EditPaymentPage(paymentViewModel.Id))
                        {
                            BarBackgroundColor = Color.Transparent
                        }));

        private async Task InitializeAsync()
        {
            if(receivedMessage == null)
            {
                logger.Error("No message received");
                return;
            }

            logger.Info($"Loading payments for category with id {receivedMessage.CategoryId}");

            var loadedPayments = mapper.Map<List<PaymentViewModel>>(
                await mediator.Send(
                    new GetPaymentsForCategoryQuery(
                        receivedMessage.CategoryId,
                        receivedMessage.StartDate,
                        receivedMessage.EndDate)));

            var dailyItems
                = DateListGroupCollection<PaymentViewModel>.CreateGroups(
                    loadedPayments,
                    s => s.Date.ToString("D", CultureInfo.CurrentCulture),
                    s => s.Date);

            PaymentList = new ObservableCollection<DateListGroupCollection<PaymentViewModel>>(dailyItems);
        }
    }
}