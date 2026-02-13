using System.Windows.Forms;

namespace ИТД_Desktop.UI.Standart
{
    public partial class ImageViewerStandartUI : Form
    {
        public ImageViewerStandartUI(System.Drawing.Bitmap bitmap)
        {
            InitializeComponent();
            pictureBox1.BackgroundImage = bitmap;
        }
    }
}
