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
        public new string Text
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
        public FileSystemNode(string name) : base(name)
        {
            _presenter = new FileSystemNodePresenter(this);

            this.Accessible = true;
            this.Filled = false;
            this.NodeFont = new Font("Verdana", 12);
            this.ContextMenuStrip = new ContextMenuStrip();

            AddContextMenuItem("Copy", FileSystemNode_Copy);
            AddContextMenuItem("Paste", FileSystemNode_Paste);
        }

        public void Fill()
        {
            _presenter.FillNode(this);
        }

        public void AddNode(IFileSystemNode node)
        {
            Console.WriteLine($"Adding node with path {node.Path}");
            Console.WriteLine($"to node with path {this.Path}");
            this.Nodes.Add(node as FileSystemNode);
        }

        public void AddNodes(IFileSystemNode[] nodes)
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

        public abstract IFileSystemNode GetClone();

        public bool IsChild(IFileSystemNode ancestor)
        {
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

        public override IFileSystemNode GetClone()
        {
            DriveNode clone = new DriveNode(this.Text)
            {
                Path = this.Path,
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

        public override IFileSystemNode GetClone()
        {
            FolderNode clone = new FolderNode(this.Text)
            {
                Path = this.Path,
                Accessible = this.Accessible,
            }; 
            clone.Fill();
            Console.WriteLine($"Folder clone: text is {clone.Text}\npath is {clone.Path}");

            foreach(var n in clone.SubNodes)
            {
                Console.WriteLine($"Folder clone subnode: text is {clone.Text}\npath is {clone.Path}");
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

        public override IFileSystemNode GetClone()
        {
            FileNode clone = new FileNode(this.Text)
            {
                Path = this.Path,
                Accessible = this.Accessible,
            };
            return clone;
        }
    }
}
