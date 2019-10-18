using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public interface IFileSystemList
    {
        IFileSystemTree Tree { get; set; }

        IFileSystemTreeNode DisplayedNode { get; }

        IFileSystemListItem DisplayedItem { get; set; }

        void Display(IFileSystemTreeNode node);

        void Display(IFileSystemListItem item);
      
        void DisplayTree();

        void AddItem(IFileSystemListItem item);

        void AddItems(List<IFileSystemListItem> item);

        void StartMoving();

        void FinishMoving();
    }
}
