using System;
using System.Collections.Generic;
using System.IO;

namespace Explorer
{
    public interface IFileSystemItemEntity 
    {
        string Path { get; set; }

        IFileSystemTreeNode Node { get; set; }

        void CopyTo(string destinationPath);

        void Delete();

        static class Factory
        {
            public static void CreateNewFolder(string path)
            {
                
                if (Directory.Exists(path))
                {
                    throw new DirectoryAlreadyExistsException();
                }
                Directory.CreateDirectory(path);
            }

            public static void CreateNewFile(string path)
            {
                try
                {
                    File.Create(path);
                }
                catch (IOException)
                {
                    throw new FileAlreadyExistsException();
                }
            }
        }

        void EditName(string newName);

        void OpenWithDefaultApplication();

        void UpdatePath(string newPath);

        void Move(string destinationPath);
    }
}
