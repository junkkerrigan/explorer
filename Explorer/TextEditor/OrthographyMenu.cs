using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    public class OrthographyMenu : ContextMenuStrip
    {
        public OrthographyMenu() : base()
        {

        }

        public void ShowCorrect(string word)
        {
            this.Items.Clear();
            ToolStripMenuItem correct = new ToolStripMenuItem(word);
            // add image
        }

        public void ShowSuggestions(string word, List<string> suggestions)
        {

        }
    }
}
