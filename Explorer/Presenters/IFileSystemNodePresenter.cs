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
        /// Fills <paramref name="node"/> with its subnodes.
        /// </summary>
        /// <param name="node"></param>
        void FillNode(IFileSystemNode node);

        void OpenWithDefaultApplication(); // IFSE

        /// <summary>
        /// Copies file system element from <paramref name="sourcePath"/> into <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        void CopyElement(string sourcePath, string destinationPath);  // IFSE

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Source to be copied to buffer.</param>
        void CopyNodeToBuffer();

        void CutNode();  // IFSE

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Destination to be pasted from buffer.</param>
        void PasteNodeFromBufferAsync();  // IFSE

        void DeleteNode();

        void ExpandNode();

        void ExpandAllSubNodes();

        void CollapseNode();

        void RenameNode();

        

    }
}
