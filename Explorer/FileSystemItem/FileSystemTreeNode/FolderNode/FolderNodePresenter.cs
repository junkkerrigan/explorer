using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public class FolderNodePresenter : DirectoryNodePresenter
    {
        public FolderNodePresenter(IFileSystemTreeNode view) : base(view)
        {
        }
    }
}
