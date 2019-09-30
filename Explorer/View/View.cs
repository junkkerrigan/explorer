using System;
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
        private readonly ImageList _nodeIcons = new ImageList();

        public FileSystemTree() : base()
        {
            this.ItemHeight = 30;
            this.ShowPlusMinus = true;

            FillNodeIconsList();
            this.ImageList = _nodeIcons;
            this.ImageList.ImageSize = new Size(18, 18);
        }

        private void FillNodeIconsList()
        {
            _nodeIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            _nodeIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            _nodeIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));
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
        public FileSystemNode(string name) : base(name)
        {
            this.NodeFont = new Font("Verdana", 12);
        }
    }

    public class DriveNode : FileSystemNode
    {
        private static readonly FileType Type = FileType.Drive;
        public DriveNode(string name) : base(name)
        {
            this.ImageIndex = (int)Type;
        }
    }

    public class FolderNode : FileSystemNode
    {
        private static readonly FileType Type = FileType.Folder;
        public FolderNode(string name) : base(name)
        {
            this.ImageIndex = (int)Type;
        }
    }

    public class FileNode : FileSystemNode
    {
        private static readonly FileType Type = FileType.File;
        public FileNode(string name) : base(name)
        {
            this.ImageIndex = (int)Type;
        }
    }

    public partial class Explorer : Form, IFileSystemView
    {
        private readonly Presenter.IPresenter _presenter;

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

            FolderView.BeforeExpand += FolderView_BeforeExpand;
            FolderView.BeforeSelect += FolderView_BeforeExpand;

            _presenter = new Presenter.Presenter(this);
            _presenter.LoadDrives();
        }

        public void Mount()
        {
            Application.Run(this);
        }

        private void FolderView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

        }

        public void MountDrives(List<DriveNode> drives)
        {
            foreach (var d in drives)
            {
                FolderView.Nodes.Add(d);
            }
        }

        private void Explorer_Paint(object sender, PaintEventArgs e)
        {
            MainWrapper.BorderStyle = BorderStyle.None;
            Pen p = new Pen(Color.White, 2);
            var border = new Rectangle(MainWrapper.Location, MainWrapper.Size);
            e.Graphics.DrawRectangle(p, border);
        }
    }
}