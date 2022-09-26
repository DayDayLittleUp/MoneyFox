﻿namespace MoneyFox.Core.ApplicationCore.Queries.Statistics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using _Pending_.Common.QueryObjects;
using Common.Helpers;
using Common.Interfaces;
using Domain.Aggregates.AccountAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;

public class GetCashFlowQuery : IRequest<List<StatisticEntry>>
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}

public class GetCashFlowQueryHandler : IRequestHandler<GetCashFlowQuery, List<StatisticEntry>>
{
    private const string GREEN_HEX_CODE = "#9bcd9b";
    private const string RED_HEX_CODE = "#cd3700";
    private const string BLUE_HEX_CODE = "#87cefa";

    private readonly IAppDbContext appDbContext;

    public GetCashFlowQueryHandler(IAppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<List<StatisticEntry>> Handle(GetCashFlowQuery request, CancellationToken cancellationToken)
    {
        List<Payment> payments = await appDbContext.Payments.Include(x => x.Category)
            .WithoutTransfers()
            .HasDateLargerEqualsThan(request.StartDate.Date)
            .HasDateSmallerEqualsThan(request.EndDate.Date)
            .ToListAsync(cancellationToken);

        decimal incomeAmount = payments.Where(x => x.Type == PaymentType.Income).Sum(x => x.Amount);
        StatisticEntry income = new(incomeAmount)
        {
            Label = Strings.RevenueLabel,
            ValueLabel = Math.Round(d: incomeAmount, decimals: 2, mode: MidpointRounding.AwayFromZero)
                .ToString(format: "C", provider: CultureHelper.CurrentCulture),
            Color = GREEN_HEX_CODE
        };

        decimal expenseAmount = payments.Where(x => x.Type == PaymentType.Expense).Sum(x => x.Amount);
        StatisticEntry spent = new(expenseAmount)
        {
            Label = Strings.ExpenseLabel,
            ValueLabel = Math.Round(d: expenseAmount, decimals: 2, mode: MidpointRounding.AwayFromZero)
                .ToString(format: "C", provider: CultureHelper.CurrentCulture),
            Color = RED_HEX_CODE
        };

        decimal valueIncreased = incomeAmount - expenseAmount;
        StatisticEntry increased = new(valueIncreased)
        {
            Label = Strings.IncreaseLabel,
            ValueLabel = Math.Round(d: valueIncreased, decimals: 2, mode: MidpointRounding.AwayFromZero)
                .ToString(format: "C", provider: CultureHelper.CurrentCulture),
            Color = BLUE_HEX_CODE
        };

        return new List<StatisticEntry> { income, spent, increased };
    }
}

