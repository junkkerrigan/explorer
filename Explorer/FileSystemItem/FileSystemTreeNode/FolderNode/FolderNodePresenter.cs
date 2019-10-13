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
            string[] contextMenuOptions =
            {
                "Copy", "Cut", "Paste", "Delete", "Rename", "Expand", "Expand all",
                "Collapse", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                View.AddContextMenuOption(option, _contextMenuActions[option]);
            }
        }
    }
}
