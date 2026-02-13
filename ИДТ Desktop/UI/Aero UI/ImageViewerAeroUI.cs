using Krypton.Toolkit;

namespace ИТД_Desktop.UI.Aero_UI
{
    public partial class ImageViewerAeroUI : KryptonForm
    {
        public ImageViewerAeroUI(System.Drawing.Bitmap bitmap)
        {
            InitializeComponent();
            ThemeHelper.RegisterForm(this, kryptonManager1);
            kryptonPictureBox1.Image = bitmap;
        }
    }
}