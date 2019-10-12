using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Views
{
    public class FileSystemListItem : ListViewItem, IFileSystemListItem
    {
        string IFileSystemItem.Name
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public IFileSystemList List
        {
            get
            {
                return this.ListView as IFileSystemList;
            }
        }

        public IFileSystemItemEntity Entity 
        {
            get
            {
                return Node.Entity;
            } 
        }

        public IFileSystemTreeNode Node { get; set; }

        public FileSystemListItem(IFileSystemTreeNode node) : base(node.Name)
        {
            Node = node;
        }
            
        public void AddContextMenuOption(string name, Action onClick)
        {
            throw new NotImplementedException();
        }

        public void ShowProperties()
        {
            // ShowModalProperties();
        }

        public void StartNameEditing()
        {
            this.BeginEdit();
        }
    }

    public class DriveItem : FileSystemListItem
    {
        public DriveItem(IFileSystemTreeNode node) : base(node)
        {

        }
    }

    public class FolderItem : FileSystemListItem
    {
        public FolderItem(IFileSystemTreeNode node) : base(node)
        {

        }
    }

    public class FileItem : FileSystemListItem
    {
        public FileItem(IFileSystemTreeNode node) : base(node)
        {

        }
    }
}
