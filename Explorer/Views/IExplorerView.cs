﻿using System.Collections.Generic;

namespace Explorer.Views
{
    /// <summary>
    /// Provides an interface to interact with <see cref="Explorer"/>.
    /// </summary>
    public interface IExplorerView
    {
        /// <summary>
        /// Runs an application.
        /// </summary>
        void Mount();
    }
}
