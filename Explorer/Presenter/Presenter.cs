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
            _view.MountDrives(Loader.GetDrives());
        }
        
        public void LoadSubDirectories(FileSystemNode node)
        {
            Loader.LoadSubDirs(node);
        }
    }
}