﻿namespace MoneyFox.Core.Commands.Categories.UpdateCategory;

using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Domain.Aggregates.CategoryAggregate;
using Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class UpdateCategoryCommand : IRequest
{
    public UpdateCategoryCommand(Category category)
    {
        Category = category;
    }

    public Category Category { get; }

    public class Handler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IAppDbContext appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            Category existingCategory = await appDbContext.Categories.SingleAsync(c => c.Id == request.Category.Id, cancellationToken: cancellationToken);
            existingCategory.UpdateData(name: request.Category.Name, note: request.Category.Note ?? "", requireNote: request.Category.RequireNote);
            _ = await appDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

