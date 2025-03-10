namespace MoneyFox.Ui.Views.Backup;

using CommunityToolkit.Mvvm.Input;
using Core.ApplicationCore.Domain.Exceptions;
using Core.ApplicationCore.UseCases.BackupUpload;
using Core.ApplicationCore.UseCases.DbBackup;
using Core.Common.Facades;
using Core.Common.Interfaces;
using Core.Interfaces;
using MediatR;
using Resources.Strings;
using Serilog;
using ViewModels;

internal sealed class BackupViewModel : BaseViewModel
{
    private readonly IBackupService backupService;
    private readonly IConnectivityAdapter connectivity;
    private readonly IDialogService dialogService;
    private readonly IMediator mediator;
    private readonly IOneDriveProfileService oneDriveProfileService;
    private readonly ISettingsFacade settingsFacade;
    private readonly IToastService toastService;
    private bool backupAvailable;

    private DateTime backupLastModified;
    private bool isLoadingBackupAvailability;

    private ImageSource? profilePicture;
    private UserAccountViewModel userAccount = new();

    public BackupViewModel(
        IMediator mediator,
        IBackupService backupService,
        IDialogService dialogService,
        IConnectivityAdapter connectivity,
        ISettingsFacade settingsFacade,
        IToastService toastService,
        IOneDriveProfileService oneDriveProfileService)
    {
        this.backupService = backupService;
        this.dialogService = dialogService;
        this.connectivity = connectivity;
        this.settingsFacade = settingsFacade;
        this.toastService = toastService;
        this.mediator = mediator;
        this.oneDriveProfileService = oneDriveProfileService;
    }

    public UserAccountViewModel UserAccount
    {
        get => userAccount;

        set
        {
            if (userAccount == value)
            {
                return;
            }

            userAccount = value;
            OnPropertyChanged();
        }
    }

    public ImageSource? ProfilePicture
    {
        get => profilePicture;
        set => SetProperty(field: ref profilePicture, newValue: value);
    }

    public RelayCommand InitializeCommand => new(async () => await InitializeAsync());

    public RelayCommand LoginCommand => new(async () => await LoginAsync());

    public RelayCommand LogoutCommand => new(async () => await LogoutAsync());

    public RelayCommand BackupCommand => new(async () => await CreateBackupAsync());

    public RelayCommand RestoreCommand => new(async () => await RestoreBackupAsync());

    public DateTime BackupLastModified
    {
        get => backupLastModified;

        private set
        {
            if (backupLastModified == value)
            {
                return;
            }

            backupLastModified = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     Indicator that the app is checking if backups available.
    /// </summary>
    public bool IsLoadingBackupAvailability
    {
        get => isLoadingBackupAvailability;

        private set
        {
            if (isLoadingBackupAvailability == value)
            {
                return;
            }

            isLoadingBackupAvailability = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     Indicator that the user logged in to the backup service.
    /// </summary>
    public bool IsLoggedIn => settingsFacade.IsLoggedInToBackupService;

    /// <summary>
    ///     Indicates if a backup is available for restore.
    /// </summary>
    public bool BackupAvailable
    {
        get => backupAvailable;

        private set
        {
            if (backupAvailable == value)
            {
                return;
            }

            backupAvailable = value;
            OnPropertyChanged();
        }
    }

    public bool IsAutoBackupEnabled
    {
        get => settingsFacade.IsBackupAutoUploadEnabled;

        set
        {
            if (settingsFacade.IsBackupAutoUploadEnabled == value)
            {
                return;
            }

            settingsFacade.IsBackupAutoUploadEnabled = value;
            OnPropertyChanged();
        }
    }

    private async Task InitializeAsync()
    {
        await LoadedAsync();
    }

    private async Task LoadedAsync()
    {
        if (!IsLoggedIn)
        {
            OnPropertyChanged(nameof(IsLoggedIn));

            return;
        }

        if (!connectivity.IsConnected)
        {
            await dialogService.ShowMessageAsync(title: Translations.NoNetworkTitle, message: Translations.NoNetworkMessage);
        }

        IsLoadingBackupAvailability = true;
        try
        {
            BackupAvailable = await backupService.IsBackupExistingAsync();
            BackupLastModified = await backupService.GetBackupDateAsync();
            var userAccountDto = await oneDriveProfileService.GetUserAccountAsync();
            UserAccount.Name = userAccountDto.Name;
            UserAccount.Email = userAccountDto.Email;
            var profilePictureStream = await oneDriveProfileService.GetProfilePictureAsync();
            if (profilePictureStream != null)
            {
                ProfilePicture = ImageSource.FromStream(() => profilePictureStream);
            }
        }
        catch (BackupAuthenticationFailedException ex)
        {
            Log.Error(exception: ex, messageTemplate: "Issue during Login process");
            await backupService.LogoutAsync();
            await dialogService.ShowMessageAsync(title: Translations.AuthenticationFailedTitle, message: Translations.ErrorMessageAuthenticationFailed);
        }
        catch (Exception ex)
        {
            if (ex.StackTrace == "4f37.717b")
            {
                await backupService.LogoutAsync();
                await dialogService.ShowMessageAsync(title: Translations.AuthenticationFailedTitle, message: Translations.ErrorMessageAuthenticationFailed);
            }

            Log.Error(exception: ex, messageTemplate: "Issue on loading backup view");
        }

        IsLoadingBackupAvailability = false;
    }

    private async Task LoginAsync()
    {
        if (!connectivity.IsConnected)
        {
            Log.Information("Tried to log in, but device isn't connected to the internet");
            await dialogService.ShowMessageAsync(title: Translations.NoNetworkTitle, message: Translations.NoNetworkMessage);
        }

        try
        {
            await backupService.LoginAsync();
            var userAccountDto = await oneDriveProfileService.GetUserAccountAsync();
            UserAccount.Name = userAccountDto.Name;
            UserAccount.Email = userAccountDto.Email;
        }
        catch (BackupOperationCanceledException)
        {
            await dialogService.ShowMessageAsync(title: Translations.CanceledTitle, message: Translations.LoginCanceledMessage);
        }
        catch (Exception ex)
        {
            Log.Error(exception: ex, messageTemplate: "Login Failed");
            await dialogService.ShowMessageAsync(
                title: Translations.LoginFailedTitle,
                message: string.Format(format: Translations.UnknownErrorMessage, arg0: ex.Message));
        }

        OnPropertyChanged(nameof(IsLoggedIn));
        await LoadedAsync();
    }

    private async Task LogoutAsync()
    {
        try
        {
            await backupService.LogoutAsync();
        }
        catch (BackupOperationCanceledException)
        {
            await dialogService.ShowMessageAsync(title: Translations.CanceledTitle, message: Translations.LogoutCanceledMessage);
        }
        catch (Exception ex)
        {
            Log.Error(exception: ex, messageTemplate: "Logout Failed");
            await dialogService.ShowMessageAsync(title: Translations.GeneralErrorTitle, message: ex.Message);
        }

        // ReSharper disable once ExplicitCallerInfoArgument
        OnPropertyChanged(nameof(IsLoggedIn));
    }

    private async Task CreateBackupAsync()
    {
        if (!await ShowOverwriteBackupInfoAsync())
        {
            return;
        }

        await dialogService.ShowLoadingDialogAsync();
        try
        {
            var result = await mediator.Send(new UploadBackup.Command());
            await ShowUploadResult(result);
        }
        catch (BackupOperationCanceledException)
        {
            await dialogService.ShowMessageAsync(title: Translations.CanceledTitle, message: Translations.UploadBackupCanceledMessage);
        }
        catch (Exception ex)
        {
            Log.Error(exception: ex, messageTemplate: "Create Backup failed");
            await dialogService.ShowMessageAsync(title: Translations.BackupFailedTitle, message: ex.Message);
        }

        await dialogService.HideLoadingDialogAsync();
    }

    private async Task ShowUploadResult(UploadBackup.UploadResult uploadResult)
    {
        switch (uploadResult)
        {
            case UploadBackup.UploadResult.Successful:
                await toastService.ShowToastAsync(Translations.BackupCreatedMessage);
                BackupLastModified = DateTime.Now;

                break;
            case UploadBackup.UploadResult.Skipped:
                await toastService.ShowToastAsync(Translations.BackupUploadSkippedMessage);

                break;
        }
    }

    private async Task RestoreBackupAsync()
    {
        if (!await ShowOverwriteDataInfoAsync())
        {
            return;
        }

        await dialogService.ShowLoadingDialogAsync();
        var backupDate = await backupService.GetBackupDateAsync();
        if (settingsFacade.LastDatabaseUpdate <= backupDate || await ShowForceOverrideConfirmationAsync())
        {
            await dialogService.ShowLoadingDialogAsync();
            try
            {
                await backupService.RestoreBackupAsync(BackupMode.Manual);
                await toastService.ShowToastAsync(Translations.BackupRestoredMessage);
            }
            catch (BackupOperationCanceledException)
            {
                Log.Information("Restoring the backup was canceled by the user");
                await dialogService.ShowMessageAsync(title: Translations.CanceledTitle, message: Translations.RestoreBackupCanceledMessage);
            }
            catch (Exception ex)
            {
                Log.Error(exception: ex, messageTemplate: "Restore Backup failed");
                await dialogService.ShowMessageAsync(title: Translations.BackupFailedTitle, message: ex.Message);
            }
        }
        else
        {
            Log.Information("Restore Backup canceled by the user due to newer local data");
        }

        await dialogService.HideLoadingDialogAsync();
    }

    private async Task<bool> ShowOverwriteBackupInfoAsync()
    {
        return await dialogService.ShowConfirmMessageAsync(
            title: Translations.OverwriteTitle,
            message: Translations.OverwriteBackupMessage,
            positiveButtonText: Translations.YesLabel,
            negativeButtonText: Translations.NoLabel);
    }

    private async Task<bool> ShowOverwriteDataInfoAsync()
    {
        return await dialogService.ShowConfirmMessageAsync(
            title: Translations.OverwriteTitle,
            message: Translations.OverwriteDataMessage,
            positiveButtonText: Translations.YesLabel,
            negativeButtonText: Translations.NoLabel);
    }

    private async Task<bool> ShowForceOverrideConfirmationAsync()
    {
        return await dialogService.ShowConfirmMessageAsync(
            title: Translations.ForceOverrideBackupTitle,
            message: Translations.ForceOverrideBackupMessage,
            positiveButtonText: Translations.YesLabel,
            negativeButtonText: Translations.NoLabel);
    }
}
