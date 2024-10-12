using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bb.Folders
{

    /// <summary>
    /// Folder explorer
    /// </summary>
    public partial class FolderExplorer : ComponentBase
    {

        /// <summary>
        /// Gets or sets the path to explore.
        /// </summary>
        [Parameter]
        public string Path { get; set; }

        protected override void OnInitialized()
        {

            var p = new DirectoryInfo(Path);

            if (p.Exists)
            {
                p.Refresh();
                TreeItems.Add(new FileItemData(p));
            }
        }

        private HashSet<FileItemData> TreeItems1 { get; set; } = new HashSet<FileItemData>();
        private IReadOnlyCollection<TreeItemData<FileItemData>> TreeItems { get; set; } = new HashSet<FileItemData>();

    }
}
