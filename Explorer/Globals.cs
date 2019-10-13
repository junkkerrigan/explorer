using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Explorer
{
    public static class Globals
    {
        public const int MainWrapperMargin = 15;

        public const int MainWrapperPadding = 10;

        public const int SpaceBetweenViews = 20;

        public const int ViewItemHeight = 30;

        public const int ViewPadding = 20;

        public static readonly Font ViewItemFont = new Font("Verdana", 12);

        public static readonly Size FileSystemNodeImageSize = new Size(18, 18);
    }
}
