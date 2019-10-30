using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
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
            Entity = new FolderEntity(this);
            ListItem = new FolderItem(this);
            Presenter = new FolderItemPresenter(this);

            this.ImageIndex = this.SelectedImageIndex = Constants.IconTypeIndexes.FolderIndex;

            this.Open = this.DisplayOnListView;

            string[] contextMenuOptions = 
            {
                "Open", "Copy", "Cut", "Paste", "Delete", "Rename", "Expand", 
                "Expand all", "Collapse", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                this.AddContextMenuOption(option);
            }
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
}
