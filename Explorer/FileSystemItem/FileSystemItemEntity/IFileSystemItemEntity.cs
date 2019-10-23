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
                if (File.Exists(path))
                {
                    throw new FileAlreadyExistsException();
                }
                File.Create(path);
            }
        }

        void EditName(string newName);

        void OpenWithDefaultApplication();

        void UpdatePath(string newPath);

        void Move(string destinationPath);

        void MergeTo(string filePath);
    }
}
