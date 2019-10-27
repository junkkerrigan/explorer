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
            public static void CreateNewFolder(string path, string name)
            {
                try
                {
                    ValidateName(name);
                }
                catch
                {
                    throw;
                }

                string folderPath = System.IO.Path.Combine(path, name);

                if (Directory.Exists(folderPath))
                {
                    throw new DirectoryAlreadyExistsException();
                }
                else if (File.Exists(folderPath))
                {
                    throw new FileAlreadyExistsException();
                }

                Directory.CreateDirectory(folderPath);             
            }

            public static void CreateNewFile(string path, string name)
            {
                try
                {
                    ValidateName(name);
                }
                catch
                {
                    throw;
                }

                string filePath = System.IO.Path.Combine(path, name);

                if (Directory.Exists(filePath))
                {
                    throw new DirectoryAlreadyExistsException();
                }
                else if (File.Exists(filePath))
                {
                    throw new FileAlreadyExistsException();
                }

                File.Create(filePath).Close();
            }

            private static void ValidateName(string name)
            {
                if (name == ".")
                {
                    throw new ArgumentException(".");
                }

                string invalidChars = "\\/|:*<>\"?";

                foreach (char symbol in invalidChars)
                {
                    if (name.IndexOf(symbol) > -1)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        void EditName(string newName);

        void OpenWithDefaultApplication();

        void UpdatePath(string newPath);

        void Move(string destinationPath);

        void MergeTo(string filePath);
    }
}
