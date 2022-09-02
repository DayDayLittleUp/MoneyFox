namespace MoneyFox.Ui;

using System.Globalization;
using Common.Exceptions;
using InversionOfControl;
using MediatR;
using MoneyFox.Core.ApplicationCore.UseCases.DbBackup;
using MoneyFox.Core.Commands.Payments.ClearPayments;
using MoneyFox.Core.Commands.Payments.CreateRecurringPayments;
using MoneyFox.Core.Common.Facades;
using MoneyFox.Core.Common.Helpers;
using MoneyFox.Core.Common.Interfaces;
using MoneyFox.Infrastructure.Adapters;
using Serilog;
using ViewModels;

public partial class App
{
    private bool isRunning;

    public App()
    {
        var settingsFacade = new SettingsFacade(new SettingsAdapter());
        // TODO: use setting?
        CultureHelper.CurrentCulture = new(CultureInfo.CurrentCulture.Name);
        InitializeComponent();
        SetupServices();
        MainPage = new AppShell();
        if (!settingsFacade.IsSetupCompleted)
        {
            //Shell.Current.GoToAsync(Routes.WelcomeViewRoute).Wait();
        }
    }

    public static Action<IServiceCollection>? AddPlatformServicesAction { get; set; }

    private static IServiceProvider? ServiceProvider { get; set; }

    internal static BaseViewModel GetViewModel<TViewModel>() where TViewModel : BaseViewModel
    {
        return ServiceProvider?.GetService<TViewModel>() ?? throw new ResolveViewModelException<TViewModel>();
    }

    protected override void OnStart()
    {
        StartupTasksAsync().ConfigureAwait(false);
    }

    protected override void OnResume()
    {
        StartupTasksAsync().ConfigureAwait(false);
    }

    private static void SetupServices()
    {
        var services = new ServiceCollection();
        AddPlatformServicesAction?.Invoke(services);
        new MoneyFoxConfig().Register(services);
        ServiceProvider = services.BuildServiceProvider();
        ServiceProvider.GetService<IAppDbContext>()?.Migratedb();
    }

    private async Task StartupTasksAsync()
    {
        // Don't execute this again when already running
        if (isRunning)
        {
            return;
        }

        if (ServiceProvider == null)
        {
            return;
        }

        isRunning = true;
        var settingsFacade = ServiceProvider.GetService<ISettingsFacade>() ?? throw new ResolveDependencyException<ISettingsFacade>();
        var mediator = ServiceProvider.GetService<IMediator>() ?? throw new ResolveDependencyException<IMediator>();
        try
        {
            if (settingsFacade.IsBackupAutoUploadEnabled && settingsFacade.IsLoggedInToBackupService)
            {
                var backupService = ServiceProvider.GetService<IBackupService>() ?? throw new ResolveDependencyException<IBackupService>();
                await backupService.RestoreBackupAsync();
            }

            await mediator.Send(new ClearPaymentsCommand());
            await mediator.Send(new CreateRecurringPaymentsCommand());
        }
        catch (Exception ex)
        {
            Log.Fatal(exception: ex, messageTemplate: "Error during startup");
        }
        finally
        {
            settingsFacade.LastExecutionTimeStampSyncBackup = DateTime.Now;
            isRunning = false;
        }
    }
}
