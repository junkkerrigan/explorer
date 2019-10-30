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

        private readonly MainMenu MainMenu;

        private readonly BottomButtonPanel BottomButtonPanel;

        private readonly SearchBox SearchBox;

        private readonly Panel DirectoryViewWrapper;

        private readonly Panel FileViewWrapper;

        private readonly FileSystemTree DirectoryView;

        private readonly FileSystemList FileView;

        private readonly CheckBox SearchOnly;

        /// <summary>
        /// Initializes a new instance of <see cref="Explorer"/>.
        /// </summary>
        public Explorer()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(236, 233, 216);

            // to key pressing
            this.KeyPreview = true;

            MainWrapper = new Panel()
            {
                Location = new Point(Constants.MainWrapperMargin, 
                    Constants.MainWrapperMargin),
                Size = new Size(this.ClientSize.Width - 2 * Constants.MainWrapperMargin, 
                    this.ClientSize.Height - 2 * Constants.MainWrapperMargin),
            };

            FileView = new FileSystemList();

            DirectoryView = new FileSystemTree(FileView);

            MainMenu = new MainMenu(FileView);

            SearchOnly = new CheckBox()
            {
                Size = new Size(175, 20),
                Text = "Search for HTML files only",
            };

            SearchOnly.AutoSize = false;

            SearchOnly.CheckedChanged += (s, e) =>
            {
                FileView.UpdateRefresh();
            };

            SearchBox = new SearchBox(SearchOnly, FileView)
            {
                Location = new Point(MainWrapper.Width - Constants.MainWrapperPadding
                    - Constants.SearchBoxWidth - 30, 5),
                Size = new Size(Constants.SearchBoxWidth, 1),
                BorderStyle = BorderStyle.FixedSingle,
            };

            SearchOnly.Location = new Point(SearchBox.Left - 175, SearchBox.Top + 5);

            BottomButtonPanel = new BottomButtonPanel(FileView)
            {
                Location = new Point(Constants.MainWrapperPadding,
                    MainWrapper.Height - Constants.MainWrapperPadding
                    - Constants.BottomButtonsHeight),

                Size = new Size(MainWrapper.Width - 2 * Constants.MainWrapperPadding,
                    Constants.BottomButtonsHeight)
            };

            DirectoryViewWrapper = new Panel()
            {
                Location = new Point(Constants.MainWrapperPadding, 
                    MainMenu.Height + Constants.MainMenuMarginBottom),
                Size = new Size(MainWrapperFreeSpaceWidth() / 3, 
                    MainWrapperFreeSpaceHeight()),
                Padding = new Padding(Constants.ViewPadding),
                BackColor = Color.White,
            };

            FileViewWrapper = new Panel()
            {
                Location = new Point(Constants.MainWrapperPadding 
                    + DirectoryViewWrapper.Width + Constants.SpaceBetweenViews,
                    MainMenu.Height + Constants.MainMenuMarginBottom),
                Size = new Size(MainWrapperFreeSpaceWidth() - DirectoryViewWrapper.Width ,
                    MainWrapperFreeSpaceHeight()),
                Padding = new Padding(Constants.ViewPadding),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
            };

            this.Controls.Add(MainWrapper);

            MainWrapper.Controls.Add(MainMenu);
            MainWrapper.Controls.Add(DirectoryViewWrapper);
            MainWrapper.Controls.Add(FileViewWrapper);
            MainWrapper.Controls.Add(BottomButtonPanel);
            MainWrapper.Controls.Add(SearchBox);
            MainWrapper.Controls.Add(SearchOnly);
            SearchBox.BringToFront();
            SearchOnly.BringToFront();

            DirectoryViewWrapper.Controls.Add(DirectoryView);
            
            FileViewWrapper.Controls.Add(FileView);

            this.Paint += Explorer_Paint;
            this.SizeChanged += Explorer_SizeChanged;
        }

        public void Mount()
        {
            Application.Run(this);
        }

        private void Explorer_SizeChanged(object sender, EventArgs e)
        {
            MainWrapper.Size = new Size(this.ClientSize.Width  
                    - 2 * Constants.MainWrapperMargin,
                this.ClientSize.Height - 2 * Constants.MainWrapperMargin);

            DirectoryViewWrapper.Size = new Size(
                MainWrapperFreeSpaceWidth() / 3,
                MainWrapperFreeSpaceHeight() + 8);

            FileViewWrapper.Location = new Point(
                DirectoryViewWrapper.Width + Constants.MainWrapperPadding +
                    Constants.SpaceBetweenViews,
                MainMenu.Height);

            FileViewWrapper.Size = new Size(MainWrapperFreeSpaceWidth() 
                - DirectoryViewWrapper.Width,
                MainWrapperFreeSpaceHeight());

            BottomButtonPanel.Location = new Point(Constants.MainWrapperPadding,
                MainWrapper.Height - Constants.MainWrapperPadding
                - Constants.BottomButtonsHeight);

            BottomButtonPanel.Size = new Size(
                MainWrapper.Width - 2 * Constants.MainWrapperPadding,
                Constants.BottomButtonsHeight);
        }

        private void Explorer_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.White, 4);
            Rectangle border = new Rectangle(MainWrapper.Location, MainWrapper.Size);
            e.Graphics.DrawRectangle(p, border);
        }

        private int MainWrapperFreeSpaceWidth()
        {
            return MainWrapper.Width - 2 * Constants.MainWrapperPadding
                - Constants.SpaceBetweenViews;
        }

        private int MainWrapperFreeSpaceHeight()
        {
            return MainWrapper.Height - MainMenu.Height - Constants.MainMenuMarginBottom
                - Constants.BottomButtonsHeight - 2 * Constants.MainWrapperPadding;
        }
    }
}