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
            string[] contextMenuOptions =
            {
                "Open", "Copy", "Cut", "Delete", "Rename", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                View.AddContextMenuOption(option, _contextMenuActions[option]);
            }
        }

        public override void FillNode()
        {
            View.IsFilled = true;
        }
    }
}
