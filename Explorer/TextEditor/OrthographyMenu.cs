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

        public OrthographyContextMenu(TextArea textArea) : base()
        {
            TextArea = textArea;
        }

        public void DisplayCorrect(string word)
        {
            this.Items.Clear();
            
            ToolStripMenuItem correct = new ToolStripMenuItem(word);
            correct.Image = Image.FromFile("../../assets/icons/tickIcon.png");
            correct.Click += (s, e) =>
            {
                SuggestionsShower.Show();
            };

            this.Items.Add(correct);
        }

        public void DisplaySuggestions(string word, List<string> suggestions)
        {
            this.Items.Clear();

            ToolStripMenuItem incorrect = new ToolStripMenuItem(word);
            incorrect.Image = Image.FromFile("../../assets/icons/crossIcon.png");
            incorrect.Click += (s, e) =>
            {
                SuggestionsShower.Show();
            };

            this.Items.Add(incorrect);
            this.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem suggestion;

            foreach(string sug in suggestions)
            {
                suggestion = new ToolStripMenuItem(sug);

                suggestion.Click += (s, e) =>
                {
                    SuggestionsShower.ReplaceWord(sug);
                    TextArea.Text = SuggestionsShower.Text;
                    SuggestionsShower.Show();
                };

                this.Items.Add(suggestion);
            }
        }
    }

    public static class SuggestionsShower
    {
        public static string BeforeSelection = "";

        public static string AfterSelection = "";

        public static string Selection = "";

        public static string Text
        {
            get => $"{BeforeSelection}{Selection}{AfterSelection}";
        }

        public static TextArea TextArea;

        private static int wordStart = -1;

        private static int wordEnd = -1;

        private static readonly char[] delimiters = new char[] { ' ', '\n' };

        public static void Reset()
        {
            BeforeSelection = Selection = AfterSelection = "";
            wordStart = wordEnd = -1;
        }

        public static void Show()
        {
            if (wordEnd == -1)
            {
                wordStart = 0;

                wordEnd = Selection.IndexOfAny(delimiters);
                if (wordEnd == -1)
                {
                    wordEnd = Selection.Length - 1;
                }
                else
                {
                    wordEnd--;
                }
            }
            else
            {
                wordStart = wordEnd + 1;
                while(wordStart < Selection.Length
                    && delimiters.Contains(Selection[wordStart]))
                {
                    wordStart++;
                }

                if (wordStart == Selection.Length)
                {
                    SetCursorToWordEnd();
                    Reset();
                    return;
                }

                wordEnd = Selection.IndexOfAny(delimiters, wordStart);
                if (wordEnd == -1)
                {
                    wordEnd = Selection.Length - 1;
                }
                else
                {
                    wordEnd--;
                }
            }

            string word = Selection.Substring(wordStart, wordEnd - wordStart + 1);

            SetCursorToWordEnd();

            Point target = TextArea.GetPositionFromCharIndex(
                BeforeSelection.Length + wordStart);

            target.X += 5;
            target.Y += 15;

            if (OrthographyChecker.IsCorrect(word))
            {
                TextArea.ShowCorrect(word, target);
            }
            else
            {
                TextArea.ShowSuggestions(word,
                    OrthographyChecker.GetSimilar(word), target);
            }
        }

        public static void ReplaceWord(string newWord)
        {
            if (wordEnd == Selection.Length - 1)
            {
                Selection = Selection.Substring(0, wordStart) + newWord;
            }
            else
            {
                Selection = Selection.Substring(0, wordStart)
                    + newWord + Selection.Substring(wordEnd + 1);
            }

            wordEnd = wordStart + newWord.Length - 1;
        }

        public static void SetCursorToWordEnd()
        {
            int position = Math.Min(
                BeforeSelection.Length + wordEnd + 1, TextArea.Text.Length);
            TextArea.SelectionStart = position;
            TextArea.ScrollToCaret();
        }
    }
}
