using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public interface IFileSystemListItem : IFileSystemItem
    {
        IFileSystemTreeNode Node { get; }

        IFileSystemList List { get; }
    }
}
