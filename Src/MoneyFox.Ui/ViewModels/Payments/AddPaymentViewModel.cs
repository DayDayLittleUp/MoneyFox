namespace MoneyFox.Ui.ViewModels.Payments;

using AutoMapper;
using Core.ApplicationCore.Domain.Aggregates.AccountAggregate;
using Core.ApplicationCore.Domain.Aggregates.CategoryAggregate;
using Core.ApplicationCore.Queries;
using Core.Commands.Payments.CreatePayment;
using Core.Common.Interfaces;
using JetBrains.Annotations;
using MediatR;

[UsedImplicitly]
internal sealed class AddPaymentViewModel : ModifyPaymentViewModel
{
    private readonly IMapper mapper;
    private readonly IMediator mediator;

    public AddPaymentViewModel(IMediator mediator, IMapper mapper, IDialogService dialogService) : base(
        mediator: mediator,
        mapper: mapper,
        dialogService: dialogService)
    {
        this.mediator = mediator;
        this.mapper = mapper;
    }

    public async Task InitializeAsync(int? defaultChargedAccountID = null)
    {
        await base.InitializeAsync();
        SelectedPayment.ChargedAccount = defaultChargedAccountID.HasValue
            ? ChargedAccounts.First(n => n.Id == defaultChargedAccountID.Value)
            : ChargedAccounts.First();
    }

    protected override async Task SavePaymentAsync()
    {
        var payment = new Payment(
            date: SelectedPayment.Date,
            amount: SelectedPayment.Amount,
            type: SelectedPayment.Type,
            chargedAccount: await mediator.Send(new GetAccountByIdQuery(SelectedPayment.ChargedAccount.Id)),
            targetAccount: SelectedPayment.TargetAccount != null ? await mediator.Send(new GetAccountByIdQuery(SelectedPayment.TargetAccount.Id)) : null,
            category: mapper.Map<Category>(SelectedPayment.Category),
            note: SelectedPayment.Note);

        if (SelectedPayment.IsRecurring && SelectedPayment.RecurringPayment != null)
        {
            payment.AddRecurringPayment(
                recurrence: SelectedPayment.RecurringPayment.Recurrence,
                isLastDayOfMonth: SelectedPayment.RecurringPayment.IsLastDayOfMonth,
                endDate: SelectedPayment.RecurringPayment.IsEndless ? null : SelectedPayment.RecurringPayment.EndDate);
        }

        await mediator.Send(new CreatePaymentCommand(payment));
    }
}


