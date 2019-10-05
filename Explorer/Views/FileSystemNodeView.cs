using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

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
    public abstract class FileSystemNode : TreeNode
    {
        /// <summary>
        /// Absolute path to element.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Indicates if it's possible to interact with element.
        /// </summary>
        public bool Accessible { get; set; }

        /// <summary>
        /// Indicates if subnodes of the node are filled.
        /// </summary>
        public bool Filled { get; set; }

        private static FileSystemNode _buffer = null;

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemNode"/> class.
        /// </summary>
        public FileSystemNode() : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="FileSystemNode"/> class
        /// with specified name.
        /// </summary>
        public FileSystemNode(string name) : base(name)
        {
            this.Accessible = true;
            this.Filled = false;
            this.NodeFont = new Font("Verdana", 12);
            this.ContextMenuStrip = new ContextMenuStrip();

            AddContextMenuItem("Copy", CopyToBuffer);
            AddContextMenuItem("Paste", PasteFromBuffer);

            void CopyToBuffer(object sender, EventArgs e)
            {
                _buffer = this.Clone();
            }

            void PasteFromBuffer(object sender, EventArgs e)
            {
                FileSystemNode nodeClone = _buffer.Clone();
                if (this.Path[this.Path.Length - 1] == '\\')
                {
                    nodeClone.Path = this.Path + nodeClone.Text;
                }
                else
                {
                    nodeClone.Path = this.Path + '\\' + nodeClone.Text;
                }
                Console.WriteLine(nodeClone.Path);
                this.Nodes.Add(nodeClone);
            }
        }

        public new FileSystemNode Clone()
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

        public void MarkAsInaccessible()
        {
            this.Accessible = false;
            this.ForeColor = Color.Gray;
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
