using System.Windows;

namespace BimBlaster.CotasParedes;

public class DimensionTypeItem
{
    public required string DisplayName { get; init; }
    public required Autodesk.Revit.DB.ElementId Id { get; init; }
}

public partial class CotasParedesWindow : Window
{
    public DimensionTypeItem? SelectedStyle { get; private set; }
    public string ParamName { get; private set; } = "";
    public string ParamValue { get; private set; } = "";

    public CotasParedesWindow(IList<DimensionTypeItem> styles, AppLanguage language)
    {
        Resources.MergedDictionaries.Add(StyleLoader.LoadBimBlasterStyles());
        InitializeComponent();

        string S(string key) => LocalizedStrings.Get(language, key);

        Title = S("WindowTitle");
        TitleText.Text = S("Title");
        DimStyleLabel.Text = S("DimStyleLabel");
        FilterSectionLabel.Text = S("FilterSection");
        ParamNameLabel.Text = S("ParamNameLabel");
        ParamValueLabel.Text = S("ParamValueLabel");
        CancelButton.Content = S("Cancel");
        RunButton.Content = S("Run");

        StyleCombo.ItemsSource = styles;
        if (styles.Count > 0) StyleCombo.SelectedIndex = 0;

        ParamNameBox.Text = "TJRS_Filtro";
        ParamValueBox.Text = "REV";
    }

    private void Run_Click(object sender, RoutedEventArgs e)
    {
        SelectedStyle = StyleCombo.SelectedItem as DimensionTypeItem;
        ParamName = ParamNameBox.Text.Trim();
        ParamValue = ParamValueBox.Text.Trim();
        DialogResult = SelectedStyle != null;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
