using System;
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

        public MainMenu(IFileSystemList list) : base()
        {
            List = list;

            this.Font = new Font("Verdana", 12);
            this.BackColor = Color.Transparent;

            var general = new ToolStripMenuItem("General");

            var create = new ToolStripMenuItem("Create");

            var folder = new ToolStripMenuItem("Folder");
            AssignGeneralMenuOption(folder, "Create folder");

            var file = new ToolStripMenuItem("File");
            AssignGeneralMenuOption(file, "Create file");

            create.DropDownItems.Add(folder);
            create.DropDownItems.Add(file);

            var paste = new ToolStripMenuItem("Paste");
            AssignGeneralMenuOption(paste, "Paste");

            var undoCut = new ToolStripMenuItem("Undo cut");
            AssignGeneralMenuOption(undoCut, "Undo cut");

            var cancelMoving = new ToolStripMenuItem("Cancel moving");
            AssignGeneralMenuOption(cancelMoving, "Cancel moving");

            general.DropDownItems.AddRange(new ToolStripMenuItem[]
            {
                create, paste, undoCut, cancelMoving,
            });

            var selected = new ToolStripMenuItem("Selected");

            var open = new ToolStripMenuItem("Open");
            AssignSelectedMenuOption(open, "Open");

            var copy = new ToolStripMenuItem("Copy");
            AssignSelectedMenuOption(copy, "Copy");

            var cut = new ToolStripMenuItem("Cut");
            AssignSelectedMenuOption(cut, "Cut");

            var move = new ToolStripMenuItem("Move");
            AssignSelectedMenuOption(move, "Move");

            var rename = new ToolStripMenuItem("Rename");
            AssignSelectedMenuOption(rename, "Rename");

            var delete = new ToolStripMenuItem("Delete");
            AssignSelectedMenuOption(delete, "Delete");

            selected.DropDownItems.AddRange(new ToolStripMenuItem[]
            {
                open, copy, cut, move, rename, delete,
            });

            var faq = new ToolStripMenuItem("FAQ");
            faq.Click += (s, e) =>
            {
                MessageBox.Show("", "FAQ");
            };

            var help = new ToolStripMenuItem("Help");
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
                IFileSystemListItem selected = List.SelectedItem;
                if (selected == null || selected is BackToFolder)
                {
                    MessageBox.Show(
                        $"Impossible to {option.Text.ToLower()}: nothing is selected.", 
                        "Operation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (selected is DriveItem && option.Text != "Open")
                {
                    MessageBox.Show(
                        $"Impossible to {option.Text.ToLower()} {selected.Name}.",
                        "Operation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    selected.Presenter.HandleListItemAction(action);
                }
            };
        }

        private void AssignGeneralMenuOption(ToolStripMenuItem option, string action)
        {
            option.Click += (s, e) =>
            {
                IFileSystemListItem selected = List.DisplayedItem;
                if (selected != null)
                {
                    selected.Presenter.HandleListItemAction(action);
                }
            };
        }
    }
}
