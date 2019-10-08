using System.Collections.Generic;
using Explorer.Views;

namespace Explorer.Presenters
{
    /// <summary>
    /// Provides an interface to interact with <see cref="FileSystemNodePresenter"/>.
    /// </summary>
    public interface IFileSystemNodePresenter
    {
        // TODO: comments

        /// <summary>
        /// Fills passed node with its subnodes.
        /// </summary>
        /// <param name="node"></param>
        void FillNode(IFileSystemNode node);

        /// <summary>
        /// Copies file system element from <paramref name="sourcePath"/> into <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        void CopyElement(string sourcePath, string destinationPath);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Source to be copied to buffer.</param>
        void CopyNodeToBuffer(IFileSystemNode source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Destination to be pasted from buffer.</param>
        void PasteNodeFromBuffer(IFileSystemNode destination);
    }
}
