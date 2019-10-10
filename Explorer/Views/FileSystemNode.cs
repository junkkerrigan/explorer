using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Explorer.Presenters;

namespace Explorer.Views
{
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
    public abstract class FileSystemNode : TreeNode, IFileSystemNode
    {
        string IFileSystemNode.Text 
        {
            get
            {
                return base.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public string Path { get; set; }

        public bool Accessible { get; set; }

        public bool Filled { get; set; }

        public List<IFileSystemNode> SubNodes
        {
            get
            {
                List<IFileSystemNode> subNodes = new List<IFileSystemNode>();
                foreach(IFileSystemNode child in this.Nodes)
                {
                    subNodes.Add(child);
                }
                return subNodes;
            }
        }

        protected IFileSystemNodePresenter Presenter { get; set; }    

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemNode"/> class
        /// with specified name.
        /// </summary>
        public FileSystemNode(string name) : base(name)
        {
            // TODO: fill context menu
            // TODO: comments
            // TODO: bind keys and context menu items

            this.Accessible = true;
            this.Filled = false;
            this.NodeFont = new Font("Verdana", 12);
            this.ContextMenuStrip = new ContextMenuStrip();
        }

        public void Fill()
        {
            Presenter.FillNode(this);
        }

        public void CopyTo(string destinationPath)
        {
            Presenter.CopyElement(this.Path, destinationPath);
        }

        public void AddNode(IFileSystemNode node)
        {
            node.Path = System.IO.Path.Combine(this.Path, node.Text);
            if (node.Filled)
            {
                foreach (IFileSystemNode child in node.SubNodes)
                {
                    child.Path = System.IO.Path.Combine(node.Path, child.Text);
                }
            }
            this.Nodes.Add(node as FileSystemNode);
        }

        public void AddNodes(IFileSystemNode[] nodes)
        {
            foreach (IFileSystemNode n in nodes)
            {
                this.AddNode(n);
            }
        }

        public void RemoveNode(IFileSystemNode node)
        {
            this.Nodes.Remove(node as FileSystemNode);
        }

        void IFileSystemNode.Delete()
        {
            this.Remove();
        }

        void IFileSystemNode.Expand()
        {
            this.Expand();
        }

        void IFileSystemNode.ExpandAll()
        {
            this.ExpandAll();
        }

        void IFileSystemNode.Collapse()
        {
            this.Collapse();
        }

        public void MarkAsInaccessible()
        {
            this.Accessible = false;
            this.ForeColor = Color.Gray;
        }

        public IFileSystemNode GetClone()
        {
            return GetClone(this.Path);
        }

        public abstract IFileSystemNode GetClone(string Path);

        public bool IsChild(IFileSystemNode ancestor)
        {
            // TODO: change cause cur is reference 
            FileSystemNode cur = this, 
                           parent = ancestor as FileSystemNode;
            while(cur.Parent != null)
            {
                cur = cur.Parent as FileSystemNode;
                if (cur == parent)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDirectChild(IFileSystemNode parent)
        {
            return parent == this.Parent;
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

        protected void FileSystemNode_Open(object sender, EventArgs e)
        {
            Presenter.OpenWithDefaultApplication();
        }

        protected void FileSystemNode_Copy(object sender, EventArgs e)
        {
            Presenter.CopyNodeToBuffer();
        }

        protected void FileSystemNode_Cut(object sender, EventArgs e)
        {
            Presenter.CutNode();
        }

        //protected void FileSystemNode_Move(object sender, EventArgs e)
        //{
        //    Presenter.MoveNode();
        //}

        protected void FileSystemNode_Paste(object sender, EventArgs e)
        {
            Presenter.PasteNodeFromBufferAsync();
        }

        protected void FileSystemNode_Delete(object sender, EventArgs e)
        {
            Presenter.DeleteNode();
        }

        protected void FileSystemNode_Expand(object sender, EventArgs e)
        {
            Presenter.ExpandNode();
        }

        protected void FileSystemNode_ExpandAll(object sender, EventArgs e)
        {
            Presenter.ExpandAllSubNodes();
        }

        protected void FileSystemNode_Collapse(object sender, EventArgs e)
        {
            Presenter.CollapseNode();
        }

        protected void FileSystemNode_Properties(object sender, EventArgs e)
        {
            // TODO: ShowModalProperties();
        }
    }

    /// <summary>
    /// Respresents a node of a system drive in <see cref="FileSystemTree"/>.
    /// </summary>
    public class DriveNode : FileSystemNode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DriveNode"/> class
        /// with specified name.
        /// </summary>

        public DriveNode(string name) : base(name)
        {
            Presenter = new DirectoryNodePresenter(this);

            this.ImageIndex = this.SelectedImageIndex = (int)IconType.Drive;

            AddContextMenuItem("Paste", FileSystemNode_Paste);
            AddContextMenuItem("Expand", FileSystemNode_Expand);
            AddContextMenuItem("Expand all", FileSystemNode_ExpandAll);
            AddContextMenuItem("Collapse", FileSystemNode_Collapse);
            AddContextMenuItem("Properties", FileSystemNode_Properties);
        }

        public override IFileSystemNode GetClone(string Path)
        {
            DriveNode clone = new DriveNode(this.Text)
            {
                Path = Path,
                Accessible = this.Accessible,
            };
            clone.Fill();

            return clone;
        }
    }

    /// <summary>
    /// Respresents a node of a folder in <see cref="FileSystemTree"/>.
    /// </summary>
    public class FolderNode : FileSystemNode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FolderNode"/> class
        /// with specified name.
        /// </summary>
        public FolderNode(string name) : base(name)
        {
            Presenter = new DirectoryNodePresenter(this);

            this.ImageIndex = this.SelectedImageIndex = (int)IconType.Folder;

            AddContextMenuItem("Copy", FileSystemNode_Copy);
            AddContextMenuItem("Cut", FileSystemNode_Cut);
            AddContextMenuItem("Paste", FileSystemNode_Paste);
            AddContextMenuItem("Delete", FileSystemNode_Delete);
            AddContextMenuItem("Expand", FileSystemNode_Expand);
            AddContextMenuItem("Expand all", FileSystemNode_ExpandAll);
            AddContextMenuItem("Collapse", FileSystemNode_Collapse);
            AddContextMenuItem("Properties", FileSystemNode_Properties);
        }

        public override IFileSystemNode GetClone(string Te)
        {
            FolderNode clone = new FolderNode(this.Text)
            {
                Path = this.Path,
                Accessible = this.Accessible,
            };
            clone.Fill();

            return clone;
        }
    }

    /// <summary>
    /// Respresents a node of a file in <see cref="FileSystemTree"/>.
    /// </summary>
    public class FileNode : FileSystemNode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FileNode"/> class
        /// with specified name.
        /// </summary>
        public FileNode(string name) : base(name)
        {
            Presenter = new FileNodePresenter(this);

            this.ImageIndex = this.SelectedImageIndex = (int)IconType.File;

            AddContextMenuItem("Open", FileSystemNode_Open);
            AddContextMenuItem("Copy", FileSystemNode_Copy);
            AddContextMenuItem("Cut", FileSystemNode_Cut);
            AddContextMenuItem("Delete", FileSystemNode_Delete);
            AddContextMenuItem("Properties", FileSystemNode_Properties);
        }

        public override IFileSystemNode GetClone(string Path)
        {
            FileNode clone = new FileNode(this.Text)
            {
                Path = Path,
                Accessible = this.Accessible,
            };
            return clone;
        }
    }
}
