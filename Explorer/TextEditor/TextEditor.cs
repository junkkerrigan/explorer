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

        public void IndicateUnsavedChanges()
        {
            this.Text = "*" + this.Text;
            this.IsSaved = false;
        }

        public void CheckOrthography()
        {
            int curPos = TextArea.GetCharIndexFromPosition(TextArea.MouseRelativeLocation);
            string text = TextArea.Text;

            if (text == "") return;

            // looking for the word to check
            int start, end;

            while (curPos >= 0 && text[curPos] != ' ' && text[curPos] != '\n') curPos--;

            start = (curPos == 0) ? 0 : curPos + 1;
            end = text.IndexOf(' ', start);

            if (end == -1)
            {
                end = text.IndexOf('\n', start);
                
                if (end == -1)
                {
                    end = text.Length;
                }
            }

            string word = text.Substring(start, end - start);

            if (word == " " || word == "") return;

            TextArea.CurrentWordStart = start;
            TextArea.CurrentWordEnd = end;

            // checking the word
            if (OrthographyChecker.IsCorrect(word))
            {
                TextArea.ShowCorrect(word);
            }
            else
            {
                List<string> suggestions = OrthographyChecker.GetSimilar(word);

                TextArea.ShowSuggestions(word, suggestions);
            }
        }

        public void ChangeCurrentWordBy(string newWord)
        {
            string text = TextArea.Text;
            int start = TextArea.CurrentWordStart, end = TextArea.CurrentWordEnd;

            text = text.Substring(0, start) + newWord 
                + text.Substring(end);

            TextArea.Text = text;
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
