namespace Toastr.Winforms;

using System.Runtime.InteropServices;

public partial class Toast : Form
{
    private int _duration;
    private bool _enableSoundEffect;
    private bool _autoCloseStarted;

    private static readonly IntPtr HwndTopMost = new(-1);
    private const int SwShowNoActivate = 4;
    private const uint SwpNoActivate = 0x0010;
    private const uint SwpShowWindow = 0x0040;

    public Toast(ToastrPosition toastrPosition = ToastrPosition.TopRight, int duration = 3000,
                bool enableSoundEffect = false)
    {
        InitializeComponent();
        _duration = duration;
        _enableSoundEffect = enableSoundEffect;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        close.BackgroundImage = ToastResources.CloseIcon;
        foreach (var scrn in Screen.AllScreens)
        {
            if (scrn.Bounds.Contains(Location))
            {
                switch (toastrPosition)
                {
                    case ToastrPosition.TopLeft:
                        Location = new Point(scrn.WorkingArea.Left + 10, scrn.WorkingArea.Top + 10);
                        break;
                    case ToastrPosition.TopRight:
                        Location = new Point(scrn.WorkingArea.Right - Width - 10, scrn.WorkingArea.Top + 10);
                        break;
                    case ToastrPosition.TopCenter:
                        Location = new Point(scrn.WorkingArea.Left + (scrn.WorkingArea.Width / 2) - (Width / 2), scrn.WorkingArea.Top + 10);
                        break;
                    case ToastrPosition.BottomLeft:
                        Location = new Point(scrn.WorkingArea.Left + 10, scrn.WorkingArea.Bottom - Height - 10);
                        break;
                    case ToastrPosition.BottomRight:
                        Location = new Point(scrn.WorkingArea.Right - Width - 10, scrn.WorkingArea.Bottom - Height - 10);
                        break;
                    case ToastrPosition.BottomCenter:
                        Location = new Point(scrn.WorkingArea.Left + (scrn.WorkingArea.Width / 2) - (Width / 2), scrn.WorkingArea.Bottom - Height - 10);
                        break;
                }
                return;
            }
        }
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams baseParams = base.CreateParams;
            baseParams.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
            return baseParams;
        }
    }

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    private void ShowToast()
    {
        if (IsDisposed)
        {
            return;
        }

        if (!IsHandleCreated)
        {
            CreateHandle();
        }

        ShowWindow(Handle, SwShowNoActivate);
        SetWindowPos(Handle, HwndTopMost, Left, Top, Width, Height, SwpNoActivate | SwpShowWindow);
        StartAutoCloseTimer();
    }

    private async void StartAutoCloseTimer()
    {
        if (_autoCloseStarted)
        {
            return;
        }

        _autoCloseStarted = true;
        await Task.Delay(_duration);

        if (!IsDisposed)
        {
            Close();
        }
    }

    /// <summary>
    /// close toastr
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void close_Click(object sender, EventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Show success
    /// </summary>
    public void ShowSuccess()
    {
        icon.Image = ToastResources.SuccessIcon;
        message.Text = "Successfully!";
        BackColor = Color.FromArgb(113, 179, 113);
        ShowToast();
        if (_enableSoundEffect)
        {
            PlaySounds.Success();
        }
    }

    /// <summary>
    /// Show success with custom message
    /// </summary>
    /// <param name="msg"></param>
    public void ShowSuccess(string msg)
    {
        icon.Image = ToastResources.SuccessIcon;
        message.Text = msg;
        BackColor = Color.FromArgb(113, 179, 113);
        ShowToast();
        if (_enableSoundEffect)
        {
            PlaySounds.Success();
        }
    }

    /// <summary>
    /// Show error
    /// </summary>
    public void ShowError()
    {
        icon.Image = ToastResources.ErrorIcon;
        message.Text = "Something went wrong!";
        BackColor = Color.FromArgb(202, 94, 88);
        ShowToast();
        if (_enableSoundEffect)
        {
            PlaySounds.Error();
        }
    }

    /// <summary>
    /// Show error with custom message
    /// </summary>
    public void ShowError(string msg)
    {
        icon.Image = ToastResources.ErrorIcon;
        message.Text = msg;
        BackColor = Color.FromArgb(202, 94, 88);
        ShowToast();
        if (_enableSoundEffect)
        {
            PlaySounds.Error();
        }
    }

    /// <summary>
    /// Show warning
    /// </summary>
    /// <param name="msg"></param>
    public void ShowWarning()
    {
        icon.Image = ToastResources.WarningIcon;
        message.Text = "Warning!";
        BackColor = Color.FromArgb(247, 167, 53);
        ShowToast();
        if (_enableSoundEffect)
        {
            PlaySounds.Warning();
        }
    }

    /// <summary>
    /// Show warning with custom message
    /// </summary>
    /// <param name="msg"></param>
    public void ShowWarning(string msg)
    {
        icon.Image = ToastResources.WarningIcon;
        message.Text = msg;
        BackColor = Color.FromArgb(247, 167, 53);
        ShowToast();
        if (_enableSoundEffect)
        {
            PlaySounds.Warning();
        }
    }

    /// <summary>
    /// load event - close after timeout seconds
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Toastr_Load(object sender, EventArgs e)
    {
        if (icon.Image == null) icon.Image = ToastResources.SuccessIcon;
        if (close.BackgroundImage == null) close.BackgroundImage = ToastResources.CloseIcon;
        StartAutoCloseTimer();
    }
}
