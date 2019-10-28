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

        public int RealWidth { get; set; }

        public virtual bool IsAccessible
        {
            get 
            {
                try {
                    return Node.IsAccessible;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return false;
            }
        }

        public FileSystemItemPresenter Presenter { get; set; }

        protected ContextMenuStrip RightClickMenu { get; set; }

        public FileSystemListItem(string name) : base(name)
        {
        }

        public FileSystemListItem() : base()
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
            this.Open = () =>
            {
                MessageBox.Show($"Impossible to open: `{this.Name}` is inaccesible.",
                    "Opening error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
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
            option.Click += (s, e) => this.Presenter.HandleListItemAction(name);

            this.RightClickMenu.Items.Add(option);
        }

        protected void AddContextMenuOption(string name, Action onClick)
        {
            ToolStripMenuItem option = new ToolStripMenuItem(name);
            option.Click += (s, e) => onClick();

            this.RightClickMenu.Items.Add(option);
        }

        public void MoveHere()
        {
            this.Presenter.HandleListItemAction("Move here");
        }

        public bool IsFileSystemItem()
        {
            return (this.Node != null && this.Entity != null);
        }
    }

    public class DriveItem : FileSystemListItem
    {
        public DriveItem(IFileSystemTreeNode node) : base(node)
        {
            this.ImageIndex = Constants.IconTypeIndexes.DriveIndex;

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
            this.ImageIndex = Constants.IconTypeIndexes.FolderIndex;

            string[] contextMenuOptions =
            {
                "Open", "Copy", "Cut", "Paste", "Move", "Delete", "Rename", "Properties",
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
            this.ImageIndex = Constants.IconTypeIndexes.FileIndex;

            string[] contextMenuOptions =
            {
                "Open", "Copy", "Cut", "Move", "Merge", "Delete", "Rename", "Properties",
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
        public override bool IsAccessible
        {
            get => true;
        }

        public BackToFolder() : base("...")
        {
            this.ImageIndex = Constants.IconTypeIndexes.BackToFolderIndex;

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

    public class Mover : FileSystemListItem
    {
        public override bool IsAccessible => true;

        public Mover() : base()
        {
            this.ImageIndex = Constants.IconTypeIndexes.MoveToIndex;
            this.Font = new Font("Verdana", 11, FontStyle.Italic);

            this.RightClickMenu = new ContextMenuStrip();
            this.AddContextMenuOption("Cancel moving", () =>
                this.Node.Presenter.HandleListItemAction("Cancel moving"));

            this.Open = () =>
            {
                this.Node.ListItem.MoveHere();
            };
        }
    }

    public class Merger : FileSystemListItem
    {
        public override bool IsAccessible => true;

        // node that will be first in result node
        public IFileSystemTreeNode MergeWithNode { get; set; }

        // node that will be second in result node
        public IFileSystemTreeNode MergeNode { get; set; }

        public Merger() : base()
        {
            this.ImageIndex = Constants.IconTypeIndexes.MergeIndex;
            this.Font = new Font("Verdana", 11, FontStyle.Italic);

            this.RightClickMenu = new ContextMenuStrip();
            this.AddContextMenuOption("Cancel merging", () =>
                this.MergeNode.Presenter.HandleListItemAction("Cancel merging"));

            this.Open = () =>
            {
                if (List.IsChoosingMergeTo)
                {
                    IFileSystemTreeNode newFile =
                            IFileSystemTreeNode.Factory.GetNewFileNode("");

                    List.DisplayedNode.AddSubNode(newFile);
                    List.DisplayedNode.Collapse();
                    List.UpdateRefresh();

                    newFile.ListItem.StartNameEditing();
                }
            };
        }
    }

    public class CurrentLocation : FileSystemListItem
    {
        public override bool IsAccessible => false;

        public bool IsMerging { get; set; }

        public CurrentLocation() : base()
        {
            this.ImageIndex = Constants.IconTypeIndexes.CurrentLocationIndex;
            this.Font = new Font("Verdana", 11, FontStyle.Italic);
            this.RightClickMenu = new ContextMenuStrip();
            this.Open = () => { };
        } 

        public void EnableMergingMode()
        {
            this.IsMerging = true;

            this.ImageIndex = Constants.IconTypeIndexes.MergeIndex;
        }

        public void DisableMergingMode()
        {
            this.IsMerging = false;

            this.ImageIndex = Constants.IconTypeIndexes.CurrentLocationIndex;

            this.RightClickMenu.Items.Clear();
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
