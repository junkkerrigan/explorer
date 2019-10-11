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

        public string Path
        {
            get
            {
                return Element.Path;
            }
            set
            {
                Element.Path = value;
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
            node.Element.Path = System.IO.Path.Combine(this.Path, node.Text);
            if (node.IsFilled)
            {
                foreach (IFileSystemNode child in node.SubNodes)
                {
                    child.Element.Path = System.IO.Path.Combine(node.Element.Path, child.Text);
                }
            }
            this.Nodes.Add(node as FileSystemNode);
        }

        public void AddSubNodes(IFileSystemNode[] nodes)
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

        void IFileSystemNode.StartNameEditing()
        {
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
            Presenter = new DriveNodePresenter(this);
            Element = new DirectoryElement(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.DriveIndex;
        }

        public override IFileSystemNode GetClone(string Path)
        {
            DriveNode clone = new DriveNode(this.Text)
            {
                IsAccessible = this.IsAccessible,
            };
            clone.Path = Path;
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
            Presenter = new FolderNodePresenter(this);
            Element = new DirectoryElement(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.FolderIndex;            
        }

        public override IFileSystemNode GetClone(string Path)
        {
            FolderNode clone = new FolderNode(this.Text)
            {
                IsAccessible = this.IsAccessible,
            };
            clone.Path = Path;
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
            Element = new FileElement(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.FileIndex;
            this.IsFilled = true;
        }

        public override IFileSystemNode GetClone(string Path)
        {
            FileNode clone = new FileNode(this.Text)
            {
                IsAccessible = this.IsAccessible,
            };
            clone.Path = Path;
            return clone;
        }
    }
}
