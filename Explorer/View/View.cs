using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Explorer.Presenter;

namespace Explorer.View
{
    public class FileSystemTree : TreeView
    {
        public FileSystemTree() : base()
        {
            this.ItemHeight = 30;
            this.ShowPlusMinus = true;

            ImageList nodeIcons = new ImageList();
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));
            this.ImageList = nodeIcons;
            this.ImageList.ImageSize = new Size(18, 18);
        }
    }

    public enum FileType
    {
        Drive,
        Folder,
        File
    }
    
    public class FileSystemNode : TreeNode
    {
        public string Path { get; set; }

        public bool IsAccessible { get; set; }

        public bool IsFilled { get; set; }

        private static FileSystemNode _buffer = null;

        public FileSystemNode() : base() { }

        public FileSystemNode(string name) : base(name)
        {
            this.IsAccessible = true;
            this.IsFilled = false;
            this.NodeFont = new Font("Verdana", 12);

            ToolStripMenuItem copy = new ToolStripMenuItem("Copy");
            ToolStripMenuItem paste = new ToolStripMenuItem("Paste");
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add(copy);
            menu.Items.Add(paste);

            this.ContextMenuStrip = menu;
            copy.Click += CopyNode;
            paste.Click += PasteNode;
        }

        void CopyNode(object sender, EventArgs e)
        {
            _buffer = this.Clone() as FileSystemNode;
        }

        void PasteNode(object sender, EventArgs e)
        {
            // add files handling
            if (_buffer == null)
            {
                this.Nodes.Add(_buffer);
            }
            else
            {
                FileSystemNode nodeClone = _buffer.Clone() as FileSystemNode;
                this.Nodes.Add(nodeClone);
            }
        }
    }

    public class DriveNode : FileSystemNode
    {
        private static readonly FileType Type = FileType.Drive;

        public DriveNode() : base() { }

        public DriveNode(string name) : base(name)
        {
            this.ImageIndex = this.SelectedImageIndex = (int)Type;

        }
    }

    public class FolderNode : FileSystemNode
    {
        private static readonly FileType Type = FileType.Folder;

        public FolderNode() : base() { }

        public FolderNode(string name) : base(name)
        {
            this.ImageIndex = this.SelectedImageIndex = (int)Type;
        }
    }

    public class FileNode : FileSystemNode
    {
        private static readonly FileType Type = FileType.File;

        public FileNode() : base() { }

        public FileNode(string name) : base(name)
        {
            this.ImageIndex = this.SelectedImageIndex = (int)Type;
        }
    }

    public partial class Explorer : Form, IFileSystemView
    {
        // TODO: Improve UI
        //       -- modal form appearance and location
        //       -- views' size and location
        //       -- view wrapper's border

        // TODO: Refactor code

        // TODO: Add inaccessibility handling

        private readonly IPresenter _presenter;
        
        private readonly FileSystemTree FolderView;

        public Explorer()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(236, 233, 216);
            this.Paint += Explorer_Paint;

            FolderViewWrapper.Padding = new Padding(20, 15, 20, 15);
            FolderViewWrapper.BorderStyle = BorderStyle.FixedSingle;
            FolderViewWrapper.BackColor = Color.White;

            FolderView = new FileSystemTree()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
            };
            FolderViewWrapper.Controls.Add(FolderView);

            FolderView.BeforeExpand += PreloadContent;
            FolderView.NodeMouseDoubleClick += CheckAccessibility;

            _presenter = new Presenter.Presenter(this);
            _presenter.LoadDrives();
        }

        public void Mount()
        {
            Application.Run(this);
        }

        private void PreloadContent(object sender, TreeViewCancelEventArgs e)
        {
            FolderView.BeginUpdate();
            _presenter.LoadSubdirs(e.Node as FileSystemNode);
            FolderView.EndUpdate();
        }

        public void ShowModal()
        {
            
        }

        void CheckAccessibility(object sender, TreeNodeMouseClickEventArgs e)
        {
            _presenter.CheckAccessibility(e.Node as FileSystemNode);
        }

        public void MountDrives(List<DriveNode> drives)
        {
            foreach (DriveNode d in drives)
            {
                FolderView.Nodes.Add(d);
            }
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