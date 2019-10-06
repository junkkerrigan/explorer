using System.Collections.Generic;

namespace Explorer.Views
{
    /// <summary>
    /// Provides an interface to interact with <see cref="FileSystemNode"/>.
    /// </summary>
    public interface IFileSystemNode
    {
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
        /// Adds a single previously created node to the end of the list of subnodes.
        /// </summary>
        /// <param name="node"></param>
        void Add(IFileSystemNode node);

        /// <summary>
        /// Adds a list of previously created nodes to the end of the list of subnodes.
        /// </summary>
        /// <param name="node"></param>
        void Add(IFileSystemNode[] nodes);

        void MarkAsInaccessible();
    }
}
