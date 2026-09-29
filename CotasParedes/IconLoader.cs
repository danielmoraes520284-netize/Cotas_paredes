using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace BimBlaster.CotasParedes;

public static class IconLoader
{
    public static BitmapImage Load(string relativePath)
    {
        var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var uri = new Uri(Path.Combine(baseDir, relativePath));
        return new BitmapImage(uri);
    }
}
