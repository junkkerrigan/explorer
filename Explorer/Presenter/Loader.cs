using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.View;

namespace Explorer.Presenter
{
    /// <summary>
    /// Provides a static method to load inner elements .
    /// </summary>
    static class Loader
    {
        /// <summary>
        /// Loads system drives.
        /// </summary>
        /// <returns></returns>
        public static List<DriveNode> GetDrives()
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
                FillDirectory(dNode);
            }

            return driveNodes;
        }

        public static void LoadSubDirs(FileSystemNode node)
        {
            foreach (FileSystemNode subNode in node.Nodes)
            {
                if (subNode is FileNode || subNode.Filled) return;
                FillDirectory(subNode);
                subNode.Filled = true;
            }
        }

        /// <summary>
        /// Loads all subnodes of passed node.
        /// </summary>
        /// <param name="node"></param>
        public static void FillDirectory(FileSystemNode node)
        {
            //Console.WriteLine(node.Path);
            string[] subFolders = GetSubFolders(node);
            foreach (string folder in subFolders)
            {
                string name = folder.Substring(folder.LastIndexOf("\\") + 1);
                FolderNode folderNode = new FolderNode(name)
                {
                    Path = folder,
                };
                node.Nodes.Add(folderNode);
                //FillDirectory(folderNode);
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
                    Console.WriteLine("Error in FillNode");
                    Console.WriteLine(ex.Message);
                }
            }

            return subFolders;
        }

        private static string[] GetInnerFiles(FileSystemNode node)
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
                    Console.WriteLine("Error in FillNode");
                    Console.WriteLine(ex.Message);
                }
            }

            return innerFiles;
        }
    }
}
