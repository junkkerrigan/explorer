using System.Collections.Generic;

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
        /// Absolute path to element.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Indicates if it's possible to interact with element.
        /// </summary>
        bool Accessible { get; set; }

        /// <summary>
        /// Indicates if subnodes of the node are filled.
        /// </summary>
        bool Filled { get; set; }

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

        void CopyTo(string Path);

        IFileSystemNode GetNewFolderNode(string name);

        IFileSystemNode GetNewFileNode(string name);

        /// <summary>
        /// Adds a single previously created node to the end of the list of subnodes
        /// and assigns its Path property depending on Path of this node.
        /// </summary>
        /// <param name="node"></param>
        void AddNode(IFileSystemNode node);

        /// <summary>
        /// Adds a list of previously created nodes to the end of the list of subnodes.
        /// </summary>
        /// <param name="node"></param>
        void AddNodes(IFileSystemNode[] nodes);

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

        /// <summary>
        /// Checks if passed node is a parent of this node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>True, if passed node is a parent of this node, 
        /// false, otherwise</returns>
        bool IsDirectChild(IFileSystemNode parent);
    }
}
