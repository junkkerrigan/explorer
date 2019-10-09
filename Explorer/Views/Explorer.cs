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
    public partial class Explorer : Form, IExplorer
    {
        // TODO: Improve UI

        public const int MainWrapperMargin = 15;
        public const int MainWrapperPadding = 15;
        public const int SpaceBetweenViews = 20;

        public readonly Padding ViewPadding = new Padding(20);

        private readonly Panel MainWrapper;

        private readonly Panel DirectoryViewWrapper;

        private readonly Panel FileViewWrapper;

        private readonly FileSystemTree DirectoryView;

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
                Location = new Point(MainWrapperMargin, MainWrapperMargin),
                Size = new Size(this.ClientSize.Width - 2 * MainWrapperMargin, 
                    this.ClientSize.Height - 2 * MainWrapperMargin),
            };

            DirectoryViewWrapper = new Panel()
            {
                Location = new Point(MainWrapperPadding, MainWrapperPadding),
                Size = new Size(
                    (MainWrapper.Width - 2 * MainWrapperPadding - SpaceBetweenViews) / 2, 
                    MainWrapper.Height - 2 * MainWrapperPadding),
                Padding = ViewPadding,
                BackColor = Color.White,
            };

            FileViewWrapper = new Panel()
            {
                Location = new Point(
                    DirectoryViewWrapper.Width + MainWrapperPadding + SpaceBetweenViews, 
                    MainWrapperPadding),
                Size = new Size(DirectoryViewWrapper.Width, DirectoryViewWrapper.Height),
                Padding = ViewPadding,
                BackColor = Color.White,
            };


            DirectoryView = new FileSystemTree();

            this.Controls.Add(MainWrapper);

            MainWrapper.Controls.Add(DirectoryViewWrapper);
            MainWrapper.Controls.Add(FileViewWrapper);

            DirectoryViewWrapper.Controls.Add(DirectoryView);
        }

        public void Mount()
        {
            Application.Run(this);
        }

        private void Explorer_SizeChanged(object sender, EventArgs e)
        {
            MainWrapper.Size = new Size(this.ClientSize.Width - 2 * MainWrapperMargin,
                    this.ClientSize.Height - 2 * MainWrapperMargin);

            DirectoryViewWrapper.Size = new Size(
                    (MainWrapper.Width - 2 * MainWrapperPadding - SpaceBetweenViews) / 2,
                    MainWrapper.Height - 2 * MainWrapperPadding);

            FileViewWrapper.Location = new Point(
                    DirectoryViewWrapper.Width + MainWrapperPadding + SpaceBetweenViews,
                    MainWrapperPadding);
            FileViewWrapper.Size = new Size(DirectoryViewWrapper.Width, DirectoryViewWrapper.Height);
        }

        private void Explorer_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.White, 4);
            Rectangle border = new Rectangle(MainWrapper.Location, MainWrapper.Size);
            e.Graphics.DrawRectangle(p, border);
        }
    }
}