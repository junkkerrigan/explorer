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

        static readonly BackToFolder backToFolder = new BackToFolder();

        static CurrentLocation currentLocation = new CurrentLocation("");

        static Separator separator = new Separator();

        private ContextMenuStrip RightClickMenu;

        public IFileSystemListItem  DisplayedItem { get; set; }

        public IFileSystemTreeNode DisplayedNode { get; set; }

        public FileSystemList() : base()
        {
            this.Dock = DockStyle.Fill;
            this.Font = Globals.ViewItemFont;
            this.BorderStyle = BorderStyle.None;
            this.View = View.Tile;
            this.TileSize = new Size(600, 30);

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
                    this.DisplayedItem.Presenter.HandleListItemContextMenuAction(
                        $"Create {name.ToLower()}");
                };

                createOption.DropDownItems.Add(subOption);
            }

            this.ItemSelectionChanged += (s, e) =>
            {
                if (e.IsSelected && e.Item == currentLocation && !currentLocation.IsMoving)
                {
                    e.Item.Selected = false;
                    e.Item.Focused = false;
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
                        Console.WriteLine("Err: empty name.");
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
                            Console.WriteLine("Folder");
                            IFileSystemItemEntity.Factory.CreateNewFolder(path);
                        }
                        else
                        {
                            throw new NotSupportedException("Error: you can't create drive.");
                        }
                    }
                    catch(AlreadyExistsException)
                    {
                        // ShowModalAlreadyExists();
                        Console.WriteLine("Err: already exists.");
                        e.CancelEdit = true;
                        item.StartNameEditing();
                    }
                    catch(Exception ex)
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
                catch (FileAlreadyExistsException)
                {
                    Console.WriteLine("File already exists");
                    e.CancelEdit = true;
                }
                catch (DirectoryAlreadyExistsException)
                {
                    Console.WriteLine("Directory already exists");
                    e.CancelEdit = true;
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

            ImageList itemIcons = new ImageList
            {
                ImageSize = new Size(25, 25)
            };
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/driveIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/folderIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/fileIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/backToFolderIcon.png"));
            itemIcons.Images.Add(Image.FromFile("../../assets/icons/moveToIcon.png"));

            this.LargeImageList = this.SmallImageList = itemIcons;
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
                    TextRenderer.MeasureText(subNode.Name, Globals.ViewItemFont).Width);
            }

            this.TileSize = new Size(maxWidth, 30);

            this.DisplayedNode = node;

            this.DisplayedItem = node.ListItem;

            if (currentLocation.IsMoving)
            {
                currentLocation.Name = $"    Move to {this.DisplayedItem.Entity.Path}";
                currentLocation.Node = this.DisplayedNode;
            }
            else
            {
                currentLocation.Name = this.DisplayedItem.Entity.Path;
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

        protected void AddContextMenuOption(string name)
        {
            ToolStripMenuItem option = new ToolStripMenuItem(name);
            option.Click += (s, e) =>
            {
                this.DisplayedItem.Presenter.HandleListItemContextMenuAction(name);
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
