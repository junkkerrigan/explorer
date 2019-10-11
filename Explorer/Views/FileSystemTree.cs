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

            this.ItemHeight = Globals.ViewItemHeight;
            this.ShowPlusMinus = true;
            this.Dock = DockStyle.Fill;
            this.BorderStyle = BorderStyle.None;
            this.Font = Globals.ViewItemFont;
            this.LabelEdit = true;


            ImageList nodeIcons = new ImageList();
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));
            this.ImageList = nodeIcons;
            this.ImageList.ImageSize = Globals.FileSystemNodeImageSize;

            this.BeforeExpand += FileSystemTree_BeforeExpand;
            this.AfterLabelEdit += FileSystemTree_AfterLabelEdit;

            _presenter.LoadDrives();
        }

        private void FileSystemTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            _presenter.PreloadContent(e.Node as IFileSystemNode);
        }

        private void FileSystemTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            //(e.Node as IFileSystemNode).EditElementName();
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
