namespace MoneyFox.Ui.Views.Popups;

using Core.Common.Messages;

public partial class DateSelectionPopup
{
    public DateSelectionPopup(DateTime dateFrom, DateTime dateTo)
    {
        InitializeComponent();
        BindingContext = App.GetViewModel<SelectDateRangeDialogViewModel>();
        ViewModel.StartDate = dateFrom;
        ViewModel.EndDate = dateTo;
    }

    public DateSelectionPopup(DateSelectedMessage message)
    {
        InitializeComponent();
        BindingContext = App.GetViewModel<SelectDateRangeDialogViewModel>();
        ViewModel.Initialize(message);
    }

    private SelectDateRangeDialogViewModel ViewModel => (SelectDateRangeDialogViewModel)BindingContext;

    private void Button_OnClicked(object sender, EventArgs e)
    {
        ViewModel.DoneCommand.Execute(null);
        Close();
    }
}
