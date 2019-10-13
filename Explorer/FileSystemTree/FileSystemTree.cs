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

            this.ItemHeight = Globals.ViewItemHeight;
            this.ShowPlusMinus = true;
            this.Dock = DockStyle.Fill;
            this.BorderStyle = BorderStyle.None;
            this.Font = Globals.ViewItemFont;

            ImageList nodeIcons = new ImageList
            {
                ImageSize = Globals.FileSystemNodeImageSize
            };
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            nodeIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));

            this.ImageList = nodeIcons;

            this.BeforeExpand += FileSystemTree_BeforeExpand;
            this.AfterLabelEdit += FileSystemTree_AfterLabelEdit;
            this.NodeMouseDoubleClick += (s, e) =>
            {
                List.Display(this.SelectedNode as IFileSystemTreeNode);
            };

            Presenter.LoadDrives();
            List.Display(this);
        }

        public void DisplayOnListView()
        {
            this.List.Display(this);
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
            catch (FileAlreadyExistsException)
            {
                Console.WriteLine("File already exists");
                e.CancelEdit = true;
            }
            catch (DirectoryAlreadyExistsException)
            {
                Console.WriteLine("Directory already exists");
                e.CancelEdit = true;
            }
            finally
            {
                this.LabelEdit = false;
                if (!e.CancelEdit)
                {
                    this.BeginInvoke(
                        new Action(() => {
                            node.Parent.SortSubNodes();
                        })
                    );
                }
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
