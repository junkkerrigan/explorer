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

        SearchBox SearchBox { get; set; }

        List<IFileSystemListItem> RootItems { get; }

        IFileSystemListItem SelectedItem { get; }

        IFileSystemTreeNode DisplayedNode { get; }

        IFileSystemListItem DisplayedItem { get; set; }

        void Display(IFileSystemTreeNode node);

        void Display(IFileSystemListItem item);

        void UpdateRefresh();

        void DisplayTree();

        void AddItem(IFileSystemListItem item);

        void AddItems(List<IFileSystemListItem> item);

        void StartMoving();

        void StartMerging(IFileSystemTreeNode node);

        void FinishMoving();
    }
}
