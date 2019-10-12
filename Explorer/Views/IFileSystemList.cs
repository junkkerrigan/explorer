using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Views
{
    public interface IFileSystemList
    {
        void Display(IFileSystemTreeNode node);
      
        void Display(IFileSystemTree tree);

        void AddItem(IFileSystemListItem item);

        void AddItems(List<IFileSystemListItem> item);
    }
}
