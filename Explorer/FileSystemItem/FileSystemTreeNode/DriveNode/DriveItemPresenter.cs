using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public class DriveItemPresenter : DirectoryItemPresenter
    {
        public DriveItemPresenter(IFileSystemTreeNode view) : base(view)
        {
        }
    }
}
