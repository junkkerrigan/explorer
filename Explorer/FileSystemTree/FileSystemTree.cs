using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer
{
    /// <summary>
    /// Displays hierarchical collection of file system elements, each 
    /// represented by <see cref="FileSystemTreeNode"/>.
    /// </summary>
    public class FileSystemTree : TreeView, IFileSystemTree
    {
        private readonly FileSystemTreePresenter Presenter;

        public IFileSystemList List { get; set; }

        public List<IFileSystemTreeNode> RootNodes
        {
            get
            {
                List<IFileSystemTreeNode> rootNodes = new List<IFileSystemTreeNode>();
                foreach (IFileSystemTreeNode node in this.Nodes)
                {
                    rootNodes.Add(node);
                }
                return rootNodes;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemTree"/>.
        /// </summary>
        public FileSystemTree(IFileSystemList list) : base()
        {
            Presenter = new FileSystemTreePresenter(this);
            List = list;
            List.Tree = this;

            this.ItemHeight = Constants.ViewItemHeight;
            this.ShowPlusMinus = true;
            this.Dock = DockStyle.Fill;
            this.BorderStyle = BorderStyle.None;
            this.Font = Constants.ViewItemFont;

            ImageList nodeIcons = new ImageList
            {
                ImageSize = Constants.FileSystemNodeImageSize
            };
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));

            this.ImageList = nodeIcons;

            this.BeforeExpand += FileSystemTree_BeforeExpand;
            this.AfterLabelEdit += FileSystemTree_AfterLabelEdit;
            this.NodeMouseDoubleClick += (s, e) =>
            {
                (this.SelectedNode as IFileSystemTreeNode).Open();
            };

            Presenter.LoadDrives();
            List.DisplayTree();
        }

        public void DisplayOnListView()
        {
            this.List.DisplayTree();
        }

        private void FileSystemTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            Presenter.PreloadContent(e.Node as IFileSystemTreeNode);
        }

        private void FileSystemTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            IFileSystemTreeNode node = e.Node as IFileSystemTreeNode;

            try
            {
                node.Entity.EditName(e.Label);
            }
            catch (DirectoryAlreadyExistsException)
            {
                MessageBox.Show("Impossible to rename: directory with name"
                    + $" `{e.Label}` already exists.", "Renaming error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
            }
            catch (FileAlreadyExistsException)
            {
                MessageBox.Show($"Impossible to rename: `{e.Label}` already exists.",
                    "Renaming error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
            }
            finally
            {
                if (!e.CancelEdit)
                {
                    node.ListItem.Name = e.Label;
                    this.BeginInvoke(
                        new Action(() => {
                            node.Parent.SortSubNodes();
                        })
                    );
                    // for sorting after node name will finally be changed
                }
                this.LabelEdit = false;
            }
        }

        public void AddNode(IFileSystemTreeNode node)
        {
            this.Nodes.Add(node as FileSystemTreeNode);
        }

        public void AddNodes(List<IFileSystemTreeNode> nodes)
        {
            foreach (IFileSystemTreeNode n in nodes)
            {
                this.AddNode(n);
            }
        }

        public void MountDrives(List<IFileSystemTreeNode> drives)
        {
            this.AddNodes(drives);
        }
    }
}
