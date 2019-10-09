using System;
using System.IO;
using Explorer.Views;

namespace Explorer.Presenters
{
    /// <summary>
    /// Implements <see cref="IExplorerPresenter"/>.
    /// </summary>
    class ExplorerPresenter
    {
        private IExplorer View { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ExplorerPresenter"/>.
        /// </summary>
        /// <param name="View"></param>
        public ExplorerPresenter(IExplorer view)
        {
            View = view;
        }

        /// <summary>
        /// Invokes application's run.
        /// </summary>
        public void Run()
        {
            View.Mount();
        }

        //public void CheckAccessibility(FileSystemNode node)
        //{
        //    //if (!node.IsAccessible) _Node.ShowModal();
        //}

        
    }
}