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
        private readonly IExplorer _view;

        /// <summary>
        /// Initializes a new instance of <see cref="ExplorerPresenter"/>.
        /// </summary>
        /// <param name="view"></param>
        public ExplorerPresenter(IExplorer view)
        {
            _view = view;
        }

        /// <summary>
        /// Invokes application's run.
        /// </summary>
        public void Run()
        {
            _view.Mount();
        }

        //public void CheckAccessibility(FileSystemNode node)
        //{
        //    //if (!node.IsAccessible) _view.ShowModal();
        //}

        
    }
}