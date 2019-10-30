using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public interface IFileSystemListItem : IFileSystemItem
    {
        IFileSystemTreeNode Node { get; }

        IFileSystemList List { get; }

        bool IsAccessible { get; }
        
        int RealWidth { get; set; }

        FileSystemItemPresenter Presenter { get; set; }

        void ShowMenu();

        Action Open { get; }

        void CalcRealWidth();
        
        void CalcRealWidth(string name);

        bool Selected { get; set; }

        void MarkAsInaccessible();

        void Display();

        void MoveHere();

        bool IsFileSystemItem();
    }
}