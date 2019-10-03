using System.Collections.Generic;

namespace Explorer.View
{
    public interface IView
    {
        void Mount();
    }

    /// <summary>
    /// Provides an interface to interact with <see cref="Explorer"/>.
    /// </summary>
    public interface IFileSystemView : IView
    {
        /// <summary>
        /// Loads drives into a <see cref="FileSystemTree"/>.
        /// </summary>
        /// <param name="drives"></param>
        void MountDrives(List<DriveNode> drives);

        //void ShowModal();
    }
}
