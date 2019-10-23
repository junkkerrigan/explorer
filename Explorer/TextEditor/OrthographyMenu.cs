using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    public class OrthographyContextMenu : ContextMenuStrip
    {
        public TextArea TextArea { get; set; }

        public OrthographyContextMenu() : base()
        {
        }

        public void DisplayCorrect(string word)
        {
            this.Items.Clear();
            
            ToolStripMenuItem correct = new ToolStripMenuItem(word);
            correct.Image = Image.FromFile("../../assets/icons/tickIcon.png");

            this.Items.Add(correct);
        }

        public void DisplaySuggestions(string word, List<string> suggestions)
        {
            this.Items.Clear();

            ToolStripMenuItem incorrect = new ToolStripMenuItem(word);
            incorrect.Image = Image.FromFile("../../assets/icons/crossIcon.png");

            this.Items.Add(incorrect);
            this.Items.Add(new ToolStripSeparator());

            foreach(string sug in suggestions)
            {
                ToolStripMenuItem suggestion = new ToolStripMenuItem(sug);

                suggestion.Click += (s, e) =>
                {
                    TextArea.Editor.ChangeCurrentWordBy(sug);
                };

                this.Items.Add(suggestion);
            }
        }
    }
}
