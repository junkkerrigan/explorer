﻿using System;
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
    public abstract class FileSystemTreeNode : TreeNode, IFileSystemTreeNode
    {
        public new string Name
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

        IFileSystemTreeNode IFileSystemTreeNode.Parent
        {
            get
            {
                return base.Parent as IFileSystemTreeNode;
            }
        }

        public bool IsFilled { get; set; }

        public bool IsAccessible { get; set; }

        public List<IFileSystemTreeNode> SubNodes
        {
            get
            {
                List<IFileSystemTreeNode> subNodes = new List<IFileSystemTreeNode>();
                foreach(IFileSystemTreeNode child in this.Nodes)
                {
                    subNodes.Add(child);
                }
                return subNodes;
            }
        }

        protected FileSystemTreeNodePresenter Presenter { get; set; }
        
        public IFileSystemItemEntity Entity { get; set; }

        public IFileSystemListItem ListItem { get; set; }

        public IFileSystemTree Tree
        {
            get
            {
                return this.TreeView as IFileSystemTree;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemTreeNode"/> class
        /// with specified name.
        /// </summary>
        public FileSystemTreeNode(string name) : base(name)
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
        public void AddSubNode(IFileSystemTreeNode node)
        {
            string newPath = System.IO.Path.Combine(this.Entity.Path, node.Name);
            node.Entity.UpdatePath(newPath);
            this.Nodes.Add(node as FileSystemTreeNode);
        }

        // TODO: if required, move or change
        public class NodeComparer : IComparer<IFileSystemTreeNode>
        {
            public int Compare(IFileSystemTreeNode X, IFileSystemTreeNode Y)
            {
                if (X is FolderNode && Y is FileNode) return -1;
                else if (X is FileNode && Y is FolderNode) return 1;
                else return String.Compare(X.Name, Y.Name);
            }
        }

        public void SortSubNodes()
        {
            List<IFileSystemTreeNode> subNodes = this.SubNodes;
            this.Nodes.Clear();
            subNodes.Sort(new NodeComparer());
            this.AddSubNodes(subNodes);
        }

        public void AddSubNodes(List<IFileSystemTreeNode> nodes)
        {
            foreach (IFileSystemTreeNode n in nodes)
            {
                this.AddSubNode(n);
            }
        }

        public void RemoveSubNode(IFileSystemTreeNode node)
        {
            this.Nodes.Remove(node as FileSystemTreeNode);
        }

        void IFileSystemTreeNode.Remove()
        {
            this.Remove();
        }

        void IFileSystemTreeNode.Expand()
        {
            this.Expand();
        }

        void IFileSystemTreeNode.ExpandAll()
        {
            this.ExpandAll();
        }

        void IFileSystemTreeNode.Collapse()
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
            this.ForeColor = Color.Gray;
            this.ContextMenuStrip.Items.Clear();
        }

        public abstract IFileSystemTreeNode GetClone();

        public bool IsChild(IFileSystemTreeNode ancestor)
        {
            // TODO: change cause cur is reference 
            FileSystemTreeNode cur = this, 
                           parent = ancestor as FileSystemTreeNode;
            while(cur.Parent != null)
            {
                cur = cur.Parent as FileSystemTreeNode;
                if (cur == parent)
                {
                    return true;
                }
            }
            return false;
        }

        // TODO: if required, change to IFSN.Method()

        public void AddContextMenuOption(string name, Action onClick)
        {
            ToolStripMenuItem option = new ToolStripMenuItem(name);
            option.Click += (s, e) => onClick();

            this.ContextMenuStrip.Items.Add(option);
        }
    }

    /// <summary>
    /// Respresents a node of a system drive in <see cref="FileSystemTree"/>.
    /// </summary>
    public class DriveNode : FileSystemTreeNode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DriveNode"/> class
        /// with specified name.
        /// </summary>
        public DriveNode(string name) : base(name)
        {
            Entity = new DirectoryEntity(this);
            Presenter = new DriveNodePresenter(this);
            ListItem = new DriveItem(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.DriveIndex;
        }

        public override IFileSystemTreeNode GetClone()
        {
            DriveNode clone = new DriveNode(this.Name)
            {
                IsFilled = this.IsFilled,
            };
            clone.Entity.Path = this.Entity.Path;
            clone.IsAccessible = this.IsAccessible;

            foreach (IFileSystemTreeNode node in this.SubNodes)
            {
                clone.AddSubNode(node.GetClone());
            }

            return clone;
        }
    }

    /// <summary>
    /// Respresents a node of a folder in <see cref="FileSystemTree"/>.
    /// </summary>
    public class FolderNode : FileSystemTreeNode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FolderNode"/> class
        /// with specified name.
        /// </summary>
        public FolderNode(string name) : base(name)
        {
            Entity = new DirectoryEntity(this);
            Presenter = new FolderNodePresenter(this);
            ListItem = new FolderItem(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.FolderIndex;            
        }

        public override IFileSystemTreeNode GetClone()
        {
            FolderNode clone = new FolderNode(this.Name)
            {
                IsFilled = this.IsFilled,
            };
            clone.Entity.Path = this.Entity.Path;
            clone.IsAccessible = this.IsAccessible;

            foreach (IFileSystemTreeNode node in this.SubNodes)
            {
                clone.AddSubNode(node.GetClone());
            }

            return clone;
        }
    }

    /// <summary>
    /// Respresents a node of a file in <see cref="FileSystemTree"/>.
    /// </summary>
    public class FileNode : FileSystemTreeNode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FileNode"/> class
        /// with specified name.
        /// </summary>
        public FileNode(string name) : base(name)
        {
            Entity = new FileEntity(this);
            Presenter = new FileNodePresenter(this);
            ListItem = new FileItem(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.FileIndex;
            this.IsFilled = true;
        }

        public override IFileSystemTreeNode GetClone()
        {
            FileNode clone = new FileNode(this.Name)
            {
                IsFilled = this.IsFilled,
            };
            clone.Entity.Path = this.Entity.Path;
            clone.IsAccessible = this.IsAccessible;
            
            return clone;
        }
    }
}
