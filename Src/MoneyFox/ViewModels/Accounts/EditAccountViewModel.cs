﻿using AutoMapper;
using MediatR;
using MoneyFox.Application.Accounts.Commands.UpdateAccount;
using MoneyFox.Application.Accounts.Queries.GetAccountById;
using MoneyFox.Application.Common.Interfaces;
using MoneyFox.Domain.Entities;
using System.Threading.Tasks;

namespace MoneyFox.ViewModels.Accounts
{
    public class EditAccountViewModel : ModifyAccountViewModel
    {
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public EditAccountViewModel(IMediator mediator,
            IMapper mapper,
            IDialogService dialogService)
            : base(dialogService)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public override bool IsEdit => true;

        public async Task InitializeAsync(int accountId) => SelectedAccountVm =
            mapper.Map<AccountViewModel>(await mediator.Send(new GetAccountByIdQuery(accountId)));

        protected override async Task SaveAccountAsync()
            => await mediator.Send(new UpdateAccountCommand(mapper.Map<Account>(SelectedAccountVm)));
    }
}