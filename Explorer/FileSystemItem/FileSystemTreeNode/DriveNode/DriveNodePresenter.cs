using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public class DriveNodePresenter : DirectoryNodePresenter
    {
        public DriveNodePresenter(IFileSystemTreeNode view) : base(view)
        {
            string[] contextMenuOptions =
            {
                "Paste", "Expand", "Expand all", "Collapse", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                View.AddContextMenuOption(option, _contextMenuActions[option]);
            }
        }
    }
}
