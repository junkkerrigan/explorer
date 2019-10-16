using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public interface IFileSystemItem
    {
        string Name { get; set; }

        IFileSystemItemEntity Entity { get; }

        void StartNameEditing();

        void ShowProperties();
    }
}
