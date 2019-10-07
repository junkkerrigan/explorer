using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Explorer.Views;

namespace Explorer.Presenters
{
    public class FileSystemNodePresenter : IFileSystemNodePresenter
    {
        private readonly IFileSystemNode _view;

        /// <summary>
        /// Stores a clone of copied node.
        /// </summary>
        private static IFileSystemNode _buffer = null;

        public FileSystemNodePresenter(IFileSystemNode view)
        {
            _view = view;
        }

        public void CopyNodeToBuffer(IFileSystemNode source)
        {
            _buffer = source;
            Console.WriteLine($"Copying to buffer: buffer path is {_buffer.Path}");
            Console.WriteLine($"Copying to buffer: source path is {source.Path}");

            foreach (var node in _buffer.SubNodes)
            {
                Console.WriteLine($"Copying to buffer: buffer subnode path is {node.Path}");
            }

            foreach (var node in source.SubNodes)
            {
                Console.WriteLine($"Copying to buffer: source subnode path is {node.Path}");
            }
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

            IFileSystemNode clone = _buffer.GetClone();

            foreach (var node in clone.SubNodes)
            {
                Console.WriteLine($"Pasting: buffer clone subnode path is {node.Path}");
            }

            if (clone is FileNode)
            {
                string newFilePath = Path.Combine(destination.Path, clone.Text);
                CopyFile(clone.Path, newFilePath);
                clone.Path = newFilePath;
                destination.AddNode(clone);
            }
            else if (clone is FolderNode)
            {
                string newFolderPath = Path.Combine(destination.Path, clone.Text);
                CopyFolder(clone.Path, newFolderPath);
                clone.Path = newFolderPath;
                foreach(IFileSystemNode node in clone.SubNodes)
                {
                    node.Path = Path.Combine(newFolderPath, node.Text);
                }
                destination.AddNode(clone);
            }

            Console.WriteLine($"Cloned node: new path is {clone.Path}");
            foreach (var node in clone.SubNodes)
            { 
                Console.WriteLine($"Subnode of cloned node: new path is {node.Path}");
            }
            // in CopyFolder destPath = dest.Path + newDirName
        }

        private void CopyFile(string sourcePath, string destinationPath) 
        {
            File.Copy(sourcePath, destinationPath);
        }

        private void CopyFolder(string sourcePath, string destinationPath)
        {
            Console.WriteLine($"Copying dir from {sourcePath} to {destinationPath}");
            Directory.CreateDirectory(destinationPath);

            string[] subDirs = Directory.GetDirectories(sourcePath);
            foreach (string dir in subDirs)
            {
                string folderName = dir.Substring(dir.LastIndexOf('\\') + 1),
                    newDestPath = Path.Combine(destinationPath, folderName);
                CopyFolder(dir, newDestPath);
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

        private string[] GetSubFolders(IFileSystemNode node)
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

        private string[] GetInnerFiles(IFileSystemNode node)
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
}
