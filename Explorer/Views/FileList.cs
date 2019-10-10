using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.Views
{
    class FileList : ListView
    {
        public FileList() : base()
        {
            this.Dock = DockStyle.Fill;
            this.Font = Globals.ViewItemFont;
            this.BorderStyle = BorderStyle.None;
        }


    }
}
