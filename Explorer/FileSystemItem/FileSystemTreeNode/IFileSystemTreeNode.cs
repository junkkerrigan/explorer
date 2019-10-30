using System.Collections.Generic;
using System.IO;
using System;

namespace Explorer
{
    /// <summary>
    /// Provides an interface to interact with <see cref="FileSystemTreeNode"/>.
    /// </summary>
    public interface IFileSystemTreeNode : IFileSystemItem
    {
        // TODO: comments

        IFileSystemTreeNode Parent { get; }

        FileSystemItemPresenter Presenter { get;  }

        /// <summary>
        /// Indicates if subnodes of the node are filled.
        /// </summary>
        bool IsFilled { get; set; }

        /// <summary>
        /// Indicates if it's possible to interact with element.
        /// </summary>
        bool IsAccessible { get; set; }

        List<IFileSystemTreeNode> SubNodes { get; }

        IFileSystemListItem ListItem { get; }

        IFileSystemTree Tree { get; }

        /// <summary>
        /// Creates a new node with the same parameters.
        /// </summary>
        /// <returns>Created node.</returns>
        IFileSystemTreeNode GetClone();

        void MarkAsSelected();

        void DisplayOnListView();

        Action Open { get; }

        public static class Factory
        {
            public static IFileSystemTreeNode GetNewDriveNode(string name)
            {
                return new DriveNode(name);
            }
            
            public static IFileSystemTreeNode GetNewFolderNode(string name)
            {
                return new FolderNode(name);
            }
            
            public static IFileSystemTreeNode GetNewFileNode(string name)
            {
                return new FileNode(name);
            }
        }

        /// <summary>
        /// Adds a single previously created node to the end of the list of subnodes
        /// and assigns its Path property depending on Path of this node.
        /// </summary>
        /// <param name="node"></param>
        void AddSubNode(IFileSystemTreeNode node);

        /// <summary>
        /// Adds a list of previously created nodes to the end of the list of subnodes.
        /// </summary>
        /// <param name="node"></param>
        void AddSubNodes(List<IFileSystemTreeNode> nodes);

        void SortSubNodes(bool display = true);

        void RemoveSubNode(IFileSystemTreeNode node);

        void Remove();

        void Expand();

        void ExpandAll();

        void Collapse();

        /// <summary>
        /// Fills node with its subnodes.
        /// </summary>
        void Fill();

        /// <summary>
        /// Checks if passed node is an ancestor of this node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>True, if passed node is an ancestor of this node, 
        /// false, otherwise</returns>
        bool IsChild(IFileSystemTreeNode ancestor);

        /// <summary>
        /// Makes item inaccessible;
        /// </summary>
        void MarkAsInaccessible();
    }
}
