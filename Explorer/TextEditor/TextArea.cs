using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    public class TextArea : RichTextBox
    {
        public TextEditor Editor { get; set; }

        public TextArea() : base()
        {
            this.Dock = DockStyle.Fill;
            this.BorderStyle = BorderStyle.None;
            this.Font = Constants.ViewItemFont;

            this.ContextMenuStrip = new ContextMenuStrip();
            this.AddContextMenuOption("Toggle Case", new Action(() => Editor.ToggleCase()));
            this.AddContextMenuOption("Save", new Action(() => Editor.Save()));

            ToolStripMenuItem saveAs = new ToolStripMenuItem("Save as...");

            ToolStripMenuItem html = new ToolStripMenuItem(".html");
            html.Click += (s, e) =>
            {
                Editor.SaveAs(".html");
            };

            ToolStripMenuItem txt = new ToolStripMenuItem(".txt");
            txt.Click += (s, e) =>
            {
                Editor.SaveAs(".txt");
            };

            saveAs.DropDownItems.AddRange(new ToolStripMenuItem[]
            {
                html, txt,
            });

            this.ContextMenuStrip.Items.Add(saveAs);

            this.AddContextMenuOption("Check orthography", new Action(
                () =>
                {
                    Editor.CheckOrthography();
                }
            ));

            SuggestionsShower.TextArea = this;

            this.TextChanged += (s, e) =>
            {
                if (Editor.IsSaved)
                {
                    Editor.IndicateUnsavedChanges();
                }

            };
        }

        public void Display(string text)
        {
            this.Text = text;
        }

        public void ShowCorrect(string word, Point target)
        {
            OrthographyContextMenu CorrectMenu = new OrthographyContextMenu(this);
            CorrectMenu.DisplayCorrect(word);
            CorrectMenu.Show(this, target);
        }

        public void ShowSuggestions(string word, List<string> suggestions, Point target)
        {
            OrthographyContextMenu IncorrectMenu = new OrthographyContextMenu(this);
            IncorrectMenu.DisplaySuggestions(word, suggestions);
            IncorrectMenu.Show(this, target);
        }

        private void AddContextMenuOption(string name, Action onClick)
        {
            ToolStripMenuItem option = new ToolStripMenuItem(name);
            option.Click += (s, e) => onClick();

            this.ContextMenuStrip.Items.Add(option);
        }
    }
}
