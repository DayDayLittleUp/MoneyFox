namespace MoneyFox.Infrastructure.Persistence;

using System;
using System.Threading;
using System.Threading.Tasks;
using Core.ApplicationCore.Domain.Aggregates;
using Core.ApplicationCore.Domain.Aggregates.AccountAggregate;
using Core.ApplicationCore.Domain.Aggregates.BudgetAggregate;
using Core.ApplicationCore.Domain.Aggregates.CategoryAggregate;
using Core.Common.Facades;
using Core.Common.Interfaces;
using Core.Common.Mediatr;
using Core.Notifications.DatabaseChanged;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly ICustomPublisher? publisher;
    private readonly ISettingsFacade? settingsFacade;

    public AppDbContext(DbContextOptions options, ICustomPublisher? publisher, ISettingsFacade? settingsFacade) : base(options)
    {
        this.publisher = publisher;
        this.settingsFacade = settingsFacade;
    }

    public DbSet<Budget> Budgets { get; set; } = null!;

    public DbSet<Account> Accounts { get; set; } = null!;

    public DbSet<Payment> Payments { get; set; } = null!;

    public DbSet<RecurringPayment> RecurringPayments { get; set; } = null!;

    public DbSet<Category> Categories { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = DateTime.Now;
                    entry.Entity.LastModified = DateTime.Now;

                    break;
                case EntityState.Modified:
                    entry.Entity.LastModified = DateTime.Now;

                    break;
            }
        }

        var changeCount = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        if (publisher == null || settingsFacade == null)
        {
            return changeCount;
        }

        // dispatch events only if save was successful
        if (changeCount > 0)
        {
            await publisher.Publish(
                notification: new DataBaseChanged.Notification(),
                strategy: PublishStrategy.ParallelNoWait,
                cancellationToken: cancellationToken);
        }

        return changeCount;
    }

    public void Migratedb()
    {
        Database.Migrate();
    }

    public void ReleaseLock()
    {
        SqliteConnection.ClearAllPools();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }
}
