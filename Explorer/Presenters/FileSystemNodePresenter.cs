using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Explorer.Views;

namespace Explorer.Presenters
{
    public abstract class FileSystemNodePresenter : IFileSystemNodePresenter
    {
        protected IFileSystemNode View { get; set; }

        /// <summary>
        /// Stores a clone of copied node.
        /// </summary>
        protected static IFileSystemNode _buffer = null;

        public FileSystemNodePresenter(IFileSystemNode view) 
        {
            View = view;
        }

        public void CopyNodeToBuffer(IFileSystemNode source)
        {
            _buffer = source;
        }

        public void PasteNodeFromBuffer(IFileSystemNode destination)
        {
            if (destination.IsChild(_buffer))
            {
                Console.WriteLine("Err: child");
                // ShowModal();
                return;
            }
            else if (destination == _buffer)
            {
                Console.WriteLine("Err: same");
                // ShowModal();
                return;
            }
            else if (_buffer.IsDirectChild(destination))
            {
                Console.WriteLine("Err: parent");
                // ShowModal();
                return;
            }

            string newPath = Path.Combine(destination.Path, _buffer.Text);

            try
            {
                _buffer.CopyTo(newPath);
                IFileSystemNode clone = _buffer.GetClone();
                destination.AddNode(clone);
            }
            catch (FileAlreadyExistsException)
            {
                Console.WriteLine("Err: file exists");
                // ShowModal();
            }
            catch (DirectoryAlreadyExistsException)
            {
                Console.WriteLine("Err: dir exists");
                // ShowModal();
            }
        }

        public abstract void CopyElement(string sourcePath, string destinationPath);

        // TODO: override in childs
        public void FillNode(IFileSystemNode node)
        {
            if (node is FileNode || node.Filled || !node.Accessible) return;
            string[] subFolders = GetSubFolders(node);
            foreach (string folder in subFolders)
            {
                string name = folder.Substring(folder.LastIndexOf("\\") + 1);
                FolderNode folderNode = new FolderNode(name)
                {
                    Path = folder,
                };
                node.AddNode(folderNode);
            }

            string[] innerFiles = GetInnerFiles(node);
            foreach (string file in innerFiles)
            {
                string name = file.Substring(file.LastIndexOf("\\") + 1);
                FileNode fileNode = new FileNode(name)
                {
                    Path = file,
                };
                node.AddNode(fileNode);
            }
            node.Filled = true;
        }

        protected string[] GetSubFolders(IFileSystemNode node)
        {
            string path = node.Path;
            string[] subFolders = { };

            try
            {
                subFolders = Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is IOException)
                {
                    node.MarkAsInaccessible();
                }
                else
                {
                    Console.WriteLine("Error in GetSubFolders");
                    Console.WriteLine(ex.Message);
                }
            }

            return subFolders;
        }

        protected string[] GetInnerFiles(IFileSystemNode node)
        {
            string path = node.Path;
            string[] innerFiles = { };
                
            try
            {
                innerFiles = Directory.GetFiles(path);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException || ex is IOException)
                {
                    node.MarkAsInaccessible();
                }
                else
                {
                    Console.WriteLine("Error in GetInnerFiles");
                    Console.WriteLine(ex.Message);
                }
            }

            return innerFiles;
        }
    }

    public class DirectoryNodePresenter : FileSystemNodePresenter
    {
        public DirectoryNodePresenter(IFileSystemNode view) : base(view) 
        {
        }

        public override void CopyElement(string sourcePath, string destinationPath)
        {
            Console.WriteLine($"Copying dir from {sourcePath} to {destinationPath}");
            if (Directory.Exists(destinationPath))
            {
                throw new DirectoryAlreadyExistsException();
            }
            Directory.CreateDirectory(destinationPath);

            string[] subDirs = Directory.GetDirectories(sourcePath);
            foreach (string dir in subDirs)
            {
                string folderName = dir.Substring(dir.LastIndexOf('\\') + 1),
                    newDestPath = Path.Combine(destinationPath, folderName);
                CopyElement(dir, newDestPath);
            }

            string[] innerFiles = Directory.GetFiles(sourcePath);
            foreach (string file in innerFiles)
            {
                string fileName = Path.GetFileName(file),
                    destFileName = Path.Combine(destinationPath, fileName);
                Console.WriteLine($"Copying file from {file} to {destFileName}");
                File.Copy(file, destFileName);
            }
        }
    }

    public class FileNodePresenter : FileSystemNodePresenter
    {
        public FileNodePresenter(IFileSystemNode view) : base(view) 
        {
        }

        public override void CopyElement(string sourcePath, string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                throw new FileAlreadyExistsException();
            }
            File.Copy(sourcePath, destinationPath);
        }
    }

    public class FileAlreadyExistsException : IOException
    {
        public FileAlreadyExistsException() 
        {
        }

        public FileAlreadyExistsException(string message) : base(message) 
        {
        }

        public FileAlreadyExistsException(string message, Exception innerEx) :
            base(message, innerEx) 
        { 
        }
    }

    public class DirectoryAlreadyExistsException : IOException
    {
        public DirectoryAlreadyExistsException()
        {
        }

        public DirectoryAlreadyExistsException(string message) : base(message)
        {
        }

        public DirectoryAlreadyExistsException(string message, Exception innerEx) :
            base(message, innerEx)
        {
        }
    }
}
