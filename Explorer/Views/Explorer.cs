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

        public readonly Padding ViewPadding = new Padding(20);

        private readonly Panel MainWrapper;

        private readonly Panel DirectoryViewWrapper;

        private readonly Panel FileViewWrapper;

        private readonly FileSystemTree DirectoryView;

        private readonly FileSystemList FileView;

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
                Location = new Point(Globals.MainWrapperMargin, 
                    Globals.MainWrapperMargin),
                Size = new Size(this.ClientSize.Width - 2 * Globals.MainWrapperMargin, 
                    this.ClientSize.Height - 2 * Globals.MainWrapperMargin),
            };

            DirectoryViewWrapper = new Panel()
            {
                Location = new Point(Globals.MainWrapperPadding, 
                    Globals.MainWrapperPadding),
                Size = new Size(
                    (MainWrapper.Width - 2 * Globals.MainWrapperPadding - 
                    Globals.SpaceBetweenViews) / 2, 
                    MainWrapper.Height - 2 * Globals.MainWrapperPadding),
                Padding = ViewPadding,
                BackColor = Color.White,
            };

            FileViewWrapper = new Panel()
            {
                Location = new Point(
                    DirectoryViewWrapper.Width + Globals.MainWrapperPadding + 
                    Globals.SpaceBetweenViews, 
                    Globals.MainWrapperPadding),
                Size = new Size(DirectoryViewWrapper.Width, 
                    DirectoryViewWrapper.Height),
                Padding = ViewPadding,
                BackColor = Color.White,
            };


            FileView = new FileSystemList();
            
            DirectoryView = new FileSystemTree(FileView);

            this.Controls.Add(MainWrapper);

            MainWrapper.Controls.Add(DirectoryViewWrapper);
            MainWrapper.Controls.Add(FileViewWrapper);

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
                    2 * Globals.MainWrapperMargin,
                this.ClientSize.Height - 2 * Globals.MainWrapperMargin);

            DirectoryViewWrapper.Size = new Size(
                (MainWrapper.Width - 2 * Globals.MainWrapperPadding - 
                    Globals.SpaceBetweenViews) / 2,
                MainWrapper.Height - 2 * Globals.MainWrapperPadding);

            FileViewWrapper.Location = new Point(
                DirectoryViewWrapper.Width + Globals.MainWrapperPadding +
                    Globals.SpaceBetweenViews,
                Globals.MainWrapperPadding);
            FileViewWrapper.Size = new Size(DirectoryViewWrapper.Width, 
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