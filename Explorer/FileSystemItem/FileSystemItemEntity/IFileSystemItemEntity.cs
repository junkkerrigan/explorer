using System;
using System.Collections.Generic;

namespace Explorer
{
    public interface IFileSystemItemEntity 
    {
        string Path { get; set; }

        IFileSystemTreeNode Node { get; set; }

        void CopyTo(string destinationPath);

        void Delete();

        void EditName(string newName);

        void OpenWithDefaultApplication();

        void UpdatePath(string newPath);

        void Move(string destinationPath);
    }
}
