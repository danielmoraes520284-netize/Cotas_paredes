using System.Globalization;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace BimBlaster.CotasParedes;

/// <summary>Permite só paredes e bloqueia as que tenham o parâmetro (tipo ou instância) igual ao valor informado.</summary>
public class WallFilter : ISelectionFilter
{
    private readonly Document _doc;
    private readonly string _paramName;
    private readonly string _paramValue;

    public WallFilter(Document doc, string paramName, string paramValue)
    {
        _doc = doc;
        _paramName = paramName;
        _paramValue = paramValue;
    }

    public bool AllowElement(Element elem)
    {
        try
        {
            if (elem is not Wall) return false;
            if (string.IsNullOrWhiteSpace(_paramName)) return true;

            if (_doc.GetElement(elem.GetTypeId()) is Element wallType && Matches(wallType)) return false;
            if (Matches(elem)) return false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool AllowReference(Reference reference, XYZ position) => false;

    private bool Matches(Element e)
    {
        var p = e.LookupParameter(_paramName);
        if (p == null || !p.HasValue) return false;

        string? val = p.StorageType == StorageType.String ? p.AsString() : p.AsValueString();
        if (val == null)
        {
            val = p.StorageType switch
            {
                StorageType.Integer => p.AsInteger().ToString(),
                StorageType.Double => p.AsDouble().ToString(CultureInfo.InvariantCulture),
                _ => null
            };
        }

        return val != null && string.Equals(val.Trim(), _paramValue.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
