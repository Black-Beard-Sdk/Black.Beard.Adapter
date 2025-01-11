namespace Bb
{


    public class ConfigurationFolder : List<DirectoryInfo>
    {

        private ConfigurationFolder()
        {

        }

        public static void AddDirectoryIfExists(string folder)
        {
            AddDirectoryIfExists(new DirectoryInfo(folder));
        }

        public static void AddDirectoryIfExists(DirectoryInfo folder)
        {
            folder.Refresh();
            if (folder.Exists)
                Instance.Add(folder);
        }

        public static void AddDirectory(string folder)
        {
            AddDirectory(new DirectoryInfo(folder));
        }

        public static void AddDirectory(DirectoryInfo folder)
        {
            Instance.Add(folder);
        }

      

        public static string[] GetPaths()
        {

            List<string> result = new List<string>();

            foreach (var item in Instance)
                result.Add(item.FullName);

            return result.ToArray();

        }

        public static ConfigurationFolder Instance
        {
            get
            {

                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                        {
                            _instance = new ConfigurationFolder();
                            AddDirectoryIfExists(Path.Combine(AppContext.BaseDirectory, "Configs"));
                        }
                return _instance;
            }
        }

        private static ConfigurationFolder _instance;
        private static object _lock = new object();

    }






    public static class LockExtensions
    {

        public static IDisposable LockForRead(this ReaderWriterLockSlim locker, Action finallyBlock = null)
        {
            var result = new UpgradeableReadLockDisposable(UpgradeableReadLockDisposable.Mode.Read, locker, finallyBlock);
            locker.EnterReadLock();
            return result;
        }

        public static IDisposable LockForWrite(this ReaderWriterLockSlim locker, Action finallyBlock = null)
        {
            var result = new UpgradeableReadLockDisposable(UpgradeableReadLockDisposable.Mode.Write, locker, finallyBlock);
            locker.EnterWriteLock();
            return result;
        }

        public static IDisposable LockForUpgradeableRead(this ReaderWriterLockSlim locker, Action finallyBlock = null)
        {
            var result = new UpgradeableReadLockDisposable(UpgradeableReadLockDisposable.Mode.UpgradeableRead, locker, finallyBlock);
            locker.EnterUpgradeableReadLock();
            return result;
        }


        private struct UpgradeableReadLockDisposable : IDisposable
        {

            public UpgradeableReadLockDisposable(Mode mode, ReaderWriterLockSlim locker, Action finallyBlock)
            {
                this._mode = mode;
                this._locker = locker;
                this._finally = finallyBlock;
            }

            public void Dispose()
            {

                switch (this._mode)
                {

                    case Mode.Read:
                        _locker.ExitReadLock();
                        break;

                    case Mode.Write:
                        _locker.ExitWriteLock();
                        break;

                    case Mode.UpgradeableRead:
                        _locker.ExitUpgradeableReadLock();
                        break;

                    default:
                        break;

                }

                _finally?.Invoke();

            }

            private readonly Mode _mode;
            private ReaderWriterLockSlim _locker;
            private Action _finally;


            public enum Mode
            {
                Read,
                Write,
                UpgradeableRead,
            }

        }

    }

}
