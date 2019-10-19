using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
     /// <summary>
    /// Displays a file manager.
    /// </summary>
    public partial class Explorer : Form, IExplorer
    {
        // TODO: Improve UI

        private readonly Panel MainWrapper;

        private readonly Panel DirectoryViewWrapper;

        private readonly Panel FileViewWrapper;

        private readonly FileSystemTree DirectoryView;

        private readonly FileSystemList FileView;

        //private readonly BreadCrumbPrevButton Prev;
        
        //private readonly BreadCrumbNextButton Next;

        /// <summary>
        /// Initializes a new instance of <see cref="Explorer"/>.
        /// </summary>
        public Explorer()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(236, 233, 216);
            this.Paint += Explorer_Paint;
            this.SizeChanged += Explorer_SizeChanged;

            MainWrapper = new Panel()
            {
                Location = new Point(Constants.MainWrapperMargin, 
                    Constants.MainWrapperMargin),
                Size = new Size(this.ClientSize.Width - 2 * Constants.MainWrapperMargin, 
                    this.ClientSize.Height - 2 * Constants.MainWrapperMargin),
            };

            DirectoryViewWrapper = new Panel()
            {
                Location = new Point(Constants.MainWrapperPadding, 
                    Constants.MainWrapperPadding),
                Size = new Size(
                    (MainWrapper.Width - 2 * Constants.MainWrapperPadding - 
                    Constants.SpaceBetweenViews) / 3, 
                    MainWrapper.Height - 2 * Constants.MainWrapperPadding),
                Padding = new Padding(Constants.ViewPadding),
                BackColor = Color.White,
            };

            FileViewWrapper = new Panel()
            {
                Location = new Point(
                    DirectoryViewWrapper.Width + Constants.MainWrapperPadding +
                    Constants.SpaceBetweenViews,
                    Constants.MainWrapperPadding),
                Size = new Size(DirectoryViewWrapper.Width * 2,
                    DirectoryViewWrapper.Height),
                Padding = new Padding(Constants.ViewPadding),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
            };

            FileView = new FileSystemList();

            DirectoryView = new FileSystemTree(FileView);

            this.Controls.Add(MainWrapper);

            MainWrapper.Controls.Add(DirectoryViewWrapper);
            MainWrapper.Controls.Add(FileViewWrapper);
            FileViewWrapper.BringToFront();

            DirectoryViewWrapper.Controls.Add(DirectoryView);
            
            FileViewWrapper.Controls.Add(FileView);
        }

        public void Mount()
        {
            Application.Run(this);
        }

        private void Explorer_SizeChanged(object sender, EventArgs e)
        {
            MainWrapper.Size = new Size(this.ClientSize.Width - 
                    2 * Constants.MainWrapperMargin,
                this.ClientSize.Height - 2 * Constants.MainWrapperMargin);

            DirectoryViewWrapper.Size = new Size(
                (MainWrapper.Width - 2 * Constants.MainWrapperPadding - 
                    Constants.SpaceBetweenViews) / 3,
                MainWrapper.Height - 2 * Constants.MainWrapperPadding);

            FileViewWrapper.Location = new Point(
                DirectoryViewWrapper.Width + Constants.MainWrapperPadding +
                    Constants.SpaceBetweenViews,
                Constants.MainWrapperPadding);
            FileViewWrapper.Size = new Size(DirectoryViewWrapper.Width * 2, 
                DirectoryViewWrapper.Height);
        }

        private void Explorer_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.White, 4);
            Rectangle border = new Rectangle(MainWrapper.Location, MainWrapper.Size);
            e.Graphics.DrawRectangle(p, border);
        }
    }
}