using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorer
{
    public class SearchBox : TextBox
    {
        private readonly IFileSystemList List;

        public SearchBox(IFileSystemList list) : base()
        {
            List = list;
            List.SearchBox = this;
            
            this.Font = new Font("Verdana", 15);

            this.TextChanged += (s, e) =>
            {
                List.UpdateRefresh();
            };
        }

        private List<int> SearchByKeywords(string query, string text)
        {
            string[] keywords = query.Split(
                  new char[] { ' ' },
                  StringSplitOptions.RemoveEmptyEntries);

            string[] textwords = text.Split(new char[] { ' ' });

            List<int> matches = new List<int>(textwords.Length);

            for(int i = 0; i < textwords.Length; i++)
            {
                matches.Add(0);
            }

            foreach (string keyword in keywords)
            {
                for (int i = 0; i < textwords.Length; i++)
                {
                    string textword = textwords[i];
                    int idx = textword.IndexOf(keyword);

                    if (textword == keyword || idx == 0)
                    {
                        matches[i] = Math.Max(matches[i], keyword.Length);
                    }
                }
            }

            return matches;
        }

    }
}
