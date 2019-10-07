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
                DriveNode dNode = new DriveNode(d.Name)
                {
                    Path = d.Name,
                };
                driveNodes.Add(dNode);
                dNode.Fill();
            }

            _view.MountDrives(driveNodes);
        }

        public void PreloadContent(IFileSystemNode node)
        {
            Console.WriteLine($"Expanding node with path {node.Path}");
            foreach (IFileSystemNode subNode in node.SubNodes)
            {
                subNode.Fill();
            }
        }
    }
}  
