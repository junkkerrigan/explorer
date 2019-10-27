﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    class MainMenu : MenuStrip
    {
        private readonly IFileSystemList List;

        private readonly IFileSystemTree Tree;

        public MainMenu(IFileSystemList list) : base()
        {
            List = list;
            Tree = list.Tree;

            this.Font = new Font("Verdana", 12);
            this.BackColor = Color.Transparent;
            this.Padding = new Padding(5, 10, 0, 10);

            ToolStripMenuItem general = new ToolStripMenuItem("General");

            ToolStripMenuItem create = new ToolStripMenuItem("Create");

            ToolStripMenuItem folder = new ToolStripMenuItem("Folder");
            AssignGeneralMenuOption(folder, "Create folder");

            ToolStripMenuItem file = new ToolStripMenuItem("File");
            AssignGeneralMenuOption(file, "Create file");

            create.DropDownItems.Add(folder);
            create.DropDownItems.Add(file);

            ToolStripMenuItem paste = new ToolStripMenuItem("Paste");
            AssignGeneralMenuOption(paste, "Paste");

            ToolStripMenuItem undoCut = new ToolStripMenuItem("Undo cut");
            AssignGeneralMenuOption(undoCut, "Undo cut");

            ToolStripMenuItem cancelMoving = new ToolStripMenuItem("Cancel moving");
            AssignGeneralMenuOption(cancelMoving, "Cancel moving");

            general.DropDownItems.AddRange(new ToolStripMenuItem[]
            {
                create, paste, undoCut, cancelMoving,
            });

            ToolStripMenuItem selected = new ToolStripMenuItem("Selected");

            ToolStripMenuItem open = new ToolStripMenuItem("Open");
            AssignSelectedMenuOption(open, "Open");

            ToolStripMenuItem copy = new ToolStripMenuItem("Copy");
            AssignSelectedMenuOption(copy, "Copy");

            ToolStripMenuItem cut = new ToolStripMenuItem("Cut");
            AssignSelectedMenuOption(cut, "Cut");

            ToolStripMenuItem move = new ToolStripMenuItem("Move");
            AssignSelectedMenuOption(move, "Move");

            ToolStripMenuItem rename = new ToolStripMenuItem("Rename");
            AssignSelectedMenuOption(rename, "Rename");

            ToolStripMenuItem delete = new ToolStripMenuItem("Delete");
            AssignSelectedMenuOption(delete, "Delete");

            selected.DropDownItems.AddRange(new ToolStripMenuItem[]
            {
                open, copy, cut, move, rename, delete,
            });

            ToolStripMenuItem faq = new ToolStripMenuItem("FAQ");
            faq.Click += (s, e) =>
            {
                MessageBox.Show("", "FAQ");
            };

            ToolStripMenuItem help = new ToolStripMenuItem("Help");
            help.Click += (s, e) =>
            {
                MessageBox.Show("Enter your question or try to find the answer in FAQ.", "Help");
            };

            this.Items.AddRange(new ToolStripMenuItem[]
            {
                general, selected, faq, help,
            });
        }

        private void AssignSelectedMenuOption(ToolStripMenuItem option, string action)
        {
            option.Click += (s, e) =>
            {
                IFileSystemListItem selectedItem = List.SelectedItem;

                if (selectedItem == null)
                {
                    MessageBox.Show(
                        $"Impossible to {option.Text.ToLower()}: nothing is selected.", 
                        "Operation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (selectedItem is DriveItem && option.Text != "Open")
                {
                    MessageBox.Show(
                        $"Impossible to {option.Text.ToLower()} `{selectedItem.Name}`.",
                        "Operation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    selectedItem.Presenter.HandleListItemAction(action);
                }
            };
        }

        private void AssignGeneralMenuOption(ToolStripMenuItem option, string action)
        {
            option.Click += (s, e) =>
            {
                IFileSystemListItem selectedItem = List.DisplayedItem;
                if (selectedItem != null)
                {
                    selectedItem.Presenter.HandleListItemAction(action);
                }
            };
        }
    }
}
