using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Explorer.Presenter;

namespace Explorer.View
{
    /// <summary>
    /// Displays hierarchical collection of file system elements, each 
    /// represented by <see cref="FileSystemNode"/>.
    /// </summary>
    public class FileSystemTree : TreeView
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemTree"/>.
        /// </summary>
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

    /// <summary>
    /// Specifies a type of file system element's icon.
    /// </summary>
    public enum IconType
    {
        Drive,
        Folder,
        File
    }
    
    /// <summary>
    /// Represents a node of <see cref="FileSystemTree"/>.
    /// </summary>
    public abstract class FileSystemNode : TreeNode
    {
        /// <summary>
        /// Absolute path to element.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Indicates if it's possible to interact with element.
        /// </summary>
        public bool Accessible { get; set; }
        
        /// <summary>
        /// Indicates if subnodes are uploaded into node.
        /// </summary>
        public bool Filled { get; set; }

        private static FileSystemNode _buffer = null;

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemNode"/> class.
        /// </summary>
        public FileSystemNode() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemNode"/> class
        /// with specified name.
        /// </summary>
        public FileSystemNode(string name) : base(name)
        {
            this.Accessible = true;
            this.Filled = false;
            this.NodeFont = new Font("Verdana", 12);
            this.ContextMenuStrip = new ContextMenuStrip();

            AddContextMenuItem("Copy", CopyToBuffer);
            AddContextMenuItem("Paste", PasteFromBuffer);

            void CopyToBuffer(object sender, EventArgs e)
            {
                _buffer = this.Clone() as FileSystemNode;
            }

            void PasteFromBuffer(object sender, EventArgs e)
            {
                FileSystemNode nodeClone = _buffer.Clone() as FileSystemNode;
                this.Nodes.Add(nodeClone);
            }
        }

        /// <summary>
        /// Adds new option to right-click menu.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="onClick"></param>
        protected void AddContextMenuItem(string name, EventHandler onClick)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(name);
            item.Click += onClick;
            this.ContextMenuStrip.Items.Add(item);
        }

        /// <summary>
        /// Adds styles that indicate the node is inaccessible.
        /// </summary>
        public void MarkAsInaccessible()
        {
            this.Accessible = false;
            this.ForeColor = Color.Gray;
        }
    }

    /// <summary>
    /// Respresents a node of a system drive in <see cref="FileSystemTree"/>.
    /// </summary>
    public class DriveNode : FileSystemNode
    {
        public DriveNode() : base() { }

        public DriveNode(string name) : base(name)
        {
            this.ImageIndex = this.SelectedImageIndex = (int)IconType.Drive;
        }
    }

    /// <summary>
    /// Respresents a node of a folder in <see cref="FileSystemTree"/>.
    /// </summary>
    public class FolderNode : FileSystemNode
    {
        public FolderNode() : base() { }

        public FolderNode(string name) : base(name)
        {
            this.ImageIndex = this.SelectedImageIndex = (int)IconType.Folder;
        }
    }

    /// <summary>
    /// Respresents a node of a file in <see cref="FileSystemTree"/>.
    /// </summary>
    public class FileNode : FileSystemNode
    {
        public FileNode() : base() { }

        public FileNode(string name) : base(name)
        {
            this.ImageIndex = this.SelectedImageIndex = (int)IconType.File;
        }
    }

    /// <summary>
    /// Displays a file manager.
    /// </summary>
    public partial class Explorer : Form, IFileSystemView
    {
        // TODO: Improve UI
        //       -- modal form appearance and location
        //       -- views' size and location
        //       -- view wrapper's border

        // TODO: Refactor code

        // TODO: Add inaccessibility handling

        private readonly IPresenter _presenter;
        
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

            DirectoryView = new FileSystemTree()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
            };
            DirectoryViewWrapper.Controls.Add(DirectoryView);

            DirectoryView.BeforeExpand += PreloadContent;
            //DirectoryView.NodeMouseDoubleClick += CheckAccessibility;

            _presenter = new Presenter.Presenter(this);
            _presenter.LoadDrives();
        }

        /// <summary>
        /// Runs an application.
        /// </summary>
        public void Mount()
        {
            Application.Run(this);
        }

        private void PreloadContent(object sender, TreeViewCancelEventArgs e)
        {
            //DirectoryView.BeginUpdate();
            _presenter.LoadSubDirectories(e.Node as FileSystemNode);
            //DirectoryView.EndUpdate();
        }

        //public void ShowModal()
        //{
            
        //}

        //private void CheckAccessibility(object sender, TreeNodeMouseClickEventArgs e)
        //{
        //    _presenter.CheckAccessibility(e.Node as FileSystemNode);
        //}

        public void MountDrives(List<DriveNode> drives)
        {
            foreach (DriveNode d in drives)
            {
                DirectoryView.Nodes.Add(d);
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