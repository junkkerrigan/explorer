using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    /// <summary>
    /// Respresents a node of a file in <see cref="FileSystemTree"/>.
    /// </summary>
    public class FileNode : FileSystemTreeNode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FileNode"/> class
        /// with specified name.
        /// </summary>
        public FileNode(string name) : base(name)
        {
            Entity = new FileEntity(this);
            Presenter = new FileNodePresenter(this);
            ListItem = new FileItem(this);

            this.ImageIndex = this.SelectedImageIndex = IconTypeIndexes.FileIndex;
            this.IsFilled = true;
        }

        public override IFileSystemTreeNode GetClone()
        {
            FileNode clone = new FileNode(this.Name)
            {
                IsFilled = this.IsFilled,
            };
            clone.Entity.Path = this.Entity.Path;
            clone.IsAccessible = this.IsAccessible;

            return clone;
        }
    }
}
