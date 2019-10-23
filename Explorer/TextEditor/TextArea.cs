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
    class TextArea : RichTextBox
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
            this.AddContextMenuOption("Check orthography",
                new Action(() => Editor.CheckOrthography()));

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

        private void AddContextMenuOption(string name, Action onClick)
        {
            ToolStripMenuItem option = new ToolStripMenuItem(name);
            option.Click += (s, e) => onClick();

            this.ContextMenuStrip.Items.Add(option);
        }
    }
}
