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
            Presenter = new FolderNodePresenter(this);
            ListItem = new FolderItem(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.FolderIndex;
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
