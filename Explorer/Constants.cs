﻿using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace Explorer
{
    public static class Constants
    {
        public const int MainWrapperMargin = 10;

        public const int MainWrapperPadding = 10;

        public const int SpaceBetweenViews = 20;

        public const int MainMenuMarginBottom = 10;

        public const int BottomButtonsHeight = 50;

        public const int ViewItemHeight = 30;

        public const int ViewPadding = 20;
        
        public const int SearchBoxWidth = 300;
        
        public static readonly Font ViewItemFont = new Font("Verdana", 12);

        public static readonly Size FileSystemNodeImageSize = new Size(18, 18);

        /// <summary>
        /// Specifies indexes of types of file system elements icons in ImageList.
        /// </summary>
        public static class IconTypeIndexes
        {
            public const int DriveIndex = 0;
            public const int FolderIndex = 1;
            public const int FileIndex = 2;
            public const int BackToFolderIndex = 3;
            public const int CurrentLocationIndex = 4;
            public const int MoveToIndex = 5;
            public const int MergeIndex = 6;
            public const int NoImageIndex = 7;
        }
    }

    public static class BufferElementState
    {
        public const int Empty = 0;
        public const int Copied = 1;
        public const int Cut = 2;
        public const int Moving = 3;
    }
}
