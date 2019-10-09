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
            List<IFileSystemNode> driveNodes = new List<IFileSystemNode>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo d in drives)
            {
                IFileSystemNode dNode = IFileSystemNode.NodeFactory.GetNewDriveNode(d.Name);
                dNode.Path = d.Name;
                driveNodes.Add(dNode);
                dNode.Fill();
            }

            _view.MountDrives(driveNodes);
        }

        public void PreloadContent(IFileSystemNode node)
        {
            foreach (IFileSystemNode subNode in node.SubNodes)
            {
                subNode.Fill();
            }
        }
    }
}  
