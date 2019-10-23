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

        public Point MouseRelativeLocation { get; set; }

        public Point MouseAbsoluteLocation { get; set; }

        public int CurrentWordStart { get; set; }

        public int CurrentWordEnd { get; set; }

        public OrthographyContextMenu OrthographyMenu { get; set; }

        public TextArea() : base()
        {
            this.Dock = DockStyle.Fill;
            this.BorderStyle = BorderStyle.None;
            this.Font = Constants.ViewItemFont;
            this.CurrentWordStart = this.CurrentWordEnd = 0;

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
                    this.MouseAbsoluteLocation = this.ContextMenuStrip.Bounds.Location;
                    Editor.CheckOrthography();
                }
            ));

            this.OrthographyMenu = new OrthographyContextMenu();
            this.OrthographyMenu.TextArea = this;

            this.TextChanged += (s, e) =>
            {
                if (Editor.IsSaved)
                {
                    Editor.IndicateUnsavedChanges();
                }

            };

            this.MouseUp += (s, e) =>
            {
                MouseRelativeLocation = e.Location;
            };


        }

        public void Display(string text)
        {
            this.Text = text;
        }

        public void ShowCorrect(string word)
        {
            this.OrthographyMenu.DisplayCorrect(word);
            this.OrthographyMenu.Show(MouseAbsoluteLocation);
        }

        public void ShowSuggestions(string word, List<string> suggestions)
        {
            this.OrthographyMenu.DisplaySuggestions(word, suggestions);
            this.OrthographyMenu.Show(MouseAbsoluteLocation);
        }

        private void AddContextMenuOption(string name, Action onClick)
        {
            ToolStripMenuItem option = new ToolStripMenuItem(name);
            option.Click += (s, e) => onClick();

            this.ContextMenuStrip.Items.Add(option);
        }
    }
}
