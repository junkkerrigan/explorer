using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public class FileNodePresenter : FileSystemTreeNodePresenter
    {
        public FileNodePresenter(IFileSystemTreeNode view) : base(view)
        {
        }

        public override void FillNode()
        {
            View.IsFilled = true;
        }
    }
}
