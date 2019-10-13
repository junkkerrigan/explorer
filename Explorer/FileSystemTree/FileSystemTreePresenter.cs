using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer
{
    public class FileSystemTreePresenter 
    {
        private readonly IFileSystemTree _view;

        public FileSystemTreePresenter(IFileSystemTree view)
        {
            _view = view;
        }

        public void LoadDrives()
        {
            List<IFileSystemTreeNode> driveNodes = new List<IFileSystemTreeNode>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo d in drives)
            {
                IFileSystemTreeNode dNode = IFileSystemTreeNode.Factory.GetNewDriveNode(d.Name);
                dNode.Entity.Path = d.Name;
                driveNodes.Add(dNode);
                dNode.Fill();
            }

            _view.MountDrives(driveNodes);
        }

        public void PreloadContent(IFileSystemTreeNode node)
        {
            foreach (IFileSystemTreeNode subNode in node.SubNodes)
            {
                subNode.Fill();
            }
        }
    }
}  
