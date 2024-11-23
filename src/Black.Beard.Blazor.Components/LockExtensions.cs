namespace Bb
{
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
