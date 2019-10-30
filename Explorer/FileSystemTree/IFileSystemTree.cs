﻿using System.Collections.Generic;

namespace Explorer
{
    /// <summary>
    /// Provides an interface to interact with <see cref="FileSystemTree"/>.
    /// </summary>
    public interface IFileSystemTree
    {
        List<IFileSystemTreeNode> RootNodes { get; }

        IFileSystemList List { get; }

        void DisplayOnListView();

        /// <summary>
        /// Loads drives into a <see cref="FileSystemTree"/>.
        /// </summary>
        /// <param name="drives"></param>
        void MountDrives(List<IFileSystemTreeNode> drives);

        /// <summary>
        /// Adds a single previously created node to the end of the list of subnodes
        /// and assigns its Path property depending on Path of this node.
        /// </summary>
        /// <param name="node"></param>
        void AddNode(IFileSystemTreeNode node);

        /// <summary>
        /// Adds a list of previously created nodes to the end of the list of subnodes.
        /// </summary>
        /// <param name="node"></param>
        void AddNodes(List<IFileSystemTreeNode> nodes);

        void BeginUpdate();

        void EndUpdate();
    }
}
