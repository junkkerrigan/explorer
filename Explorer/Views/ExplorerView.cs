using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Explorer.Presenters;

namespace Explorer.Views
{
    /// <summary>
    /// Displays a file manager.
    /// </summary>
    public partial class Explorer : Form, IExplorerView
    {
        // TODO: Improve UI
        //       -- modal form appearance and location
        //       -- views' size and location
        //       -- view wrapper's border

        // TODO: Refactor code

        // TODO: Add inaccessibility handling

        private readonly FileSystemTree DirectoryView;

        /// <summary>
        /// Initializes a new instance of <see cref="Explorer"/>.
        /// </summary>
        public Explorer()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(236, 233, 216);
            this.Paint += Explorer_Paint;

            DirectoryViewWrapper.Padding = new Padding(20, 15, 20, 15);
            DirectoryViewWrapper.BorderStyle = BorderStyle.FixedSingle;
            DirectoryViewWrapper.BackColor = Color.White;

            DirectoryView = new FileSystemTree();
            DirectoryViewWrapper.Controls.Add(DirectoryView);
        }

        public void Mount()
        {
            Application.Run(this);
        }

        private void Explorer_Paint(object sender, PaintEventArgs e)
        {
            MainWrapper.BorderStyle = BorderStyle.None;
            Pen p = new Pen(Color.White, 2);
            Rectangle border = new Rectangle(MainWrapper.Location, MainWrapper.Size);
            e.Graphics.DrawRectangle(p, border);
        }
    }
}