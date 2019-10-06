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

        public FileSystemNodePresenter(IFileSystemNode view)
        {
            _view = view;
        }

        public void CopyNodeToBuffer(FileSystemNode source)
        {

        }

        public void PasteNodeFromBuffer(FileSystemNode destination)
        { 
            
        }
    }
}
