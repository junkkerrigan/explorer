using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Explorer.Views;

namespace Explorer.Presenters
{
    public abstract class FileSystemTreeNodePresenter
    {
        // TODO: comments 
        // TODO: code splitting
        // TODO: async file copying
        // TODO: error modals
        // TODO: rename View
        protected IFileSystemTreeNode View { get; set; }

        /// <summary>
        /// Stores a clone of copied node.
        /// </summary>
        protected static IFileSystemTreeNode _buffer = null;

        protected Dictionary<string, Action> _contextMenuActions;

        public FileSystemTreeNodePresenter(IFileSystemTreeNode view) 
        {
            View = view;

            _contextMenuActions = new Dictionary<string, Action>
            {
                { "Open", View.Entity.OpenWithDefaultApplication },
                { "Copy", this.CopyNodeToBuffer },
                { "Cut", this.CutNodeToBuffer },
                { "Paste", this.PasteNodeFromBuffer },
                { "Delete", this.RemoveNode },
                { "Expand", 
                    () => {
                        View.Tree.List.Display(View);
                        View.Expand(); 
                    } 
                },
                { "Expand all", View.ExpandAll },
                { "Collapse", View.Collapse },
                { "Properties", View.ShowProperties },
                { "Rename", View.StartNameEditing },
                { "Create", View.StartNameEditing },
            };
        }

        protected void CopyNodeToBuffer()
        {
            _buffer = View;
        }

        protected void CutNodeToBuffer()
        {
            // TODO: handle pasting in same directory
            _buffer = View;
            View.Remove();
            View.Entity.Delete();
        }

        protected void PasteNodeFromBuffer()
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

            string newPath = Path.Combine(View.Entity.Path, _buffer.Name);
            IFileSystemTreeNode clone = _buffer.GetClone();
            View.AddSubNode(clone);
            View.SortSubNodes();

            try
            {
                _buffer.Entity.CopyTo(newPath);
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

        protected async void PasteNodeFromBufferAsync()
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

            string newPath = Path.Combine(View.Entity.Path, _buffer.Name);
            IFileSystemTreeNode clone = _buffer.GetClone();
            View.AddSubNode(clone);

            bool isCopied = await Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    _buffer.Entity.CopyTo(newPath);
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

        protected string[] GetSubFolders(IFileSystemTreeNode node)
        {
            string path = node.Entity.Path;
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

        protected string[] GetInnerFiles(IFileSystemTreeNode node)
        {
            string path = node.Entity.Path;
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
        protected void RemoveNode()
        {
            View.Entity.Delete();
            View.Remove();
        }
    }

    public class DirectoryNodePresenter : FileSystemTreeNodePresenter
    {
        public DirectoryNodePresenter(IFileSystemTreeNode view) : base(view) 
        {
        }

        public override void FillNode()
        {
            if (View.IsFilled || !View.IsAccessible) return;
            string[] subFolders = GetSubFolders(View);
            foreach (string folder in subFolders)
            {
                string name = folder.Substring(folder.LastIndexOf("\\") + 1);
                IFileSystemTreeNode folderNode = IFileSystemTreeNode.Factory.GetNewFolderNode(name);
                folderNode.Entity.Path = folder;
                View.AddSubNode(folderNode);
            }

            string[] innerFiles = GetInnerFiles(View);
            foreach (string file in innerFiles)
            {
                string name = file.Substring(file.LastIndexOf("\\") + 1);
                IFileSystemTreeNode fileNode = IFileSystemTreeNode.Factory.GetNewFileNode(name);
                fileNode.Entity.Path = file;
                View.AddSubNode(fileNode);
            }
            View.SortSubNodes();
            View.IsFilled = true;
        }
    }

    public class DriveNodePresenter : DirectoryNodePresenter
    {
        public DriveNodePresenter(IFileSystemTreeNode view) : base(view)
        {
            string[] contextMenuOptions =
            {
                "Paste", "Expand", "Expand all", "Collapse", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                View.AddContextMenuOption(option, _contextMenuActions[option]);
            }
        }
    }

    public class FolderNodePresenter : DirectoryNodePresenter
    {
        public FolderNodePresenter(IFileSystemTreeNode view) : base(view)
        {
            string[] contextMenuOptions =
            {
                "Copy", "Cut", "Paste", "Delete", "Rename", "Expand", "Expand all", 
                "Collapse", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                View.AddContextMenuOption(option, _contextMenuActions[option]);
            }
        }
    }

    public class FileNodePresenter : FileSystemTreeNodePresenter
    {
        public FileNodePresenter(IFileSystemTreeNode view) : base(view) 
        {
            string[] contextMenuOptions =
            {
                "Open", "Copy", "Cut", "Delete", "Rename", "Properties",
            };

            foreach (string option in contextMenuOptions)
            {
                View.AddContextMenuOption(option, _contextMenuActions[option]);
            }
        }

        public override void FillNode()
        {
            View.IsFilled = true;
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
