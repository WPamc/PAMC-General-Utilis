using System.Reflection;

namespace Toastr.Winforms;

internal static class ToastResources
{
    private const string ResourcePrefix = "Toastr.Winforms.Resources.";

    public static Bitmap ErrorIcon => LoadBitmap("error_icon.png");

    public static Bitmap SuccessIcon => LoadBitmap("success_icon.png");

    public static Bitmap WarningIcon => LoadBitmap("warning_icon.png");

    public static Bitmap CloseIcon => LoadBitmap("x_icon_light.png");

    public static Stream OpenErrorSound() => OpenResource("error_sound.wav");

    public static Stream OpenSuccessSound() => OpenResource("success_sound.wav");

    public static Stream OpenWarningSound() => OpenResource("warning_sound.wav");

    private static Bitmap LoadBitmap(string name)
    {
        using Stream stream = OpenResource(name);
        return new Bitmap(stream);
    }

    private static Stream OpenResource(string name)
    {
        Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourcePrefix + name);
        if (stream == null)
        {
            throw new InvalidOperationException("Embedded resource not found: " + name);
        }

        return stream;
    }
}
