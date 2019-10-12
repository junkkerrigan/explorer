using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Explorer.Presenters;

namespace Explorer.Views
{
    /// <summary>
    /// Specifies indexes of types of file system elements' icons in ImageList.
    /// </summary>
    public static class IconTypeIndexes
    {
        public const int DriveIndex = 0;
        public const int FolderIndex = 1;
        public const int FileIndex = 2;
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

        IFileSystemNode IFileSystemNode.Parent
        {
            get
            {
                return base.Parent as IFileSystemNode;
            }
        }

        public bool IsAccessible { get; set; }

        public bool IsFilled { get; set; }

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
        
        public IFileSystemElement Element { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemNode"/> class
        /// with specified name.
        /// </summary>
        public FileSystemNode(string name) : base(name)
        {
            // TODO: fill context menu
            // TODO: comments
            // TODO: bind keys and context menu items

            this.IsAccessible = true;
            this.IsFilled = false;
            this.ContextMenuStrip = new ContextMenuStrip();
        }

        public void Fill()
        {
            try
            {
                Presenter.FillNode();
            }
            catch(Exception)
            {
            }
        }

        // TODO: override in FileNode with NotSupported
        public void AddSubNode(IFileSystemNode node)
        {
            string newPath = System.IO.Path.Combine(this.Element.Path, node.Text);
            node.Element.UpdatePath(newPath);
            this.Nodes.Add(node as FileSystemNode);
        }

        public class NodeComparer : IComparer<IFileSystemNode>
        {
            public int Compare(IFileSystemNode X, IFileSystemNode Y)
            {
                if (X is FolderNode && Y is FileNode) return 1;
                else if (X is FileNode && Y is FolderNode) return -1;
                else return String.Compare(X.Text, Y.Text);
            }
        }

        public void Sort()
        {
            List<IFileSystemNode> subNodes = this.SubNodes;
            this.Nodes.Clear();
            subNodes.Sort(new NodeComparer());
            foreach (var a in subNodes)
            {
                Console.WriteLine(a.Text);
            }
            this.AddSubNodes(subNodes);
        }

        public void AddSubNodes(List<IFileSystemNode> nodes)
        {
            foreach (IFileSystemNode n in nodes)
            {
                this.AddSubNode(n);
            }
        }

        public void RemoveSubNode(IFileSystemNode node)
        {
            this.Nodes.Remove(node as FileSystemNode);
        }

        void IFileSystemNode.Remove()
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

        public void StartNameEditing()
        {
            this.TreeView.LabelEdit = true;
            this.BeginEdit();
        }

        public void ShowProperties()
        {
        }

        public void MarkAsInaccessible()
        {
            this.IsAccessible = false;
            this.ForeColor = Color.Gray;
            this.ContextMenuStrip.Items.Clear();
        }

        public abstract IFileSystemNode GetClone();

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

        // TODO: if required, change to IFSN.Method()

        public void AddContextMenuItem(string name, Action onClick)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(name);
            item.Click += (s, e) => onClick();
            this.ContextMenuStrip.Items.Add(item);
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
            Element = new DirectoryElement(this);
            Presenter = new DriveNodePresenter(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.DriveIndex;
        }

        public override IFileSystemNode GetClone()
        {
            DriveNode clone = new DriveNode(this.Text)
            {
                IsFilled = this.IsFilled,
                IsAccessible = this.IsAccessible,
            };
            clone.Element.Path = this.Element.Path;

            foreach (IFileSystemNode node in this.SubNodes)
            {
                clone.AddSubNode(node.GetClone());
            }

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
            Element = new DirectoryElement(this);
            Presenter = new FolderNodePresenter(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.FolderIndex;            
        }

        public override IFileSystemNode GetClone()
        {
            FolderNode clone = new FolderNode(this.Text)
            {
                IsFilled = this.IsFilled,
                IsAccessible = this.IsAccessible,
            };
            clone.Element.Path = this.Element.Path;

            foreach (IFileSystemNode node in this.SubNodes)
            {
                clone.AddSubNode(node.GetClone());
            }

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
            Element = new FileElement(this);
            Presenter = new FileNodePresenter(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.FileIndex;
            this.IsFilled = true;
        }

        public override IFileSystemNode GetClone()
        {
            FileNode clone = new FileNode(this.Text)
            {
                IsFilled = this.IsFilled,
                IsAccessible = this.IsAccessible,
            };
            clone.Element.Path = this.Element.Path;

            return clone;
        }
    }
}
