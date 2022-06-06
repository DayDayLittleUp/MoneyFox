﻿namespace MoneyFox.Core.Commands.Payments.ClearPayments
{

    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using _Pending_.Common.QueryObjects;
    using Common.Interfaces;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class ClearPaymentsCommand : IRequest
    {
        public class Handler : IRequestHandler<ClearPaymentsCommand>
        {
            private readonly IContextAdapter contextAdapter;

            public Handler(IContextAdapter contextAdapter)
            {
                this.contextAdapter = contextAdapter;
            }

            public async Task<Unit> Handle(ClearPaymentsCommand request, CancellationToken cancellationToken)
            {
                var unclearedPayments = await contextAdapter.Context.Payments.Include(x => x.ChargedAccount)
                    .Include(x => x.TargetAccount)
                    .AsQueryable()
                    .AreNotCleared()
                    .ToListAsync(cancellationToken: cancellationToken);

                foreach (var payment in unclearedPayments)
                {
                    payment.ClearPayment();
                }

                await contextAdapter.Context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }

}
