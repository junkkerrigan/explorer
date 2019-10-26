using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Explorer
{
    public class TextEditor : Form
    {
        private readonly TextArea TextArea;

        private TextFileEntity DisplayedFile;

        public bool IsSaved { get; set; }

        public Saver Saver;

        public TextEditor() : base()
        {
            // TODO: add save button
            this.BackColor = Color.White;
            this.Padding = new Padding(5);
            this.Size = new Size(1000, 600);
            
            this.IsSaved = true;

            TextArea = new TextArea();
            TextArea.Editor = this;
            this.Controls.Add(TextArea);
        }
        
        public void Save()
        {
            DisplayedFile.Write(TextArea.Text);
            DisplayedFile.Save();
            
            if (this.IsSaved) return;
            this.Text = this.Text.Substring(1); // without asterisk
            IsSaved = true;
        }

        public void SaveAs(string extension)
        {
            DisplayedFile.Write(TextArea.Text);

            string text = this.Text.Trim(new char[] { '*' });

            int idx = text.LastIndexOf('.');

            if (extension == ".html")
            {
                Saver = new HTMLSaver(DisplayedFile);
                Saver.Save();

                if (idx == -1)
                {
                    text += ".html";
                }
                else
                {
                    text = text.Substring(0, idx) + ".html";
                }

            }
            else if (extension == ".txt")
            {
                Saver = new TXTSaver(DisplayedFile);
                Saver.Save();

                if (idx == -1)
                {
                    text += ".txt";
                }
                else
                {
                    text = text.Substring(0, idx) + ".txt";
                }
            }

            this.Text = text;
        }

        public void IndicateUnsavedChanges()
        {
            this.Text = "*" + this.Text;
            this.IsSaved = false;
        }

        public void CheckOrthography()
        {
            char[] delimiters = new char[] { ' ', '\n' };

            int begin = TextArea.SelectionStart, len = TextArea.SelectionLength,
                end = begin + len - 1;
            string text = TextArea.Text;

            if (len == 0) return;

            if (delimiters.Contains(text[begin]))
            {
                while (
                    begin <= end
                    && (delimiters.Contains(text[begin])))
                {
                    begin++;
                }
            }
            else
            {
                while(
                    begin > 0
                    && !delimiters.Contains(text[begin - 1]))
                {
                    begin--;
                }
            }

            if (delimiters.Contains(text[end]))
            {
                while (
                    end > begin
                    && delimiters.Contains(text[end]))
                {
                    end--;
                }
            }
            else
            {
                while (
                    end < text.Length - 1
                    && !delimiters.Contains(text[end + 1]))
                {
                    end++;
                }
            }

            if (begin > end) return;

            TextArea.SelectionLength = 0;

            SuggestionsShower.Reset();
            SuggestionsShower.BeforeSelection = text.Substring(0, begin);
            SuggestionsShower.Selection = text.Substring(begin, end - begin + 1);
            if (end < text.Length - 1)
            {
                SuggestionsShower.AfterSelection = text.Substring(end + 1);
            }

            SuggestionsShower.Show();
        }

        public void ToggleCase()
        {
            string selectedText = TextArea.SelectedText,
                text = TextArea.Text;

            int start = TextArea.SelectionStart, length = TextArea.SelectionLength;

            if (selectedText == selectedText.ToUpper())
            {
                text = text.Substring(0, start) + selectedText.ToLower()
                    + text.Substring(start + length);
            }
            else
            {
                text = text.Substring(0, start) + selectedText.ToUpper()
                    + text.Substring(start + length);
            }

            TextArea.Text = text;

            TextArea.Select(start, length);
        }

        public void Open(IFileSystemItem file)
        {
            this.DisplayedFile = new TextFileEntity(file.Entity as FileEntity);

            this.Text = file.Name;

            string fileText = DisplayedFile.Read();

            TextArea.Display(fileText);

            this.ShowDialog();
        }
    }
}
