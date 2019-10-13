using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public class BreadCrumbPrevButton : BreadCrumbControlButton
    {
        public BreadCrumbPrevButton() : base()
        {
            this.BackgroundImage = Image.FromFile("../../assets/icons/prevIcon.png");
        }
    }
}
