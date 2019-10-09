using System.Collections.Generic;

namespace Explorer.Views
{
    /// <summary>
    /// Provides an interface to interact with <see cref="FileSystemTree"/>.
    /// </summary>
    public interface IFileSystemTree
    {
        /// <summary>
        /// Loads drives into a <see cref="FileSystemTree"/>.
        /// </summary>
        /// <param name="drives"></param>
        void MountDrives(List<IFileSystemNode> drives);

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

        //void ShowModal();
    }
}
