using MudBlazor;

namespace Bb.Folders
{
    public class FileItemData
    {


        #region ctor

        public FileItemData(DirectoryInfo dir)
        {
            Text = dir.Name;
            this._dir = dir;
            _instance = dir;
        }

        public FileItemData(FileInfo file)
        {
            Text = file.Name;
            _file = file;
            _instance = file;
        }

        #endregion ctor


        public string Text { get; }

        public string? Icon => _dir == null
            ? @Icons.Custom.FileFormats.FileDocument
            : !HasChild ? Icons.Custom.Uncategorized.Folder : null;

        public string? ExpandedIcon => _dir != null && HasChild
            ? Icons.Custom.Uncategorized.FolderOpen
            : null;

        public bool IsExpanded { get; set; }

        public bool HasChild => TreeItems != null && TreeItems.Count > 0;

        public HashSet<FileItemData> TreeItems
        {
            get
            {

                if (_o == null)
                {

                    _o = new HashSet<FileItemData>();

                    if (_dir != null)
                    {

                        foreach (var item in _dir.GetDirectories())
                            _o.Add(new FileItemData(item));

                        foreach (var item in _dir.GetFiles())
                            _o.Add(new FileItemData(item));

                    }

                }

                return _o;
            }
        }

        public void Refresh()
        {
            _instance.Refresh();
            _o = null;
        }

        /// <summary>
        ///  Gets a value indicating whether the directory exists.
        /// </summary>
        /// <returns>true if the directory exists; otherwise, false.</returns>
        public bool Exists => _instance.Exists;

        /// <summary>
        /// The file is read-only. ReadOnly is supported on Windows, Linux, and macOS. On
        /// Linux and macOS, changing the ReadOnly flag is a permissions operation.
        /// </summary>
        public bool IsReadOnly => _instance.Attributes.HasFlag(FileAttributes.ReadOnly);

        /// <summary>
        /// The file is hidden, and thus is not included in an ordinary directory listing.
        //  Hidden is supported on Windows, Linux, and macOS.
        /// </summary>
        public bool IsHidden => _instance.Attributes.HasFlag(FileAttributes.Hidden);

        /// <summary>
        /// The file is a system file. That is, the file is part of the operating system
        //  or is used exclusively by the operating system.
        /// </summary>
        public bool IsSystem => _instance.Attributes.HasFlag(FileAttributes.System);

        /// <summary>
        /// The file is a directory. Directory is supported on Windows, Linux, and macOS.
        /// </summary>
        public bool IsDirectory => _instance.Attributes.HasFlag(FileAttributes.Directory);

        /// <summary>
        /// This file is marked to be included in incremental backup operation. Windows sets
        /// this attribute whenever the file is modified, and backup software should clear
        /// it when processing the file during incremental backup.
        /// </summary>
        public bool IsArchive => _instance.Attributes.HasFlag(FileAttributes.Archive);

        /// <summary>
        /// The file is a standard file that has no special attributes. This attribute is
        /// valid only if it is used alone. Normal is supported on Windows, Linux, and macOS.
        /// </summary>
        public bool IsNormal => _instance.Attributes.HasFlag(FileAttributes.Normal);

        /// <summary>
        ///  The file is temporary. A temporary file contains data that is needed while an
        ///  application is executing but is not needed after the application is finished.
        ///  File systems try to keep all the data in memory for quicker access rather than
        ///  flushing the data back to mass storage. A temporary file should be deleted by
        ///  the application as soon as it is no longer needed.
        /// </summary>
        public bool IsTemporary => _instance.Attributes.HasFlag(FileAttributes.Temporary);

        /// <summary>
        /// The file is compressed.
        /// </summary>
        public bool IsCompressed => _instance.Attributes.HasFlag(FileAttributes.Compressed);

        /// <summary>
        /// The file or directory is encrypted. For a file, this means that all data in the
        /// file is encrypted. For a directory, this means that encryption is the default
        /// for newly created files and directories.
        /// </summary>
        public bool IsEncrypted => _instance.Attributes.HasFlag(FileAttributes.Encrypted);


        public FileSystemInfo FileSystemInfo => _instance;


        private readonly DirectoryInfo _dir;
        private readonly FileInfo _file;
        private HashSet<FileItemData>? _o;
        private FileSystemInfo _instance;

    }
}
