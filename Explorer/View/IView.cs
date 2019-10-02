using System.Collections.Generic;

namespace Explorer.View
{
    public interface IView
    {
        void Mount();
    }

    public interface IFileSystemView : IView
    {
        void MountDrives(List<DriveNode> drives);

        void ShowModal();
    }
}
