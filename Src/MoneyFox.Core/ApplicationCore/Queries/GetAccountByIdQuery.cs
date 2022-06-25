﻿namespace MoneyFox.Core.ApplicationCore.Queries
{

    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Aggregates.AccountAggregate;
    using MediatR;
    using MoneyFox.Core.Common.Interfaces;

    public class GetAccountByIdQuery : IRequest<Account?>
    {
        public GetAccountByIdQuery(int accountId)
        {
            AccountId = accountId;
        }

        public int AccountId { get; }

        public class Handler : IRequestHandler<GetAccountByIdQuery, Account?>
        {
            private readonly IAppDbContext appDbContext;

            public Handler(IAppDbContext appDbContext)
            {
                this.appDbContext = appDbContext;
            }

            public async Task<Account?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
            {
                return await appDbContext.Accounts.FindAsync(request.AccountId);
            }
        }
    }

}
