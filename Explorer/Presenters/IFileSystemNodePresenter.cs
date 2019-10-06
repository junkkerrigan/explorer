using System.Collections.Generic;
using Explorer.Views;

namespace Explorer.Presenters
{
    /// <summary>
    /// Provides an interface to interact with <see cref="FileSystemNodePresenter"/>.
    /// </summary>
    public interface IFileSystemNodePresenter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Node that will be copied to buffer</param>
        void CopyNodeToBuffer(FileSystemNode source);

        void PasteNodeFromBuffer(FileSystemNode destination);
    }
}
