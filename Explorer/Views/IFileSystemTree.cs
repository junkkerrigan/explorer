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
        void MountDrives(List<DriveNode> drives);

        //void ShowModal();
    }
}
