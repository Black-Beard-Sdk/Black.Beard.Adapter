using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bb.Folders
{

    /// <summary>
    /// Folder explorer
    /// </summary>
    public partial class FolderExplorer : ComponentBase
    {

        [Parameter]
        public string Path { get; set; }

        protected override void OnInitialized()
        {

            var p = new DirectoryInfo(Path);
            if (p .Exists)
                TreeItems.Add(new FileItemData(p));

        }

        private HashSet<FileItemData> TreeItems { get; set; } = new HashSet<FileItemData>();

    }
}
