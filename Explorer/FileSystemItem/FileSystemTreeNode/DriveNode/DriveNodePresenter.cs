﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    public class DriveNodePresenter : DirectoryNodePresenter
    {
        public DriveNodePresenter(IFileSystemTreeNode view) : base(view)
        {
        }
    }
}
