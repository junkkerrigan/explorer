using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Explorer.Views;

namespace Explorer.Presenters
{
    public class FileSystemTreePresenter : IFileSystemTreePresenter
    {
        private readonly IFileSystemTree _view;

        public FileSystemTreePresenter(IFileSystemTree view)
        {
            _view = view;
        }

        public void LoadDrives()
        {
            List<DriveNode> driveNodes = new List<DriveNode>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo d in drives)
            {
                DriveNode dNode = new DriveNode(d.Name + "         ")
                {
                    Path = d.Name,
                };
                driveNodes.Add(dNode);
                FillSubNode(dNode);
            }

            _view.MountDrives(driveNodes);
        }

        public void FillNode(FileSystemNode node)
        {
            foreach (FileSystemNode subNode in node.Nodes)
            {
                if (subNode is FileNode || subNode.Filled) return;
                FillSubNode(subNode);
                subNode.Filled = true;
            }
        }

        private static void FillSubNode(FileSystemNode node)
        {
            string[] subFolders = GetSubFolders(node);
            foreach (string folder in subFolders)
            {
                string name = folder.Substring(folder.LastIndexOf("\\") + 1);
                FolderNode folderNode = new FolderNode(name)
                {
                    Path = folder,
                };
                node.Nodes.Add(folderNode);
            }

            string[] innerFiles = GetInnerFiles(node);
            foreach (string file in innerFiles)
            {
                string name = file.Substring(file.LastIndexOf("\\") + 1);
                FileNode fileNode = new FileNode(name)
                {
                    Path = file,
                };
                node.Nodes.Add(fileNode);
            }
        }

        private static string[] GetSubFolders(FileSystemNode node)
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

        private static string[] GetInnerFiles(FileSystemNode node)
        {
            //Console.WriteLine(node.Path == null);
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

    /// <summary>
    /// Provides static methods to load system elements.
    /// </summary>
    static class Loader
    {
        /// <summary>
        /// Loads system drives.
        /// </summary>
        /// <returns></returns>
        
}
