using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.Views
{
    public class FileSystemList : ListView, IFileSystemList
    {
        public FileSystemList() : base()
        {
            this.Dock = DockStyle.Fill;
            this.Font = Globals.ViewItemFont;
            this.BorderStyle = BorderStyle.None;
            this.View = View.Tile;
            this.ContextMenu = new ContextMenu();

            ImageList itemIcons = new ImageList
            {
                ImageSize = new Size(25, 25)
            };
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));

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

        public void Display(IFileSystemTree tree)
        {
            this.Items.Clear();

            this.TileSize = new Size((tree as TreeView).Size.Width, 30);

            foreach (IFileSystemTreeNode node in tree.RootNodes)
            {
                this.AddItem(node.ListItem);
            }
        }

        public void Display(IFileSystemTreeNode node)
        {
            // TODO: move to presenter
            this.Items.Clear();

            int maxWidth = this.Size.Width;

            foreach (IFileSystemTreeNode subNode in node.SubNodes)
            {
                maxWidth = System.Math.Max(maxWidth, 40 + 
                    TextRenderer.MeasureText(subNode.Name, Globals.ViewItemFont).Width);
            }

            this.TileSize = new Size(maxWidth, 30);

            foreach (IFileSystemTreeNode subNode in node.SubNodes)
            {
                this.AddItem(subNode.ListItem);
            }
        }
    }
}
