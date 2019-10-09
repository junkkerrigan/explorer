using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using Explorer.Presenters;

namespace Explorer.Views
{
    /// <summary>
    /// Displays hierarchical collection of file system elements, each 
    /// represented by <see cref="FileSystemNode"/>.
    /// </summary>
    public class FileSystemTree : TreeView, IFileSystemTree
    {
        private readonly IFileSystemTreePresenter _presenter;

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemTree"/>.
        /// </summary>
        public FileSystemTree() : base()
        {
            _presenter = new FileSystemTreePresenter(this);

            this.ItemHeight = 30;
            this.ShowPlusMinus = true;
            this.Dock = DockStyle.Fill;
            this.BorderStyle = BorderStyle.None;
            this.Font = new Font("Verdana", 12);

            ImageList nodeIcons = new ImageList();
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));
            this.ImageList = nodeIcons;
            this.ImageList.ImageSize = new Size(18, 18);

            this.BeforeExpand += FileSystemTree_Expand;

            _presenter.LoadDrives();
        }

        private void FileSystemTree_Expand(object sender, TreeViewCancelEventArgs e)
        {
            //DirectoryView.BeginUpdate();
            _presenter.PreloadContent(e.Node as FileSystemNode);
            //DirectoryView.EndUpdate();
        }

        public void AddNode(IFileSystemNode node)
        {
            this.Nodes.Add(node as FileSystemNode);
        }

        public void AddNodes(IFileSystemNode[] nodes)
        {
            foreach (IFileSystemNode n in nodes)
            {
                this.AddNode(n);
            }
        }

        public void MountDrives(List<IFileSystemNode> drives)
        {
            foreach (IFileSystemNode d in drives)
            {
                this.AddNode(d);
            }
        }
    }
}
