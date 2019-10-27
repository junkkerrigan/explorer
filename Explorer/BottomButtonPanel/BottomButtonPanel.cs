using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    class BottomButtonPanel : FlowLayoutPanel
    {
        IFileSystemList List { get; set; }

        public BottomButtonPanel(IFileSystemList list) : base()
        {
            List = list;

            BottomButton copy = new BottomButton("Copy");
            AssignSelectedMenuOption(copy, "Copy");

            BottomButton cut = new BottomButton("Cut");
            AssignSelectedMenuOption(cut, "Cut");

            BottomButton move = new BottomButton("Move");
            AssignSelectedMenuOption(move, "Move");

            BottomButton rename  = new BottomButton("Rename");
            AssignSelectedMenuOption(rename, "Rename");

            BottomButton delete = new BottomButton("Delete");
            AssignSelectedMenuOption(delete, "Delete");

            BottomButton paste = new BottomButton("Paste");
            AssignGeneralMenuOption(paste, "Paste");

            BottomButton help = new BottomButton("Help");
            help.Click += (s, e) =>
            {
                MessageBox.Show("Enter your question or try to find the answer in FAQ.", "Help");
            };

            this.Controls.AddRange(new BottomButton[]
            {
                 copy, cut, paste, move, rename, delete, help,
            });
        }

        private void AssignSelectedMenuOption(BottomButton btn, string action)
        {
            btn.Click += (s, e) =>
            {
                IFileSystemListItem selected = List.SelectedItem;
                if (selected == null || selected is DriveItem) return;
                else
                {
                    selected.Presenter.HandleListItemAction(action);
                }
            };
        }

        private void AssignGeneralMenuOption(BottomButton btn, string action)
        {
            btn.Click += (s, e) =>
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
