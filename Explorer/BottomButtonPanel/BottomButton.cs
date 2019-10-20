using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    public class BottomButton : Button
    {
        public BottomButton(string text) : base()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Text = text;
            this.Height = Constants.BottomButtonsHeight;
            this.Width = 150;
            this.Font = new Font("Verdana", 11);
            this.Margin = new Padding(5, 0, 5, 0);
        }
    }
}
