using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public class FolderItemPresenter : DirectoryItemPresenter
    {
        public FolderItemPresenter(IFileSystemTreeNode view) : base(view)
        {
        }
    }
}
