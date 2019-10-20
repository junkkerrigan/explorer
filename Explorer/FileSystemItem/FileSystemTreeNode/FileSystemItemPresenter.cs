using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer
{
    public abstract class FileSystemItemPresenter
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

        public IFileSystemTreeNode Buffer 
        { 
            get
            {
                return _buffer;
            }
        }

        /// <summary>
        /// Stores parent of a cut node
        /// </summary>
        protected static IFileSystemTreeNode _parent = null;

        protected static int _bufferElementState = BufferElementState.Empty;

        protected Dictionary<string, Action> _nodeContextMenuActions;

        protected Dictionary<string, Action> _listItemContextMenuActions;

        public FileSystemItemPresenter(IFileSystemTreeNode view) 
        {
            View = view;

            View.ListItem.Presenter = this;

            _listItemContextMenuActions = new Dictionary<string, Action>
            {
                { "Open", new Action(() => View.ListItem.Open()) }, 
                // because View.Open will be initialized later in constructors 
                // of derived classes
                { "Copy", this.CopyNodeToBuffer },
                { "Cut", this.CutNodeToBuffer },
                { "Undo cut", new Action(() => this.UndoCut(true)) },
                { "Paste", this.PasteNodeFromBuffer },
                { "Move", () =>
                    {
                        if (_bufferElementState == BufferElementState.Moving)
                        {
                            MessageBox.Show(
                                $"Impossible to move {View.Name} while {_buffer.Name} is moving",
                                "Moving error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (_bufferElementState == BufferElementState.Cut)
                        {
                            this.UndoCut();
                        }

                        this.CutNodeToBuffer();

                        _parent.Tree.List.StartMoving();
                        _parent.DisplayOnListView();

                        _bufferElementState = BufferElementState.Moving;
                    }
                },
                { "Move here", this.MoveNode },
                { "Cancel moving", () =>
                    {
                        this.UndoCut(true);

                        View.Tree.List.FinishMoving();

                        _bufferElementState = BufferElementState.Empty;
                    }
                },
                { "Delete", this.RemoveNode },
                { "Properties", View.ListItem.ShowProperties },
                { "Rename", View.ListItem.StartNameEditing },
                { "Create folder", () =>
                    {
                        IFileSystemTreeNode newFolder =
                            IFileSystemTreeNode.Factory.GetNewFolderNode("");

                        View.AddSubNode(newFolder);
                        View.Collapse();
                        View.DisplayOnListView();

                        newFolder.ListItem.StartNameEditing();
                    }
                },
                { "Create file", () =>
                    {
                        IFileSystemTreeNode newFolder =
                            IFileSystemTreeNode.Factory.GetNewFileNode("");

                        View.AddSubNode(newFolder);
                        View.Collapse();
                        View.DisplayOnListView();

                        newFolder.ListItem.StartNameEditing();
                    }
                },
            };

            _nodeContextMenuActions = new Dictionary<string, Action>
            {
                { "Open", new Action(() => View.Open()) }, 
                // because View.Open will be initialized later in constructors 
                // of derived classes
                { "Copy", this.CopyNodeToBuffer },
                { "Cut", this.CutNodeToBuffer },
                { "Undo cut", new Action(() => this.UndoCut(true)) },
                { "Paste", this.PasteNodeFromBuffer },
                { "Delete", this.RemoveNode },
                { "Expand", View.Expand },
                { "Expand all", View.ExpandAll },
                { "Collapse", View.Collapse },
                { "Properties", View.ShowProperties },
                { "Rename", View.StartNameEditing },
            };
        }

        public void HandleNodeAction(string name)
        {
            _nodeContextMenuActions[name]();
        }

        public void HandleListItemAction(string name)
        {
            _listItemContextMenuActions[name]();
        }

        protected void CopyNodeToBuffer()
        {
            if (_bufferElementState == BufferElementState.Moving)
            {
                MessageBox.Show(
                    $"Impossible to copy {View.Name} while {_buffer.Name} is moving",
                    "Copying error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_bufferElementState == BufferElementState.Cut)
            {
                if (_parent == this.View.Tree.List.DisplayedNode)
                {
                    this.UndoCut(true);
                }
                else
                {
                    this.UndoCut();
                }
            }
            _buffer = View;
            _bufferElementState = BufferElementState.Copied;
        }

        protected void CutNodeToBuffer()
        {
            if (_bufferElementState == BufferElementState.Moving)
            {
                MessageBox.Show(
                    $"Impossible to cut {View.Name} while {_buffer.Name} is moving",
                    "Cutting error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_bufferElementState == BufferElementState.Cut)
            {
                if (_parent == this.View.Tree.List.DisplayedNode)
                {
                    this.UndoCut(true);
                }
                else
                {
                    this.UndoCut();
                }
            }
            _buffer = View;
            _parent = View.Parent;
            View.Remove();
            _parent.DisplayOnListView();
            _bufferElementState = BufferElementState.Cut;
        }

        protected void UndoCut(bool displayParent = false)
        {
            if (_buffer == null) return;

            _parent.AddSubNode(_buffer);
            _parent.SortSubNodes(displayParent);

            _buffer.ListItem.Selected = false;

            _parent = null;
            _buffer = null;

            _bufferElementState = BufferElementState.Empty;
        }

        protected void MoveNode()
        {
            if (View == _parent)
            {
                MessageBox.Show($"Impossible to move {_buffer.Name} into the same directory",
                    "Moving error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string newPath = Path.Combine(View.Entity.Path, _buffer.Name);

            _clone = _buffer.GetClone();

            View.AddSubNode(_clone);

            View.SortSubNodes(false);

            bool isMovingSucceed = true;

            try
            {
                _buffer.Entity.Move(newPath);
            }
            catch (AlreadyExistsException)
            {
                MessageBox.Show($"Impossible to move: {_buffer.Name} already exists.",
                    "Moving error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isMovingSucceed = false;
            }
            finally
            {
                if (!isMovingSucceed)
                {
                    View.RemoveSubNode(_clone);
                }
                else
                {
                    _clone.MarkAsSelected();
                    View.Tree.List.FinishMoving();
                    _clone = null;
                    _parent = null;
                    _buffer = null;
                    _bufferElementState = BufferElementState.Empty;
                }
                View.Expand();
                View.DisplayOnListView();
            }
        }

        protected void PasteNodeFromBuffer()
        {
            if (_buffer == null)
            {
                MessageBox.Show("Nothing to paste.", "Pasting error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (View.IsChild(_buffer))
            {
                MessageBox.Show($"Impossible to paste {_buffer.Name} into child directory.",
                    "Pasting error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (View == _parent && _bufferElementState == BufferElementState.Cut)
            {
                MessageBox.Show($"Impossible to paste {_buffer.Name} into the same directory.",
                    "Pasting error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (View == _buffer)
            {
                MessageBox.Show($"Impossible to paste {_buffer.Name} into itself.",
                    "Pasting error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string newPath = Path.Combine(View.Entity.Path, _buffer.Name);

            if (_clone == null)
            {
                _clone = _buffer.GetClone();
            }

            View.Tree.BeginUpdate();

            View.AddSubNode(_clone);

            View.SortSubNodes(false);

            bool isCopyingSucceed = true;

            try
            {
                _buffer.Entity.CopyTo(newPath);
            }
            catch (AlreadyExistsException)
            {
                MessageBox.Show($"Impossible to paste: {_buffer.Name} already exists.",
                    "Pasting error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (_bufferElementState == BufferElementState.Cut)
                    {
                        _buffer = null;
                        _parent = null;
                    }
                    _clone = null;
                    _bufferElementState = BufferElementState.Empty;
                    View.Expand();
                    View.DisplayOnListView();
                }
                View.Tree.EndUpdate();
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

        public void RemoveNode()
        {
            DialogResult reallyDelete =  MessageBox.Show(
                $"Are you sure you want to delete {View.Name}?", "Deleting", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (reallyDelete != DialogResult.Yes) return;
            IFileSystemTreeNode parent = View.Parent;
            View.Entity.Delete();
            View.Remove();
            parent.DisplayOnListView();
        }
    }

    public class DirectoryItemPresenter : FileSystemItemPresenter
    {
        public DirectoryItemPresenter(IFileSystemTreeNode view) : base(view) 
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

