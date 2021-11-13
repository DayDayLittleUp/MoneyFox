﻿using MoneyFox.Application.Resources;
using System.Threading.Tasks;
using UniversalRateReminder;

#nullable enable
namespace MoneyFox.Uwp.Services
{
    internal static class RateDisplayService
    {
        internal static async Task ShowIfAppropriateAsync()
        {
            RatePopup.RateButtonText = Strings.YesLabel;
            RatePopup.CancelButtonText = Strings.NotNowLabel;
            RatePopup.Title = Strings.RateReminderTitle;
            RatePopup.Content = Strings.RateReminderText;

            await RatePopup.CheckRateReminderAsync();
        }
    }
}
