using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toastr.Winforms
{
    public partial class Toast : Form
    {
        private int _duration;
        private bool _enableSoundEffect;
        private Form _owner;
        private bool _autoCloseStarted;

        private static readonly IntPtr HwndTopMost = new IntPtr(-1);
        private const int SwShowNoActivate = 4;
        private const uint SwpNoActivate = 0x0010;
        private const uint SwpShowWindow = 0x0040;

        public Toast(Form owner = null, ToastrPosition toastrPosition = ToastrPosition.TopRight, int duration = 5000,
                    bool enableSoundEffect = false)
        {
            InitializeComponent();
            _owner = owner;
            _duration = duration;
            _enableSoundEffect = enableSoundEffect;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            ShowInTaskbar = false;
            close.BackgroundImage = Properties.Resources.x_icon_light;

            if (_owner != null)
            {
                CalculateOwnerRelativePosition(toastrPosition);
            }
            else
            {
                CalculateScreenRelativePosition(toastrPosition);
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

        private void CalculateOwnerRelativePosition(ToastrPosition position)
        {
            Rectangle bounds = _owner.Bounds;
            switch (position)
            {
                case ToastrPosition.TopLeft:
                    Location = new Point(bounds.Left + 10, bounds.Top + 10);
                    break;
                case ToastrPosition.TopRight:
                    Location = new Point(bounds.Right - Width - 10, bounds.Top + 10);
                    break;
                case ToastrPosition.TopCenter:
                    Location = new Point(bounds.Left + (bounds.Width / 2) - (Width / 2), bounds.Top + 10);
                    break;
                case ToastrPosition.BottomLeft:
                    Location = new Point(bounds.Left + 10, bounds.Bottom - Height - 10);
                    break;
                case ToastrPosition.BottomRight:
                    Location = new Point(bounds.Right - Width - 10, bounds.Bottom - Height - 10);
                    break;
                case ToastrPosition.BottomCenter:
                    Location = new Point(bounds.Left + (bounds.Width / 2) - (Width / 2), bounds.Bottom - Height - 10);
                    break;
            }
        }

        private void CalculateScreenRelativePosition(ToastrPosition position)
        {
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(Cursor.Position) || scrn.Primary)
                {
                    switch (position)
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
                    if (scrn.Bounds.Contains(Location)) break;
                }
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void ShowSuccess()
        {
            icon.Image = Properties.Resources.success_icon;
            message.Text = "Successfully!";
            BackColor = Color.FromArgb(113, 179, 113);
            ShowToast();
            if (_enableSoundEffect)
            {
                PlaySounds.Success();
            }
        }

        public void ShowSuccess(string msg)
        {
            icon.Image = Properties.Resources.success_icon;
            message.Text = msg;
            BackColor = Color.FromArgb(113, 179, 113);
            ShowToast();
            if (_enableSoundEffect)
            {
                PlaySounds.Success();
            }
        }

        public void ShowError()
        {
            icon.Image = Properties.Resources.error_icon;
            message.Text = "Something went wrong!";
            BackColor = Color.FromArgb(202, 94, 88);
            ShowToast();
            if (_enableSoundEffect)
            {
                PlaySounds.Error();
            }
        }

        public void ShowError(string msg)
        {
            icon.Image = Properties.Resources.error_icon;
            message.Text = msg;
            BackColor = Color.FromArgb(202, 94, 88);
            ShowToast();
            if (_enableSoundEffect)
            {
                PlaySounds.Error();
            }
        }

        public void ShowWarning()
        {
            icon.Image = Properties.Resources.warning_icon;
            message.Text = "Warning!";
            BackColor = Color.FromArgb(247, 167, 53);
            ShowToast();
            if (_enableSoundEffect)
            {
                PlaySounds.Warning();
            }
        }

        public void ShowWarning(string msg)
        {
            icon.Image = Properties.Resources.warning_icon;
            message.Text = msg;
            BackColor = Color.FromArgb(247, 167, 53);
            ShowToast();
            if (_enableSoundEffect)
            {
                PlaySounds.Warning();
            }
        }

        private async void Toastr_Load(object sender, EventArgs e)
        {
            // Note: Designer sets these, but we re-verify here as per original code
            if (icon.Image == null) icon.Image = Properties.Resources.success_icon;
            if (close.BackgroundImage == null) close.BackgroundImage = Properties.Resources.x_icon_light;
            StartAutoCloseTimer();
        }
    }
}
