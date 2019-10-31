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
            ListItem = new FileItem(this);
            Presenter = new FileItemPresenter(this);

            this.ImageIndex = this.SelectedImageIndex = Constants.IconTypeIndexes.FileIndex;
            
            this.IsFilled = true;

            this.Open = this.Entity.OpenWithDefaultApplication;

            string[] contextMenuOptions =
            {
                "Open", "Copy", "Cut", "Delete", "Rename", 
            };

            foreach (string option in contextMenuOptions)
            {
                this.AddContextMenuOption(option);
            }

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
