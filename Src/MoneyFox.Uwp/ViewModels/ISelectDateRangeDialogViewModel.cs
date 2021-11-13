﻿using GalaSoft.MvvmLight.Command;
using System;

#nullable enable
namespace MoneyFox.Uwp.ViewModels
{
    public interface ISelectDateRangeDialogViewModel
    {
        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

        RelayCommand DoneCommand { get; set; }
    }
}
