﻿namespace MoneyFox.ViewModels.Payments
{

    using System.Threading.Tasks;
    using AutoMapper;
    using CommunityToolkit.Mvvm.Input;
    using Core.Commands.Payments.DeletePaymentById;
    using Core.Commands.Payments.UpdatePayment;
    using Core.Common.Interfaces;
    using Core.Queries;
    using Core.Resources;
    using MediatR;
    using Xamarin.Forms;

    public class EditPaymentViewModel : ModifyPaymentViewModel
    {
        private readonly IDialogService dialogService;
        private readonly IMapper mapper;

        private readonly IMediator mediator;

        public EditPaymentViewModel(IMediator mediator, IMapper mapper, IDialogService dialogService) : base(
            mediator: mediator,
            mapper: mapper,
            dialogService: dialogService)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.dialogService = dialogService;
        }

        public RelayCommand<PaymentViewModel> DeleteCommand => new RelayCommand<PaymentViewModel>(async p => await DeletePaymentAsync(p));

        public async Task InitializeAsync(int paymentId)
        {
            await base.InitializeAsync();
            SelectedPayment = mapper.Map<PaymentViewModel>(await mediator.Send(new GetPaymentByIdQuery(paymentId)));
        }

        protected override async Task SavePaymentAsync()
        {
            var updateRecurring = false;
            if (SelectedPayment.IsRecurring)
            {
                updateRecurring = await dialogService.ShowConfirmMessageAsync(
                    title: Strings.ModifyRecurrenceTitle,
                    message: Strings.ModifyRecurrenceMessage,
                    positiveButtonText: Strings.YesLabel,
                    negativeButtonText: Strings.NoLabel);
            }

            var command = new UpdatePaymentCommand(
                id: SelectedPayment.Id,
                date: SelectedPayment.Date,
                amount: SelectedPayment.Amount,
                isCleared: SelectedPayment.IsCleared,
                type: SelectedPayment.Type,
                note: SelectedPayment.Note,
                isRecurring: SelectedPayment.IsRecurring,
                categoryId: SelectedPayment.Category?.Id ?? 0,
                chargedAccountId: SelectedPayment.ChargedAccount?.Id ?? 0,
                targetAccountId: SelectedPayment.TargetAccount?.Id ?? 0,
                updateRecurringPayment: updateRecurring,
                recurrence: SelectedPayment.RecurringPayment?.Recurrence,
                isEndless: SelectedPayment.RecurringPayment?.IsEndless,
                endDate: SelectedPayment.RecurringPayment?.EndDate);

            await mediator.Send(command);
        }

        private async Task DeletePaymentAsync(PaymentViewModel payment)
        {
            if (await dialogService.ShowConfirmMessageAsync(title: Strings.DeleteTitle, message: Strings.DeletePaymentConfirmationMessage))
            {
                var deleteCommand = new DeletePaymentByIdCommand(payment.Id);
                if (SelectedPayment.IsRecurring)
                {
                    deleteCommand.DeleteRecurringPayment = await dialogService.ShowConfirmMessageAsync(
                        title: Strings.DeleteRecurringPaymentTitle,
                        message: Strings.DeleteRecurringPaymentMessage);
                }

                try
                {
                    await dialogService.ShowLoadingDialogAsync();
                    await mediator.Send(deleteCommand);
                    await Shell.Current.Navigation.PopModalAsync();
                }
                finally
                {
                    await dialogService.HideLoadingDialogAsync();
                }
            }
        }
    }

}
