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
    class TextEditor : Form
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

        }

        public void ToggleCase()
        {
            string selectedText = TextArea.SelectedText;
            int start = TextArea.SelectionStart, length = TextArea.SelectionLength;

            if (selectedText == selectedText.ToUpper())
            {
                TextArea.Text = TextArea.Text.Replace(selectedText, selectedText.ToLower());
            }
            else
            {
                TextArea.Text = TextArea.Text.Replace(selectedText, selectedText.ToUpper());
            }

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
