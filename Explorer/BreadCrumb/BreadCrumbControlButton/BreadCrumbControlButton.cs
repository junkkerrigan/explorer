using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Explorer
{
    public class BreadCrumbControlButton : Button
    {
        public BreadCrumbControlButton() : base()
        {
            // TODO: replace magic numbers with Globals constants
            this.Size = new Size(35, 35);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
        }
    }
}
