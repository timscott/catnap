namespace Catnap.Sqlite
{
    public enum SqliteResult
    {
        OK = 0,
        Error = 1,
        Row = 100,
        Done = 101,
        Internal = 2,
        Perm = 3,
        Abort = 4,
        Busy = 5,
        Locked = 6,
        NoMem = 7,
        ReadOnly = 8,
        Interrupt = 9,
        IOError = 10,
        Corrupt = 11,
        NotFound = 12,
        TooBig = 18,
        Constraint = 19,
    }
}