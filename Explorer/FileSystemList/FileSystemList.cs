using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer
{
    public class FileSystemList : ListView, IFileSystemList
    {
        public IFileSystemTree Tree { get; set; }

        public SearchBox SearchBox { get; set; }

        public List<IFileSystemListItem> RootItems 
        { 
            get 
            {
                List<IFileSystemListItem> items = new List<IFileSystemListItem>();

                foreach(IFileSystemListItem item in this.Items)
                {
                    // if real item but not staff item
                    if (item.Node != null && item.Entity != null)
                    items.Add(item);
                }

                return items;
            }
        }

        public IFileSystemListItem  DisplayedItem { get; set; }

        public IFileSystemTreeNode DisplayedNode { get; set; }

        public IFileSystemListItem SelectedItem
        {
            get
            {
                if (this.SelectedItems.Count == 0 || 
                    this.SelectedItems[0] is BackToFolder) return null;
                
                return (this.SelectedItems[0] as IFileSystemListItem);
            }
        }

        private readonly BackToFolder backToFolder = new BackToFolder();
        private readonly Separator separator = new Separator();
        private readonly CurrentLocation currentLocation = new CurrentLocation();
        private readonly Mover mover = new Mover();
        private readonly Merger merger = new Merger();

        private readonly ContextMenuStrip RightClickMenu;
        
        private bool IsMoving; // move to constants as enum and implement CurrentAction
        private bool IsChoosingMergeWith;

        public bool IsChoosingMergeTo { get; set; }

        public FileSystemList() : base()
        {
            this.Dock = DockStyle.Fill;
            this.Font = Constants.ViewItemFont;
            this.BorderStyle = BorderStyle.None;
            this.View = View.Tile;
            this.TileSize = new Size(600, 35);
            this.OwnerDraw = true;
            this.LabelEdit = false;

            RightClickMenu = new ContextMenuStrip();

            // open editor
            this.KeyUp += FileSystemList_KeyUp;
            this.MouseDown += FileSystemList_MouseDown;
            // to prevent from being selected or focused 
            this.ItemSelectionChanged += FileSystemList_ItemSelectionChanged;
            this.DrawItem += FileSystemList_DrawItem;
            this.MouseDoubleClick += FileSystemList_MouseDoubleClick;
            this.AfterLabelEdit += FileSystemList_AfterLabelEdit;

            FillImageList();
        }

        private void FillImageList()
        {
            ImageList itemIcons = new ImageList
            {
                ImageSize = new Size(25, 25)
            };
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/backToFolderIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/currentLocationIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/moveToIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/mergeIcon.png"));

            this.LargeImageList = this.SmallImageList = itemIcons;
        }

        private void FileSystemList_KeyUp(object s, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.VolumeUp && this.SelectedItem is FileItem)
            {
                TextEditor editor = new TextEditor();

                editor.Open(this.SelectedItem);
            }
        }

        private void FileSystemList_MouseDown(object s, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem item = this.GetItemAt(e.X, e.Y);
                if (item == null)
                {
                    RightClickMenu.Show(Cursor.Position);
                }
                else
                {
                    IFileSystemListItem target = item as IFileSystemListItem;
                    if (target.IsAccessible && target.RealWidth >= e.X)
                    {
                        target.ShowMenu();
                    }
                    else
                    {
                        RightClickMenu.Show(Cursor.Position);
                    }
                }
            }
        }

        private void FileSystemList_ItemSelectionChanged(object s, 
            ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected && !(e.Item as IFileSystemListItem).IsAccessible)
            {
                e.Item.Selected = false;
                e.Item.Focused = false;
            }
        }

        private void FileSystemList_MouseDoubleClick(object s, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;
            if (this.SelectedItems.Count == 0) return;

            IFileSystemListItem selectedItem = this.SelectedItems[0]
                as IFileSystemListItem;

            if (IsChoosingMergeWith && selectedItem is FileItem)
            {
                IsChoosingMergeWith = false;
                IsChoosingMergeTo = true;
                merger.MergeWithNode = selectedItem.Node;
                this.UpdateRefresh();
                merger.EnsureVisible();
                return;
            }

            selectedItem.Open();
        }

        private void FileSystemList_AfterLabelEdit(object s, LabelEditEventArgs e)
        {
            this.LabelEdit = false;
            IFileSystemListItem item = this.Items[e.Item] as IFileSystemListItem;
            string path = this.DisplayedNode.Entity.Path, name = e.Label;

            if (IsChoosingMergeTo) // editing after file that is result of merging created
            {
                HandleAfterMergeResultCreating();
            }
            else if (item.Name == "") // editing after creating
            {
                HandleAfterCreating();
            }
            else
            {
                HandleAfterRenaming();
            }

            item.CalcRealWidth(item.Node.Name);

            void HandleAfterRenaming()
            {
                if (name == null || name == "")
                {
                    e.CancelEdit = true;
                    return;
                }

                try
                {
                    item.Entity.EditName(e.Label);
                }
                catch (DirectoryAlreadyExistsException)
                {
                    if (e.Label != item.Name)
                    {
                        MessageBox.Show("Impossible to rename: directory with name"
                            + $" `{e.Label}` already exists.", "Renaming error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.CancelEdit = true;
                    }
                }
                catch (FileAlreadyExistsException)
                {
                    if (e.Label != item.Name)
                    {
                        MessageBox.Show($"Impossible to rename: `{e.Label}` already exists.",
                            "Renaming error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.CancelEdit = true;
                    }
                }
                catch (ArgumentException ex)
                {
                    if (ex.Message == ".")
                    {
                        MessageBox.Show("Impossible to rename: invalid name.", "Creating error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Impossible to rename: name can't contain invalid"
                            + " characters ( \\ / | : * < > \" ? ).", "Creating error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    e.CancelEdit = true;
                }
                if (!e.CancelEdit)
                {
                    item.Node.Name = e.Label;
                    this.BeginInvoke(
                        new Action(() => {
                            item.Node.Parent.SortSubNodes();
                        })
                    );
                    // for sorting after node name will finally be changed
                }
                else
                {
                    item.StartNameEditing();
                }
            }

            void HandleAfterCreating()
            {
                if (name == "" || name == null)
                {
                    if (item is FolderItem)
                    {
                        int cnt = 0;
                        string folderName = "New folder";
                        
                        while(true)
                        {
                            if (cnt != 0)
                            {
                                folderName = $"New folder ({cnt})";
                            }

                            try
                            {
                                e.CancelEdit = true;
                                IFileSystemItemEntity.Factory.CreateNewFolder(path, 
                                    folderName);
                            }
                            catch (AlreadyExistsException)
                            {
                                e.CancelEdit = false;
                            }
                            if (e.CancelEdit)
                            {
                                string folderPath = Path.Combine(path, folderName);

                                item.Name = folderName;
                                item.Node.Name = folderName;
                                item.Entity.Path = folderPath;

                                return;
                            }

                            cnt++;
                        }
                    }
                    else if (item is FileItem)
                    {
                        int cnt = 0;
                        string fileName = "New file";

                        while (true)
                        {
                            if (cnt != 0)
                            {
                                fileName = $"New file ({cnt})";
                            }

                            try
                            {
                                e.CancelEdit = true;
                                IFileSystemItemEntity.Factory.CreateNewFile(path,
                                    fileName);
                            }
                            catch (AlreadyExistsException)
                            {
                                e.CancelEdit = false;
                            }
                            if (e.CancelEdit)
                            {
                                string filePath = Path.Combine(path, fileName);

                                item.Name = fileName;
                                item.Node.Name = fileName;
                                item.Entity.Path = filePath;

                                return;
                            }

                            cnt++;
                        }
                    }
                }

                try
                {
                    if (item is FileItem)
                    {
                        IFileSystemItemEntity.Factory.CreateNewFile(path, name);
                    }
                    else if (item is FolderItem)
                    {
                        IFileSystemItemEntity.Factory.CreateNewFolder(path, name);
                    }
                    else
                    {
                        throw new NotSupportedException("Error: you can't create drive.");
                    }
                }
                catch (DirectoryAlreadyExistsException)
                {
                    MessageBox.Show("Impossible to create: directory with name"
                        + $" `{e.Label}` already exists.", "Creating error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.CancelEdit = true;
                    item.StartNameEditing();
                }
                catch (FileAlreadyExistsException)
                {
                    MessageBox.Show($"Impossible to create: `{e.Label}` already exists.",
                        "Creating error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.CancelEdit = true;
                    item.StartNameEditing();
                }
                catch (ArgumentException ex)
                {
                    if (ex.Message == ".")
                    {
                        MessageBox.Show("Impossible to create: invalid name.", "Creating error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Impossible to create: name can't contain invalid"
                            + " characters ( \\ / | : * < > \" ? ).", "Creating error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    e.CancelEdit = true;
                    item.StartNameEditing();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    e.CancelEdit = true;
                    item.StartNameEditing();
                }
                finally
                {
                    if (!e.CancelEdit)
                    {
                        string itemPath = Path.Combine(path, name);

                        item.Node.Name = e.Label;
                        item.Entity.Path = itemPath;
                    }
                }
            }

            void HandleAfterMergeResultCreating()
            {
                string path = this.DisplayedNode.Entity.Path, name = e.Label;

                Console.WriteLine($"-{name}-");

                if (name == null || name == "")
                {
                    Console.WriteLine("N or E");
                    string extenstion = Path.GetExtension(merger.MergeWithNode.Entity.Path);
                    if (extenstion == "")
                    {
                        extenstion = Path.GetExtension(merger.MergeNode.Entity.Path);
                    }
                    string mergeWithName =
                        Path.GetFileNameWithoutExtension(merger.MergeWithNode.Name),
                        mergeName = Path.GetFileNameWithoutExtension(merger.MergeNode.Name);
                    string fileName = $"{mergeWithName}+{mergeName}{extenstion}";
                    int cnt = 0;

                    while (true)
                    {
                        if (cnt != 0)
                        {
                            fileName = $"{mergeWithName}+{mergeName}({cnt}){extenstion}";
                        }

                        try
                        {
                            e.CancelEdit = true;
                            IFileSystemItemEntity.Factory.CreateNewFile(path,
                                fileName);
                        }
                        catch (AlreadyExistsException)
                        {
                            e.CancelEdit = false;
                        }
                        if (e.CancelEdit)
                        {
                            string filePath = Path.Combine(path, fileName);
                            Console.WriteLine($"-{filePath}-");

                            Console.WriteLine($"-{item.Name}-");
                            item.Name = fileName;

                            item.Node.Name = fileName;
                            item.Entity.Path = filePath;

                            item.Entity.Merge(merger.MergeWithNode.Entity, merger.MergeNode.Entity);
                            this.FinishMerging();

                            return;
                        }

                        cnt++;
                    }
                }

                try
                {
                    IFileSystemItemEntity.Factory.CreateNewFile(path, name);
                }
                catch (DirectoryAlreadyExistsException)
                {
                    MessageBox.Show("Impossible to create: directory with name"
                        + $" `{e.Label}` already exists.", "Creating error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.CancelEdit = true;
                    item.StartNameEditing();
                }
                catch (FileAlreadyExistsException)
                {
                    MessageBox.Show($"Impossible to create: `{e.Label}` already exists.",
                        "Creating error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.CancelEdit = true;
                    item.StartNameEditing();
                }
                catch (ArgumentException ex)
                {
                    if (ex.Message == ".")
                    {
                        MessageBox.Show("Impossible to create: invalid name.", "Creating error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Impossible to create: name can't contain invalid"
                            + " characters ( \\ / | : * < > \" ? ).", "Creating error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    e.CancelEdit = true;
                    item.StartNameEditing();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    e.CancelEdit = true;
                    item.StartNameEditing();
                }
                finally
                {
                    if (!e.CancelEdit)
                    {
                        string itemPath = Path.Combine(path, name);

                        item.Node.Name = name;
                        item.Entity.Path = itemPath;
                    }
                    item.Entity.Merge(merger.MergeWithNode.Entity, merger.MergeNode.Entity);
                    this.FinishMerging();
                }

            }
        }

        private void FileSystemList_DrawItem(object s, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true; // if custom render, changing below

            if (this.SearchBox.Text == "")
            {
                return;
            }

            IFileSystemListItem item = e.Item as IFileSystemListItem;

            if (!item.IsFileSystemItem() || !item.IsAccessible)
            {
                return;
            }

            if (this.SearchBox.SearchOnly && !e.Item.Text.EndsWith(".html"))
            {
                return;
            }

            List<int> matches = SearchByKeywords(this.SearchBox.Text, e.Item.Text);

            if (matches == null)
            {
                return;
            }

            string[] chunks = e.Item.Text.Split(new char[] { ' ' });

            string highlight;


            // check if name contains all the keywords
            bool isMatch = false;

            for (int i = 0; i < chunks.Length; i++)
            {
                string chunk = chunks[i];

                highlight = chunk.Substring(0, matches[i]);

                if (highlight.Length > 0) isMatch = true;
            }

            if (!isMatch)
            {
                return;
            }

            // draw item
            Font ordinaryFont = new Font("Verdana", 13);
            Font highlightFont = new Font("Verdana", 14, FontStyle.Bold);

            Brush ordinaryBrush = new SolidBrush(Color.Black);
            Brush highlightBrush = new SolidBrush(Color.Red);

            int idx = e.Item.ImageIndex;

            if (idx >= 0 && idx < this.SmallImageList.Images.Count)
            {
                Image icon = this.SmallImageList.Images[idx];
                e.Graphics.DrawImage(icon, new Point(e.Bounds.X, e.Bounds.Y));
            }

            float left = this.SmallImageList.ImageSize.Width + 2,
                top = e.Bounds.Top + 3;

            string ordinary;

            SizeF size;

            for (int i = 0; i < chunks.Length; i++)
            {
                string chunk = chunks[i];

                highlight = chunk.Substring(0, matches[i]);

                e.Graphics.DrawString(highlight, highlightFont, highlightBrush, left,
                    top - 2);

                size = e.Graphics.MeasureString(highlight, highlightFont);

                left += size.Width;

                ordinary = chunk.Substring(matches[i]);

                e.Graphics.DrawString(ordinary, ordinaryFont, ordinaryBrush, left, top);

                size = e.Graphics.MeasureString(ordinary, ordinaryFont);

                left += size.Width;
            }

            e.DrawDefault = false;
        }

        private void FillGeneralContextMenu()
        {
            string[] contextMenuOptions =
            {
                "Create", "Paste",
            };

            foreach (string option in contextMenuOptions)
            {
                this.AddContextMenuOption(option);
            }

            string[] createOptionSubItems =
            {
                "Folder", "File",
            };

            ToolStripMenuItem createOption = this.RightClickMenu.Items[0]
                as ToolStripMenuItem;

            foreach (string name in createOptionSubItems)
            {
                ToolStripMenuItem subOption = new ToolStripMenuItem(name);
                subOption.Click += (s, e) =>
                {
                    this.DisplayedItem.Presenter.HandleListItemAction(
                        $"Create {name.ToLower()}");
                };

                createOption.DropDownItems.Add(subOption);
            }
        }

        private List<int> SearchByKeywords(string query, string text)
        {
            string[] keywords = query.Split(
                  new char[] { ' ' },
                  StringSplitOptions.RemoveEmptyEntries);

            string[] textwords = text.Split(new char[] { ' ' });

            List<int> matches = new List<int>(textwords.Length);

            for (int i = 0; i < textwords.Length; i++)
            {
                matches.Add(0);
            }

            List<bool> used = new List<bool>(keywords.Length);

            for(int i = 0; i < keywords.Length; i++)
            {
                used.Add(false);
            }

            for(int i = 0; i < keywords.Length; i++)
            {
                for (int j = 0; j < textwords.Length; j++)
                {
                    string textword = textwords[j].ToLower(),
                        keyword = keywords[i].ToLower();
                    int idx = textword.IndexOf(keyword);

                    if (textword == keyword || idx == 0)
                    {
                        used[i] = true;
                        matches[j] = Math.Max(matches[j], keyword.Length);
                    }
                }
            }

            for (int i = 0; i < used.Count; i++)
            {
                if (!used[i]) return null;
            }

            return matches;
        }

        public void AddItem(IFileSystemListItem item)
        {
            item.CalcRealWidth();
            this.Items.Add(item as ListViewItem);
        }

        public void AddItems(List<IFileSystemListItem> items)
        {
            foreach(IFileSystemListItem i in items)
            {
                this.AddItem(i);
            }
        }

        public void DisplayTree()
        {
            this.Items.Clear();
            this.RightClickMenu.Items.Clear();

            this.DisplayedNode = null;
            this.DisplayedItem = null;

            foreach (IFileSystemTreeNode node in this.Tree.RootNodes)
            {
                this.AddItem(node.ListItem);
            }
        }

        public void Display(IFileSystemTreeNode node)
        {
            this.Items.Clear();
            if (this.RightClickMenu.Items.Count == 0)
            {
                this.FillGeneralContextMenu();
            }

            int maxWidth = this.Size.Width;
            foreach (IFileSystemTreeNode subNode in node.SubNodes)
            {
                maxWidth = System.Math.Max(maxWidth, subNode.ListItem.RealWidth);
            }
            this.TileSize = new Size(maxWidth, 35);

            this.DisplayedNode = node;
            this.DisplayedItem = node.ListItem;

            if (IsMoving)
            {
                mover.Name = $"    Move `{DisplayedNode.Presenter.Buffer.Name}`" +
                    $" to `{DisplayedItem.Entity.Path}`";
                mover.Node = DisplayedNode;
                this.AddItem(mover);
            }
            else if (IsChoosingMergeWith)
            {
                merger.Name =
                    $"    Merge `{merger.MergeNode.Name}` with...";
                this.AddItem(merger);
            }
            else if (IsChoosingMergeTo)
            {
                merger.Name = 
                    $"    Merge `{merger.MergeNode.Name}` with `{merger.MergeWithNode.Name}`"
                    + $" to `{DisplayedItem.Entity.Path}`";
                this.AddItem(merger);
            }
            else
            {
                currentLocation.Name = $"    {this.DisplayedItem.Entity.Path}";
                this.AddItem(currentLocation);
            }

            this.AddItem(separator);
            this.AddItem(backToFolder);

            foreach (IFileSystemTreeNode subNode in node.SubNodes)
            {
                this.AddItem(subNode.ListItem);
            }
        }

        public void Display(IFileSystemListItem item)
        {
            this.Display(item.Node);
        } 

        public void UpdateRefresh()
        {
            if (this.DisplayedNode == null)
            {
                this.DisplayTree();
            }
            else
            {
                this.Display(DisplayedNode);
            }
        }

        protected void AddContextMenuOption(string name)
        {
            ToolStripMenuItem option = new ToolStripMenuItem(name);
            option.Click += (s, e) =>
            {
                this.DisplayedItem.Presenter.HandleListItemAction(name);
            };

            this.RightClickMenu.Items.Add(option);
        }

        public void StartMoving()
        {
            IsMoving = true;
        }

        public void FinishMoving()
        {
            IsMoving = false;
            this.UpdateRefresh();
        }

        public void StartMerging(IFileSystemTreeNode node)
        {
            IsChoosingMergeWith = true;
            merger.MergeNode = node;
            this.UpdateRefresh();
            merger.EnsureVisible();
        }

        public void FinishMerging()
        {
            IsChoosingMergeWith = IsChoosingMergeTo = false;
            merger.MergeNode = merger.MergeWithNode = null;
            this.UpdateRefresh();
        }
    }
}
