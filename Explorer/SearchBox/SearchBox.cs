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

        private readonly CheckBox SearchOnlyCheckBox;

        public bool SearchOnly { get => SearchOnlyCheckBox.Checked; }

        public SearchBox(CheckBox searchOnly, IFileSystemList list) : base()
        {
            SearchOnlyCheckBox = searchOnly;
            List = list;
            List.SearchBox = this;
         
            this.Font = new Font("Verdana", 15);

            this.TextChanged += (s, e) =>
            {
                List.UpdateRefresh();
            };
        }
    }
}
