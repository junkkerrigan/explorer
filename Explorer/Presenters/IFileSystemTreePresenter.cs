using System.Collections.Generic;
using Explorer.Views;

namespace Explorer.Presenters
{
    /// <summary>
    /// Provides an interface to interact with <see cref="FileSystemTreePresenter"/>.
    /// </summary>
    public interface IFileSystemTreePresenter
    {
        /// <summary>
        /// Loads system drives.
        /// </summary>
        void LoadDrives();

        /// <summary>
        /// Loads subnodes of subnodes.
        /// </summary>
        /// <param name="node"></param>
        void FillNode(FileSystemNode node);
    }
}
