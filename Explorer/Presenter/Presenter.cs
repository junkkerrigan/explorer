using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Explorer.View;

namespace Explorer.Presenter
{
    /// <summary>
    /// Implements <see cref="IPresenter"/>.
    /// </summary>
    class Presenter : IPresenter
    {
        private readonly IFileSystemView _view;

        /// <summary>
        /// Initializes a new instance of <see cref="Presenter"/>.
        /// </summary>
        /// <param name="view"></param>
        public Presenter(IFileSystemView view)
        {
            _view = view;
        }

        public void Run()
        {
            _view.Mount();
        }

        //public void CheckAccessibility(FileSystemNode node)
        //{
        //    //if (!node.IsAccessible) _view.ShowModal();
        //}

        public void LoadDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            List<DriveNode> driveNodes = new List<DriveNode>();
            foreach (DriveInfo d in drives)
            {
                DriveNode dNode = new DriveNode(d.Name + "         ")
                {
                    Path = d.Name,
                };
                driveNodes.Add(dNode);
                FillNode(dNode);
            }

            _view.MountDrives(driveNodes);
        }

        public void LoadSubdirs(FileSystemNode node)
        {
            if (!node.IsFilled)
            {
                foreach (FileSystemNode subNode in node.Nodes)
                {
                    if (subNode is FolderNode && Directory.Exists(subNode.Path))
                    {
                        string path = subNode.Path;
                        FillNode(subNode);
                    }
                }
                node.IsFilled = true;
            }
        }

        private void FillNode(FileSystemNode node)
        {
            List<string> InaccessibleDirectories = new List<string>(); 
            try
            {
                string path = node.Path;
                string[] subDirs = Directory.GetDirectories(path);
                foreach (string dir in subDirs)
                {
                    string dirName = dir.Substring(dir.LastIndexOf("\\") + 1);
                    FolderNode dirNode = new FolderNode(dirName)
                    {
                        Path = dir,
                    };
                    node.Nodes.Add(dirNode);
                }

                string[] innerFiles = Directory.GetFiles(path);
                foreach (string file in innerFiles)
                {
                    string fileName = file.Substring(file.LastIndexOf("\\") + 1);
                    FileNode fileNode = new FileNode(fileName)
                    {
                        Path = file
                    };
                    node.Nodes.Add(fileNode);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                string m = ex.Message;
                int start = m.IndexOf("\"") + 1,
                    len = m.LastIndexOf("\"") - m.IndexOf("\"") - 1;
                string dirName = m.Substring(start, len);
                if (!InaccessibleDirectories.Contains(dirName))
                {
                    InaccessibleDirectories.Add(dirName);
                }
            }
            catch (IOException)
            {
                if (!InaccessibleDirectories.Contains(node.Path))
                {
                    InaccessibleDirectories.Add(node.Path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FillNode");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (InaccessibleDirectories.Contains(node.Path))
                {
                    node.IsAccessible = false;
                    node.ForeColor = Color.Gray;
                }
            }
        }
    }
}