using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Views
{
    public interface IFileSystemItem
    {
        string Name { get; set; }

        IFileSystemItemEntity Entity { get; }

        /// <summary>
        /// Adds new option to right-click menu.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="onClick"></param>
        void AddContextMenuOption(string name, Action onClick);

        void StartNameEditing();

        void ShowProperties();
    }
}
