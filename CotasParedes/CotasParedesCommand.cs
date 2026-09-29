using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;

namespace BimBlaster.CotasParedes;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class CotasParedesCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiDoc = commandData.Application.ActiveUIDocument;
        var doc = uiDoc.Document;
        var view = doc.ActiveView;
        var lang = LanguageResolver.Resolve(commandData.Application.Application.Language);
        string S(string key) => LocalizedStrings.Get(lang, key);

        try
        {
            // Estilos de cota lineares (independe do idioma do Revit)
            var styles = new FilteredElementCollector(doc)
                .OfClass(typeof(DimensionType))
                .Cast<DimensionType>()
                .Where(t => t.StyleType == DimensionStyleType.Linear || t.StyleType == DimensionStyleType.LinearFixed)
                .Select(t => new DimensionTypeItem
                {
                    DisplayName = string.IsNullOrEmpty(t.FamilyName) ? t.Name : $"{t.FamilyName} | {t.Name}",
                    Id = t.Id
                })
                .OrderBy(i => i.DisplayName, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            if (styles.Count == 0)
            {
                TaskDialog.Show(S("Title"), S("NoStyle"));
                return Result.Cancelled;
            }

            var window = new CotasParedesWindow(styles, lang);
            if (window.ShowDialog() != true || window.SelectedStyle == null)
                return Result.Cancelled;

            var dimType = (DimensionType)doc.GetElement(window.SelectedStyle.Id);
            var filter = new WallFilter(doc, window.ParamName, window.ParamValue);

            var walls = uiDoc.Selection
                .PickElementsByRectangle(filter, S("PickPrompt"))
                .OfType<Wall>()
                .ToList();

            if (walls.Count == 0)
            {
                TaskDialog.Show(S("Title"), S("NoWalls"));
                return Result.Cancelled;
            }

            var refArray = new ReferenceArray();
            foreach (var wall in walls)
            {
                try
                {
                    var ext = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
                    var inn = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Interior);
                    if (ext.Count > 0) refArray.Append(ext[0]);
                    if (inn.Count > 0) refArray.Append(inn[0]);
                }
                catch
                {
                    // parede sem faces laterais utilizáveis (ex.: parede cortina) - ignora
                }
            }

            if (refArray.Size < 2)
            {
                TaskDialog.Show(S("Title"), S("NotEnoughRefs"));
                return Result.Failed;
            }

            var curve = walls.Select(w => (w.Location as LocationCurve)?.Curve).FirstOrDefault(c => c != null);
            if (curve == null)
            {
                TaskDialog.Show(S("Title"), S("NoLocation"));
                return Result.Failed;
            }

            var mid = curve.Evaluate(0.5, true);
            var direction = (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();
            var perp = direction.CrossProduct(XYZ.BasisZ).Normalize();
            var extension = perp.Multiply(10);
            var dimLine = Line.CreateBound(mid - extension, mid + extension);

            using var t = new Transaction(doc, S("TransactionName"));
            t.Start();
            try
            {
                doc.Create.NewDimension(view, dimLine, refArray, dimType);
                doc.Regenerate();
                t.Commit();
            }
            catch (Exception ex)
            {
                t.RollBack();
                TaskDialog.Show(S("Title"), string.Format(S("ApiError"), ex.Message));
                return Result.Failed;
            }

            uiDoc.RefreshActiveView();
            TaskDialog.Show(S("Title"), string.Format(S("Success"), walls.Count));
            return Result.Succeeded;
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            return Result.Cancelled;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return Result.Failed;
        }
    }
}
