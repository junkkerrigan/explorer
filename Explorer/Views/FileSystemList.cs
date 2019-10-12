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
            this.View = View.List;
            this.ContextMenu = new ContextMenu();
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
            foreach (IFileSystemTreeNode node in tree.RootNodes)
            {
                this.AddItem(node.ListItem);
            }
        }

        public void Display(IFileSystemTreeNode node)
        {
            // TODO: move to presenter
            this.Items.Clear();
            foreach (IFileSystemTreeNode subNode in node.SubNodes)
            {
                this.AddItem(subNode.ListItem);
            }
        }
    }
}
