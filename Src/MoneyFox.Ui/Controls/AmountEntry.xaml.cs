namespace MoneyFox.Ui.Controls;

public partial class AmountEntry : ContentView
{
    public static readonly BindableProperty AmountFieldTitleProperty = BindableProperty.Create(
        propertyName: nameof(AmountFieldTitle),
        returnType: typeof(string),
        declaringType: typeof(AmountEntry),
        defaultValue: string.Empty);

    public static readonly BindableProperty AmountProperty = BindableProperty.Create(
        propertyName: nameof(Amount),
        returnType: typeof(string),
        declaringType: typeof(AmountEntry),
        defaultValue: string.Empty,
        defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty EntryPlaceholderProperty = BindableProperty.Create(
        propertyName: nameof(EntryPlaceholder),
        returnType: typeof(string),
        declaringType: typeof(AmountEntry),
        defaultValue: string.Empty);

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(
        propertyName: nameof(IsReadOnly),
        returnType: typeof(bool),
        declaringType: typeof(AmountEntry),
        defaultValue: false);

    public AmountEntry()
    {
        InitializeComponent();
    }

    public string AmountFieldTitle
    {
        get => (string)GetValue(AmountFieldTitleProperty);
        set => SetValue(property: AmountFieldTitleProperty, value: value);
    }

    public string Amount
    {
        get => (string)GetValue(AmountProperty);
        set => SetValue(property: AmountProperty, value: value);
    }

    public string EntryPlaceholder
    {
        get => (string)GetValue(EntryPlaceholderProperty);
        set => SetValue(property: EntryPlaceholderProperty, value: value);
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(property: IsReadOnlyProperty, value: value);
    }

    private void AmountFieldGotFocus(object sender, FocusEventArgs e)
    {
        AmountField.CursorPosition = 0;
        AmountField.SelectionLength = AmountField.Text != null ? AmountField.Text.Length : 0;
    }
}
