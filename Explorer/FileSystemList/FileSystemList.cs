using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer
{
    public class FileSystemList : ListView, IFileSystemList
    {
        public IFileSystemTree Tree { get; set; }

        static readonly BackToFolder backToFolder = new BackToFolder();

        public IFileSystemTreeNode DisplayedNode { get; set; }

        public FileSystemList() : base()
        {
            this.Dock = DockStyle.Fill;
            this.Font = Globals.ViewItemFont;
            this.BorderStyle = BorderStyle.None;
            this.View = View.Tile;
            this.TileSize = new Size(600, 30);

            ContextMenuStrip ViewMenu = new ContextMenuStrip();
            ViewMenu.Items.Add(new ToolStripMenuItem("Gen"));

            this.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (this.GetItemAt(e.X, e.Y) == null)
                    {
                        ViewMenu.Show(Cursor.Position);
                    }
                    else
                    {
                        IFileSystemListItem target = this.GetItemAt(e.X, e.Y) as IFileSystemListItem;
                        target.ShowMenu();
                    }
                }
            };

            this.MouseDoubleClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right) return; 
                IFileSystemListItem selectedItem = this.SelectedItems[0] 
                    as IFileSystemListItem;
                selectedItem.Open();
            };

            ImageList itemIcons = new ImageList
            {
                ImageSize = new Size(25, 25)
            };
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/backToFolderIcon.png"));

            this.LargeImageList = this.SmallImageList = itemIcons;
        }

        public void AddItem(IFileSystemListItem item)
        {
            this.Items.Add(item as ListViewItem);
        }

        public void AddItems(List<IFileSystemListItem> items)
        {
            foreach(IFileSystemListItem i in items)
            {
                this.AddItem(i);
            }
        }

        public void DisplayTree()
        {
            this.Items.Clear();

            foreach (IFileSystemTreeNode node in this.Tree.RootNodes)
            {
                this.AddItem(node.ListItem);
            }
        }

        public void Display(IFileSystemTreeNode node)
        {
            // TODO: move to presenter
            // TODO: handle file nodes 
            this.Items.Clear();

            this.View = View.Tile;

            int maxWidth = this.Size.Width;

            foreach (IFileSystemTreeNode subNode in node.SubNodes)
            {
                maxWidth = System.Math.Max(maxWidth, 40 + 
                    TextRenderer.MeasureText(subNode.Name, Globals.ViewItemFont).Width);
            }

            this.TileSize = new Size(maxWidth, 30);

            this.AddItem(backToFolder);

            this.DisplayedNode = node;

            foreach (IFileSystemTreeNode subNode in node.SubNodes)
            {
                this.AddItem(subNode.ListItem);
            }
        }

        public void Display(IFileSystemListItem item)
        {
            this.Display(item.Node);
        }
    }
}
