using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    /// <summary>
    /// Respresents a node of a system drive in <see cref="FileSystemTree"/>.
    /// </summary>
    public class DriveNode : FileSystemTreeNode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DriveNode"/> class
        /// with specified name.
        /// </summary>
        public DriveNode(string name) : base(name)
        {
            Entity = new DriveEntity(this);
            Presenter = new DriveNodePresenter(this);
            ListItem = new DriveItem(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.DriveIndex;
        }

        public override IFileSystemTreeNode GetClone()
        {
            DriveNode clone = new DriveNode(this.Name)
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
