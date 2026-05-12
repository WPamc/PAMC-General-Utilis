using System.Media;

namespace Toastr.Winforms;

internal class PlaySounds
{
    public static void Success()
    {
        SoundPlayer player =
            new SoundPlayer(ToastResources.OpenSuccessSound());
        player.Play();
    }

    public static void Error()
    {
        SoundPlayer player =
            new SoundPlayer(ToastResources.OpenErrorSound());
        player.Play();
    }

    public static void Warning()
    {
        SoundPlayer player =
            new SoundPlayer(ToastResources.OpenWarningSound());
        player.Play();
    }
}
