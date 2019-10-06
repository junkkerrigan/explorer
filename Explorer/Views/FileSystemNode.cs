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
        public string Path { get; set; }

        public bool Accessible { get; set; }

        public bool Filled { get; set; }

        public List<IFileSystemNode> SubNodes
        {
            get
            {
                List<IFileSystemNode> subNodes = new List<IFileSystemNode>();
                foreach(FileSystemNode node in this.Nodes)
                {
                    subNodes.Add(node);
                }
                return subNodes;
            }
        }

        private readonly IFileSystemNodePresenter _presenter;

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemNode"/> class.
        /// </summary>
        public FileSystemNode() : base() 
        {
            _presenter = new FileSystemNodePresenter(this);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemNode"/> class
        /// with specified name.
        /// </summary>
        public FileSystemNode(string name) : base(name + ' ')
        {
            _presenter = new FileSystemNodePresenter(this);

            this.Accessible = true;
            this.Filled = false;
            this.NodeFont = new Font("Verdana", 12);
            this.ContextMenuStrip = new ContextMenuStrip();

            AddContextMenuItem("Copy", FileSystemNode_Copy);
            AddContextMenuItem("Paste", FileSystemNode_Paste);
        }

        public void Add(IFileSystemNode node)
        {
            this.Nodes.Add(node as FileSystemNode);
        }

        public void Add(IFileSystemNode[] nodes)
        {
            foreach (IFileSystemNode n in nodes)
            {
                this.Nodes.Add(n as FileSystemNode);
            }
        }

        public void MarkAsInaccessible()
        {
            this.Accessible = false;
            this.ForeColor = Color.Gray;
        }

        private FileSystemNode GetClone()
        {
            FileSystemNode clone = base.Clone() as FileSystemNode;
            clone.Path = this.Path;
            clone.Accessible = this.Accessible;
            clone.Filled = this.Filled;
            return clone;
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

        private void FileSystemNode_Copy(object sender, EventArgs e)
        {
            _presenter.CopyNodeToBuffer(this);
        }

        private void FileSystemNode_Paste(object sender, EventArgs e)
        {
            _presenter.PasteNodeFromBuffer(this);
        }
    }

    /// <summary>
    /// Respresents a node of a system drive in <see cref="FileSystemTree"/>.
    /// </summary>
    public class DriveNode : FileSystemNode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DriveNode"/> class.
        /// </summary>
        public DriveNode() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="DriveNode"/> class
        /// with specified name.
        /// </summary>
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
        /// <summary>
        /// Initializes a new instance of <see cref="FolderNode"/> class.
        /// </summary>
        public FolderNode() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="FolderNode"/> class
        /// with specified name.
        /// </summary>
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
        /// <summary>
        /// Initializes a new instance of <see cref="FileNode"/> class.
        /// </summary>
        public FileNode() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="FileNode"/> class
        /// with specified name.
        /// </summary>
        public FileNode(string name) : base(name)
        {
            this.ImageIndex = this.SelectedImageIndex = (int)IconType.File;
        }
    }
}
