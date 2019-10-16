using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer
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
        protected static IFileSystemTreeNode _clone = null;

        /// <summary>
        /// Stores a copied node.
        /// </summary>
        protected static IFileSystemTreeNode _buffer = null;

        protected Dictionary<string, Action> _contextMenuActions;

        public FileSystemTreeNodePresenter(IFileSystemTreeNode view) 
        {
            View = view;

            _contextMenuActions = new Dictionary<string, Action>
            {
                { "Open", new Action(() => View.Open()) }, 
                // because View.Open will be initialized later in constructors 
                // of derived classes
                { "Copy", this.CopyNodeToBuffer },
                { "Cut", this.CutNodeToBuffer },
                { "Paste", this.PasteNodeFromBuffer },
                { "Delete", this.RemoveNode },
                { "Expand", View.Expand },
                { "Expand all", View.ExpandAll },
                { "Collapse", View.Collapse },
                { "Properties", View.ShowProperties },
                { "Rename", View.StartNameEditing },
            };
        }

        public void HandleContextMenuAction(string name)
        {
            _contextMenuActions[name]();
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
            if (_clone == null)
            {
                _clone = _buffer.GetClone();
                Console.WriteLine(_clone.Name);

            }
            View.AddSubNode(_clone);

            View.SortSubNodes(false);

            bool isCopyingSucceed = true;

            try
            {
                _buffer.Entity.CopyTo(newPath);
            }
            catch (FileAlreadyExistsException)
            {
                Console.WriteLine("Err: file exists");
                // TODO: ShowModal();
                isCopyingSucceed = false;
            }
            catch (DirectoryAlreadyExistsException)
            {
                Console.WriteLine("Err: dir exists");
                // TODO: ShowModal();
                isCopyingSucceed = false;
            }
            finally
            {
                if (!isCopyingSucceed)
                {
                    View.RemoveSubNode(_clone);
                }
                else
                {
                    _clone.MarkAsSelected();
                    _clone = null;
                }
                View.Expand();
                View.DisplayOnListView();
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
            IFileSystemTreeNode parent = View.Parent;
            View.Entity.Delete();
            View.Remove();
            parent.DisplayOnListView();
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
            View.SortSubNodes(false);
            View.IsFilled = true;
        }
    }
}
