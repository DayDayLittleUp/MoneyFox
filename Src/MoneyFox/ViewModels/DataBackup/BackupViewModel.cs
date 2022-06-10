﻿namespace MoneyFox.ViewModels.DataBackup
{

    using System;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Core._Pending_.Common.Facades;
    using Core.ApplicationCore.Domain.Exceptions;
    using Core.ApplicationCore.UseCases.BackupUpload;
    using Core.ApplicationCore.UseCases.DbBackup;
    using Core.Common.Interfaces;
    using Core.Interfaces;
    using Core.Resources;
    using MediatR;
    using Serilog;

    public class BackupViewModel : ObservableObject, IBackupViewModel
    {
        private readonly IMediator mediator;
        private readonly IBackupService backupService;
        private readonly IConnectivityAdapter connectivity;
        private readonly IDialogService dialogService;
        private readonly ISettingsFacade settingsFacade;
        private readonly IToastService toastService;
        private bool backupAvailable;

        private DateTime backupLastModified;
        private bool isLoadingBackupAvailability;
        private UserAccount userAccount = new UserAccount(name: "", email: "");

        public BackupViewModel(
            IMediator mediator,
            IBackupService backupService,
            IDialogService dialogService,
            IConnectivityAdapter connectivity,
            ISettingsFacade settingsFacade,
            IToastService toastService)
        {
            this.backupService = backupService;
            this.dialogService = dialogService;
            this.connectivity = connectivity;
            this.settingsFacade = settingsFacade;
            this.toastService = toastService;
            this.mediator = mediator;
        }

        public UserAccount UserAccount
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

        public RelayCommand InitializeCommand => new RelayCommand(async () => await InitializeAsync());

        public RelayCommand LoginCommand => new RelayCommand(async () => await LoginAsync());

        public RelayCommand LogoutCommand => new RelayCommand(async () => await LogoutAsync());

        public RelayCommand BackupCommand => new RelayCommand(async () => await CreateBackupAsync());

        public RelayCommand RestoreCommand => new RelayCommand(async () => await RestoreBackupAsync());

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

        /// <inheritdoc />
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
                await dialogService.ShowMessageAsync(title: Strings.NoNetworkTitle, message: Strings.NoNetworkMessage);
            }

            IsLoadingBackupAvailability = true;
            try
            {
                BackupAvailable = await backupService.IsBackupExistingAsync();
                BackupLastModified = await backupService.GetBackupDateAsync();
                UserAccount = await backupService.GetUserAccount();
            }
            catch (BackupAuthenticationFailedException ex)
            {
                Log.Error(exception: ex, messageTemplate: "Issue during Login process");
                await backupService.LogoutAsync();
                await dialogService.ShowMessageAsync(title: Strings.AuthenticationFailedTitle, message: Strings.ErrorMessageAuthenticationFailed);
            }
            catch (Exception ex)
            {
                if (ex.StackTrace == "4f37.717b")
                {
                    await backupService.LogoutAsync();
                    await dialogService.ShowMessageAsync(title: Strings.AuthenticationFailedTitle, message: Strings.ErrorMessageAuthenticationFailed);
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
                await dialogService.ShowMessageAsync(title: Strings.NoNetworkTitle, message: Strings.NoNetworkMessage);
            }

            try
            {
                await backupService.LoginAsync();
                UserAccount = await backupService.GetUserAccount();
            }
            catch (BackupOperationCanceledException)
            {
                await dialogService.ShowMessageAsync(title: Strings.CanceledTitle, message: Strings.LoginCanceledMessage);
            }
            catch (Exception ex)
            {
                Log.Error(exception: ex, messageTemplate: "Login Failed");
                await dialogService.ShowMessageAsync(
                    title: Strings.LoginFailedTitle,
                    message: string.Format(format: Strings.UnknownErrorMessage, arg0: ex.Message));
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
                await dialogService.ShowMessageAsync(title: Strings.CanceledTitle, message: Strings.LogoutCanceledMessage);
            }
            catch (Exception ex)
            {
                Log.Error(exception: ex, messageTemplate: "Logout Failed");
                await dialogService.ShowMessageAsync(title: Strings.GeneralErrorTitle, message: ex.Message);
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
                await dialogService.ShowMessageAsync(title: Strings.CanceledTitle, message: Strings.UploadBackupCanceledMessage);
            }
            catch (Exception ex)
            {
                Log.Error(exception: ex, messageTemplate: "Create Backup failed");
                await dialogService.ShowMessageAsync(title: Strings.BackupFailedTitle, message: ex.Message);
            }

            await dialogService.HideLoadingDialogAsync();
        }

        private async Task ShowUploadResult(UploadBackup.UploadResult uploadResult)
        {
            switch (uploadResult)
            {
                case UploadBackup.UploadResult.Successful:
                    await toastService.ShowToastAsync(Strings.BackupCreatedMessage);
                    BackupLastModified = DateTime.Now;

                    break;
                case UploadBackup.UploadResult.Skipped:
                    await toastService.ShowToastAsync(Strings.BackupUploadSkippedMessage);

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
                    await toastService.ShowToastAsync(Strings.BackupRestoredMessage);
                }
                catch (BackupOperationCanceledException)
                {
                    Log.Information("Restoring the backup was canceled by the user");
                    await dialogService.ShowMessageAsync(title: Strings.CanceledTitle, message: Strings.RestoreBackupCanceledMessage);
                }
                catch (Exception ex)
                {
                    Log.Error(exception: ex, messageTemplate: "Restore Backup failed");
                    await dialogService.ShowMessageAsync(title: Strings.BackupFailedTitle, message: ex.Message);
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
                title: Strings.OverwriteTitle,
                message: Strings.OverwriteBackupMessage,
                positiveButtonText: Strings.YesLabel,
                negativeButtonText: Strings.NoLabel);
        }

        private async Task<bool> ShowOverwriteDataInfoAsync()
        {
            return await dialogService.ShowConfirmMessageAsync(
                title: Strings.OverwriteTitle,
                message: Strings.OverwriteDataMessage,
                positiveButtonText: Strings.YesLabel,
                negativeButtonText: Strings.NoLabel);
        }

        private async Task<bool> ShowForceOverrideConfirmationAsync()
        {
            return await dialogService.ShowConfirmMessageAsync(
                title: Strings.ForceOverrideBackupTitle,
                message: Strings.ForceOverrideBackupMessage,
                positiveButtonText: Strings.YesLabel,
                negativeButtonText: Strings.NoLabel);
        }
    }

}
