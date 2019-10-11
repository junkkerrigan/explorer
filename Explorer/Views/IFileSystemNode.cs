using System.Collections.Generic;
using System.IO;
using System;

namespace Explorer.Views
{
    /// <summary>
    /// Provides an interface to interact with <see cref="FileSystemNode"/>.
    /// </summary>
    public interface IFileSystemNode
    {
        // TODO: comments

        /// <summary>
        /// Text in node.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Indicates if it's possible to interact with element.
        /// </summary>
        bool IsAccessible { get; set; }

        /// <summary>
        /// Indicates if subnodes of the node are filled.
        /// </summary>
        bool IsFilled { get; set; }

        IFileSystemElement Element { get; set; }

        List<IFileSystemNode> SubNodes { get; }

        /// <summary>
        /// Creates a new node with the same parameters.
        /// </summary>
        /// <returns>Created node.</returns>
        IFileSystemNode GetClone();

        /// <summary>
        /// Creates a new node with the same parameters but specified Path property.
        /// </summary>
        /// <returns>Created node.</returns>
        IFileSystemNode GetClone(string Path);

        /// <summary>
        /// Adds new option to right-click menu.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="onClick"></param>
        void AddContextMenuItem(string name, Action onClick);

        public static class Factory
        {
            public static IFileSystemNode GetNewDriveNode(string name)
            {
                return new DriveNode(name);
            }
            
            public static IFileSystemNode GetNewFolderNode(string name)
            {
                return new FolderNode(name);
            }
            
            public static IFileSystemNode GetNewFileNode(string name)
            {
                return new FileNode(name);
            }
        }

        /// <summary>
        /// Adds a single previously created node to the end of the list of subnodes
        /// and assigns its Path property depending on Path of this node.
        /// </summary>
        /// <param name="node"></param>
        void AddSubNode(IFileSystemNode node);

        /// <summary>
        /// Adds a list of previously created nodes to the end of the list of subnodes.
        /// </summary>
        /// <param name="node"></param>
        void AddSubNodes(IFileSystemNode[] nodes);

        void RemoveSubNode(IFileSystemNode node);

        void Remove();

        void Expand();

        void ExpandAll();

        void Collapse();

        void StartNameEditing();

        void ShowProperties();

        /// <summary>
        /// Fills node with its subnodes.
        /// </summary>
        void Fill();

        /// <summary>
        /// Highlights a node as inaccessible.
        /// </summary>
        void MarkAsInaccessible();

        /// <summary>
        /// Checks if passed node is an ancestor of this node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>True, if passed node is an ancestor of this node, 
        /// false, otherwise</returns>
        bool IsChild(IFileSystemNode ancestor);
    }
}
