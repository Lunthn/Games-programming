using System.Drawing;
using System.Windows.Forms;

namespace SteeringCS
{
    class Double_buffered_Panel : Panel
    {
        public Double_buffered_Panel()
        {
            this.DoubleBuffered = true;
        }

        public void Set_background_image(string filename)
        { 
            this.BackgroundImage = Image.FromFile(filename);
        }

    }
}

