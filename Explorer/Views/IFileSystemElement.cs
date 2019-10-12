using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Views
{
    public interface IFileSystemElement 
    {
        string Path { get; set; }

        IFileSystemNode Node { get; set; }

        void CopyTo(string destinationPath);

        void Delete();

        void EditName(string newName);

        void OpenWithDefaultApplication();

        void UpdatePath(string newPath);

        void Move(string destinationPath);
    }
}
