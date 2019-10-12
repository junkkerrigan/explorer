using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Explorer.Views;

namespace Explorer.Presenters
{
    public abstract class FileSystemNodePresenter : IFileSystemNodePresenter
    {
        // TODO: comments 
        // TODO: code splitting
        // TODO: async file copying
        // TODO: error modals
        // TODO: rename View
        protected IFileSystemNode View { get; set; }

        /// <summary>
        /// Stores a clone of copied node.
        /// </summary>
        protected static IFileSystemNode _buffer = null;

        protected Dictionary<string, Action> _contextMenuActions;

        public FileSystemNodePresenter(IFileSystemNode view) 
        {
            View = view;

            _contextMenuActions = new Dictionary<string, Action>
            {
                { "Open", View.Element.OpenWithDefaultApplication },
                { "Copy", this.CopyNodeToBuffer },
                { "Cut", this.CutNodeToBuffer },
                { "Paste", this.PasteNodeFromBuffer },
                { "Delete", this.RemoveNode },
                { "Expand", View.Expand },
                { "Expand all", View.ExpandAll },
                { "Collapse", View.Collapse },
                { "Properties", View.ShowProperties },
                { "Rename", View.StartNameEditing },
                //{ "Move", View.StartNameEditing },
            };
        }

        public void CopyNodeToBuffer()
        {
            _buffer = View;
        }

        public void CutNodeToBuffer()
        {
            // TODO: handle pasting in same directory
            _buffer = View;
            View.Remove();
            View.Element.Delete();
        }

        public void PasteNodeFromBuffer()
        {
            if (View.IsChild(_buffer))
            {
                Console.WriteLine("Err: child");
                // TODO: ShowModal();
                return;
            }
            else if (View == _buffer)
            {
                Console.WriteLine("Err: same");
                // TODO: ShowModal();
                return;
            }

            string newPath = Path.Combine(View.Element.Path, _buffer.Text);
            IFileSystemNode clone = _buffer.GetClone();
            View.AddSubNode(clone);
            try
            {
                _buffer.Element.CopyTo(newPath);
            }
            catch (FileAlreadyExistsException)
            {
                Console.WriteLine("Err: file exists");
                // TODO: ShowModal();
                View.RemoveSubNode(clone);
            }
            catch (DirectoryAlreadyExistsException)
            {
                Console.WriteLine("Err: dir exists");
                // TODO: ShowModal();
                View.RemoveSubNode(clone);
            }
        }

        public async void PasteNodeFromBufferAsync()
        {
            if (View.IsChild(_buffer))
            {
                Console.WriteLine("Err: child");
                // TODO: ShowModal();
                return;
            }
            else if (View == _buffer)
            {
                Console.WriteLine("Err: same");
                // TODO: ShowModal();
                return;
            }

            string newPath = Path.Combine(View.Element.Path, _buffer.Text);
            IFileSystemNode clone = _buffer.GetClone();
            View.AddSubNode(clone);

            bool isCopied = await Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    _buffer.Element.CopyTo(newPath);
                }
                catch (FileAlreadyExistsException)
                {
                    Console.WriteLine("Err: file exists");
                    // TODO: ShowModal();
                    return false;
                }
                catch (DirectoryAlreadyExistsException)
                {
                    Console.WriteLine("Err: dir exists");
                    // TODO: ShowModal();
                    return false;
                }
                return true;
            });

            if (!isCopied)
            {
                View.RemoveSubNode(clone);
            }
        }

        public abstract void FillNode();

        protected string[] GetSubFolders(IFileSystemNode node)
        {
            string path = node.Element.Path;
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

        protected string[] GetInnerFiles(IFileSystemNode node)
        {
            string path = node.Element.Path;
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

        // TODO: ShowModalReallyDelete()
        public void RemoveNode()
        {
            View.Element.Delete();
            View.Remove();
        }
    }

    public class DirectoryNodePresenter : FileSystemNodePresenter
    {
        public DirectoryNodePresenter(IFileSystemNode view) : base(view) 
        {
        }

        public override void FillNode()
        {
            if (View.IsFilled || !View.IsAccessible) return;
            string[] subFolders = GetSubFolders(View);
            foreach (string folder in subFolders)
            {
                string name = folder.Substring(folder.LastIndexOf("\\") + 1);
                IFileSystemNode folderNode = IFileSystemNode.Factory.GetNewFolderNode(name);
                folderNode.Element.Path = folder;
                View.AddSubNode(folderNode);
            }

            string[] innerFiles = GetInnerFiles(View);
            foreach (string file in innerFiles)
            {
                string name = file.Substring(file.LastIndexOf("\\") + 1);
                IFileSystemNode fileNode = IFileSystemNode.Factory.GetNewFileNode(name);
                fileNode.Element.Path = file;
                View.AddSubNode(fileNode);
            }
            View.IsFilled = true;
        }
    }

    public class DriveNodePresenter : DirectoryNodePresenter
    {
        public DriveNodePresenter(IFileSystemNode view) : base(view)
        {
            string[] contextMenuOptions =
            {
                "Paste", "Expand", "Expand all", "Collapse", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                View.AddContextMenuItem(option, _contextMenuActions[option]);
            }
        }
    }

    public class FolderNodePresenter : DirectoryNodePresenter
    {
        public FolderNodePresenter(IFileSystemNode view) : base(view)
        {
            string[] contextMenuOptions =
            {
                "Copy", "Cut", "Paste", "Delete", "Rename", "Expand", "Expand all", 
                "Collapse", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                View.AddContextMenuItem(option, _contextMenuActions[option]);
            }
        }
    }

    public class FileNodePresenter : FileSystemNodePresenter
    {
        public FileNodePresenter(IFileSystemNode view) : base(view) 
        {
            string[] contextMenuOptions =
            {
                "Open", "Copy", "Cut", "Delete", "Rename", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                View.AddContextMenuItem(option, _contextMenuActions[option]);
            }
        }

        public override void FillNode()
        {
            throw new NotSupportedException("Error: impossible to fill FileNode");
        }
    }

    public abstract class AlreadyExistsException : IOException
    {
        public AlreadyExistsException()
        {
        }

        public AlreadyExistsException(string message) : base(message)
        {
        }

        public AlreadyExistsException(string message, Exception innerEx) :
            base(message, innerEx)
        {
        }
    }

    public class FileAlreadyExistsException : AlreadyExistsException
    {
        public FileAlreadyExistsException() 
        {
        }

        public FileAlreadyExistsException(string message) : base(message) 
        {
        }

        public FileAlreadyExistsException(string message, Exception innerEx) :
            base(message, innerEx) 
        { 
        }
    }

    public class DirectoryAlreadyExistsException : AlreadyExistsException
    {
        public DirectoryAlreadyExistsException()
        {
        }

        public DirectoryAlreadyExistsException(string message) : base(message)
        {
        }

        public DirectoryAlreadyExistsException(string message, Exception innerEx) :
            base(message, innerEx)
        {
        }
    }
}
