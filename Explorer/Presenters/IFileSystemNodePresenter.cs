using System.Collections.Generic;
using Explorer.Views;

namespace Explorer.Presenters
{
    /// <summary>
    /// Provides an interface to interact with <see cref="FileSystemNodePresenter"/>.
    /// </summary>
    public interface IFileSystemNodePresenter
    {
        void FillNode(IFileSystemNode node);
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
