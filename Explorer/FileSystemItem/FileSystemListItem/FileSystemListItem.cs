using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public abstract class FileSystemListItem : ListViewItem, IFileSystemListItem
    {
        // TODO: highlight after changes
        
        // TODO: split classes

        public new string Name
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

        public FileSystemItemPresenter Presenter { get; set; }

        protected ContextMenuStrip RightClickMenu { get; set; }

        public FileSystemListItem(string name) : base(name)
        {
        }

        public FileSystemListItem(IFileSystemTreeNode node) : base(node.Name)
        {
            Node = node;

            this.RightClickMenu = new ContextMenuStrip();
        }

        public Action Open { get; set; }

        public void MarkAsInaccessible()
        {
            this.ForeColor = Color.Gray;
            this.Open = () => { };
            this.RightClickMenu.Items.Clear();
        }

        public void ShowMenu()
        {
            this.RightClickMenu.Show(Cursor.Position);
        }

        public void ShowProperties()
        {
            // ShowModalProperties();
        }

        public void StartNameEditing()
        {
            this.ListView.LabelEdit = true; 
            this.BeginEdit();
        }

        public void Display()
        {
            this.List.Display(this);
        }

        protected void AddContextMenuOption(string name)
        {
            ToolStripMenuItem option = new ToolStripMenuItem(name);
            option.Click += (s, e) => this.Presenter.HandleListItemContextMenuAction(name);

            this.RightClickMenu.Items.Add(option);
        }
    }

    public class DriveItem : FileSystemListItem
    {
        public DriveItem(IFileSystemTreeNode node) : base(node)
        {
            this.ImageIndex = Globals.IconTypeIndexes.DriveIndex;

            string[] contextMenuOptions =
            {
                "Open", "Paste", "Properties",
            };

            foreach(string option in contextMenuOptions)
            {
                this.AddContextMenuOption(option);
            }

            this.Open = () =>
            {
                this.Node.Fill();
                foreach (IFileSystemTreeNode node in this.Node.SubNodes)
                {
                    node.Fill();
                }
                List.Display(this);
            };
        }
    }

    public class FolderItem : FileSystemListItem
    {
        public FolderItem(IFileSystemTreeNode node) : base(node)
        {
            this.ImageIndex = Globals.IconTypeIndexes.FolderIndex;

            string[] contextMenuOptions =
            {
                "Open", "Copy", "Cut", "Paste", "Delete", "Rename", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                this.AddContextMenuOption(option);
            }

            this.Open = () =>
            {
                this.Node.Fill();
                foreach(IFileSystemTreeNode node in this.Node.SubNodes)
                {
                    node.Fill();
                }
                List.Display(this);
            };
        }
    }

    public class FileItem : FileSystemListItem
    {
        public FileItem(IFileSystemTreeNode node) : base(node)
        {
            this.ImageIndex = Globals.IconTypeIndexes.FileIndex;

            string[] contextMenuOptions =
            {
                "Open", "Copy", "Cut", "Delete", "Rename", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                this.AddContextMenuOption(option);
            }

            this.Open = () =>
            {
                this.Entity.OpenWithDefaultApplication();
            };
        }
    }

    public class BackToFolder : FileSystemListItem
    {
        public BackToFolder() : base("...")
        {
            this.ImageIndex = Globals.IconTypeIndexes.BackToFolderIndex;

            this.Open = () =>
            {
                IFileSystemTreeNode parent = this.List.DisplayedNode.Parent;
                if (parent == null)
                {
                    List.DisplayTree();
                }
                else
                {
                    List.Display(parent);
                }
            };
        }
    }

    public class CurrentFolder : FileSystemListItem
    {
        public CurrentFolder(string name) : base(name)
        {
            this.Open = () => { };
            this.Font = new Font("Verdana", 11, FontStyle.Italic);
        } 
    }

    public class Separator : FileSystemListItem
    {
        public Separator() : base("")
        {
            this.Open = () => { };
        }
    }
}
