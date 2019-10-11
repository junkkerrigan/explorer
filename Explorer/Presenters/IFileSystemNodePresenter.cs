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
        /// Fills node with its subnodes.
        /// </summary>
        void FillNode();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Source to be copied to buffer.</param>
        void CopyNodeToBuffer();

        void CutNodeToBuffer();  // IFSE

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Destination to be pasted from buffer.</param>
        void PasteNodeFromBufferAsync();  // IFSE

        void RemoveNode(); 
    }
}
