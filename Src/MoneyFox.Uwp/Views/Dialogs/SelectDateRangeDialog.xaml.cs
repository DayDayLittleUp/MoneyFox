﻿#nullable enable
namespace MoneyFox.Uwp.Views.Dialogs
{
    public sealed partial class SelectDateRangeDialog
    {
        public SelectDateRangeDialog()
        {
            InitializeComponent();

            DataContext = ViewModelLocator.SelectDateRangeDialogVm;
        }
    }
}
