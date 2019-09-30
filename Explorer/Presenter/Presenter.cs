using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.View;

namespace Explorer.Presenter
{
    class Presenter : IPresenter
    {
        private readonly IFileSystemView _view;

        public Presenter(IFileSystemView view)
        {
            _view = view;
        }

        public void Run()
        {
            _view.Mount();
        }

        public void LoadDrives()
        {
            var drives = DriveInfo.GetDrives();

            var driveNodes = new List<DriveNode>();
            foreach (DriveInfo d in drives)
            {
                var newNode = new DriveNode(d.Name + "         ");
                driveNodes.Add(newNode);
                if (!Directory.Exists(d.Name)) continue;
                FillNode(newNode, d.Name);
            }

            _view.MountDrives(driveNodes);
        }

        public void FillNode(FileSystemNode node, string path)
        {

            try
            {
                var innerFiles = Directory.GetFileSystemEntries(path);

                foreach (var file in innerFiles)
                {
                    Console.WriteLine(file);
                    var newNode = new FileSystemNode(file);
                    node.Nodes.Add(newNode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FillNode");
                Console.WriteLine(ex.Message);
            }
        }
    }
}