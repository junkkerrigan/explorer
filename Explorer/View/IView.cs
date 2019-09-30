using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.View
{
    public interface IView
    {
        void Mount();
    }

    public interface IFileSystemView : IView
    {
        void MountDrives(List<DriveNode> drives);
    }
}
