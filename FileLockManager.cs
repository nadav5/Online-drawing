public class FileLockManager
{
    private static FileLockManager _instance = null;
    private static readonly object instanceLock = new object();

    private readonly HashSet<string> lockedFiles = new HashSet<string>();
    private readonly object lockObj = new object();

    private FileLockManager() { }

    public static FileLockManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new FileLockManager();
                    }
                }
            }
            return _instance;
        }
    }

    public bool TryLock(string fileName)
    {
        lock (lockObj)
        {
            if (lockedFiles.Contains(fileName))
                return false;

            lockedFiles.Add(fileName);
            return true;
        }
    }

    public void Release(string fileName)
    {
        lock (lockObj)
        {
            lockedFiles.Remove(fileName);
        }
    }

    public bool IsLocked(string fileName)
    {
        lock (lockObj)
        {
            return lockedFiles.Contains(fileName);
        }
    }
}
