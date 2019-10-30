using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public class FileItemPresenter : FileSystemItemPresenter
    {
        public FileItemPresenter(IFileSystemTreeNode view) : base(view)
        {
        }

        public override void FillNode()
        {
            View.IsFilled = true;
        }
    }
}
