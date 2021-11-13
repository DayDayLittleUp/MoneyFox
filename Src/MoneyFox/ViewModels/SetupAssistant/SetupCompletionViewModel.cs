﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MoneyFox.Application.Common.Facades;
using Xamarin.Forms;

namespace MoneyFox.ViewModels.SetupAssistant
{
    public class SetupCompletionViewModel : ViewModelBase
    {
        private readonly ISettingsFacade settingsFacade;

        public SetupCompletionViewModel(ISettingsFacade settingsFacade)
        {
            this.settingsFacade = settingsFacade;
        }

        public RelayCommand CompleteCommand
            => new RelayCommand(CompleteSetup);

        public RelayCommand BackCommand
            => new RelayCommand(async () => await Shell.Current.Navigation.PopAsync());

        private void CompleteSetup()
        {
            settingsFacade.IsSetupCompleted = true;
            Xamarin.Forms.Application.Current.MainPage = new AppShell();
        }
    }
}