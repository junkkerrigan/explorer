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

        static readonly BackToFolder backToFolder = new BackToFolder();

        static CurrentLocation currentLocation = new CurrentLocation("");

        static Separator separator = new Separator();

        private ContextMenuStrip RightClickMenu;

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

        public FileSystemList() : base()
        {
            this.Dock = DockStyle.Fill;
            this.Font = Constants.ViewItemFont;
            this.BorderStyle = BorderStyle.None;
            this.View = View.Tile;
            this.TileSize = new Size(600, 30);
            this.OwnerDraw = true;

            RightClickMenu = new ContextMenuStrip();

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

            this.KeyUp += (s, e) =>
            {
                if (e.KeyCode == Keys.VolumeUp && this.SelectedItem is FileItem)
                {
                    TextEditor editor = new TextEditor();

                    editor.Open(this.SelectedItem);
                }
            };

            this.DrawItem += (s, e) =>
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
            };

            // to prevent from being selected or focused 
            this.ItemSelectionChanged += (s, e) =>
            {
                if (e.IsSelected)
                {
                    if (!(e.Item as IFileSystemListItem).IsAccessible)
                    {
                        e.Item.Selected = false;
                        e.Item.Focused = false;
                    }
                }
            };

            this.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (this.GetItemAt(e.X, e.Y) == null)
                    {
                        RightClickMenu.Show(Cursor.Position);
                    }
                    else
                    {
                        IFileSystemListItem target = this.GetItemAt(e.X, e.Y) as IFileSystemListItem;
                        target.ShowMenu();
                    }
                }
            };

            this.MouseDoubleClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right) return;
                if (this.SelectedItems.Count == 0) return;

                IFileSystemListItem selectedItem = this.SelectedItems[0] 
                    as IFileSystemListItem;
                selectedItem.Open();
            };

            this.AfterLabelEdit += (s, e) =>
            {
                IFileSystemListItem item = this.Items[e.Item] as IFileSystemListItem;

                if (item.Name == "") // editing after creating
                {
                    if (e.Label == "" || e.Label == null)
                    {
                        string itemType = (item is FolderItem) ? "folder" : "file";

                        MessageBox.Show($"Impossible to create {itemType} with empty name.",
                            "Creating error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.CancelEdit = true;
                        item.StartNameEditing();
                        return;
                    }

                    string path = System.IO.Path.Combine(this.DisplayedNode.Entity.Path, e.Label);

                    try
                    {
                        if (item is FileItem)
                        {
                            IFileSystemItemEntity.Factory.CreateNewFile(path);
                        }
                        else if (item is FolderItem)
                        {
                            IFileSystemItemEntity.Factory.CreateNewFolder(path);
                        }
                        else
                        {
                            throw new NotSupportedException("Error: you can't create drive.");
                        }
                    }
                    catch (AlreadyExistsException)
                    {
                        MessageBox.Show($"Impossible to create: {e.Label} already exists.",
                            "Creating error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.CancelEdit = true;
                        item.StartNameEditing();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        if (!e.CancelEdit)
                        {
                            item.Node.Name = e.Label;
                            item.Entity.Path = path;
                        }
                    }
                    return;
                }



                try
                {
                    item.Entity.EditName(e.Label);
                }
                catch (AlreadyExistsException)
                {
                    if (e.Label == null || e.Label == "")
                    {
                        e.CancelEdit = true;
                    }
                    else if (e.Label != item.Name)
                    {
                        MessageBox.Show($"Impossible to rename: {e.Label} already exists.",
                            "Renaming error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.CancelEdit = true;
                    }
                }
                finally
                {
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
                    this.LabelEdit = false;
                }
            };

            // TODO: move to Globals
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

            this.LargeImageList = this.SmallImageList = itemIcons;
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

            this.DisplayedNode = null;

            this.DisplayedItem = null;

            foreach (IFileSystemTreeNode node in this.Tree.RootNodes)
            {
                this.AddItem(node.ListItem);
            }
        }

        public void Display(IFileSystemTreeNode node)
        {
            // TODO: move to presenter
            this.Items.Clear();

            this.View = View.Tile;

            int maxWidth = this.Size.Width;

            foreach (IFileSystemTreeNode subNode in node.SubNodes)
            {
                maxWidth = System.Math.Max(maxWidth, 40 + 
                    TextRenderer.MeasureText(subNode.Name, Constants.ViewItemFont).Width);
            }

            this.TileSize = new Size(maxWidth, 30);

            this.DisplayedNode = node;

            this.DisplayedItem = node.ListItem;

            if (currentLocation.IsMoving)
            {
                currentLocation.Name = 
                    $"    Move {this.DisplayedNode.Presenter.Buffer.Name}" +
                    $" to {this.DisplayedItem.Entity.Path}";
                currentLocation.Node = this.DisplayedNode;
            }
            else
            {
                currentLocation.Name = $"    {this.DisplayedItem.Entity.Path}";
            }

            this.AddItem(currentLocation);

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
            currentLocation.EnableMovingMode();
        }

        public void FinishMoving()
        {
            currentLocation.DisableMovingMode();
            this.Display(DisplayedNode);
        }
    }
}
