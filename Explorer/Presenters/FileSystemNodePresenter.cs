using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Explorer.Views;

namespace Explorer.Presenters
{
    public abstract class FileSystemNodePresenter : IFileSystemNodePresenter
    {
        // TODO: comments 
        // TODO: code splitting
        // TODO: async file copying
        // TODO: error modals
        // TODO: rename View
        protected IFileSystemNode View { get; set; }

        /// <summary>
        /// Stores a clone of copied node.
        /// </summary>
        protected static IFileSystemNode _buffer = null;

        public FileSystemNodePresenter(IFileSystemNode view) 
        {
            View = view;
        }

        public void CopyNodeToBuffer()
        {
            _buffer = View;
        }

        public async void PasteNodeFromBufferAsync()
        {
            if (View.IsChild(_buffer))
            {
                Console.WriteLine("Err: child");
                // ShowModal();
                return;
            }
            else if (View == _buffer)
            {
                Console.WriteLine("Err: same");
                // ShowModal();
                return;
            }

            string newPath = Path.Combine(View.Path, _buffer.Text);
            IFileSystemNode clone = _buffer.GetClone();
            View.AddNode(clone);

            bool isCopied = await Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    _buffer.CopyTo(newPath);
                }
                catch (FileAlreadyExistsException)
                {
                    Console.WriteLine("Err: file exists");
                    // ShowModal();
                    return false;
                }
                catch (DirectoryAlreadyExistsException)
                {
                    Console.WriteLine("Err: dir exists");
                    // ShowModal();
                    return false;
                }
                return true;
            });

            if (!isCopied)
            {
                View.RemoveNode(clone);
            }
        }

        public abstract void CopyElement(string sourcePath, string destinationPath);

        public virtual void FillNode(IFileSystemNode node)
        {
            if (node.Filled || !node.Accessible) return;
            string[] subFolders = GetSubFolders(node);
            foreach (string folder in subFolders)
            {
                string name = folder.Substring(folder.LastIndexOf("\\") + 1);
                IFileSystemNode folderNode = IFileSystemNode.NodeFactory.GetNewFolderNode(name);
                folderNode.Path = folder;
                node.AddNode(folderNode);
            }

            string[] innerFiles = GetInnerFiles(node);
            foreach (string file in innerFiles)
            {
                string name = file.Substring(file.LastIndexOf("\\") + 1);
                IFileSystemNode fileNode = IFileSystemNode.NodeFactory.GetNewFileNode(name);
                fileNode.Path = file;
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
                File.Copy(file, destFileName);
            }

            // ShowModalWhenFinished();
        }
    }

    public class FileNodePresenter : FileSystemNodePresenter
    {
        public FileNodePresenter(IFileSystemNode view) : base(view) 
        {
        }

        public override void FillNode(IFileSystemNode node)
        {
            node.Filled = true;
            return;
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

    public abstract class AlreadyExistsException : IOException
    {
        public AlreadyExistsException()
        {
        }

        public AlreadyExistsException(string message) : base(message)
        {
        }

        public AlreadyExistsException(string message, Exception innerEx) :
            base(message, innerEx)
        {
        }
    }

    public class FileAlreadyExistsException : AlreadyExistsException
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

    public class DirectoryAlreadyExistsException : AlreadyExistsException
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
