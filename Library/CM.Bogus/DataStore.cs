namespace CM.Bogus;

/// <summary>
/// Provides in-memory storage of seed data for development and testing.
/// </summary>
internal static class DataStore
{
    private static readonly List<Checkable> _checkables;
    private static readonly List<CheckList> _checkLists;
    private static readonly List<User> _users;

    static DataStore()
    {
        _users = DataSeed.GetUsers().ToList();
        _checkables = DataSeed.GetCheckables().ToList();
        _checkLists = DataSeed.GetCheckLists().ToList();
    }

    internal static List<Checkable> Checkables => _checkables;
    internal static List<CheckList> CheckLists => _checkLists;
    internal static Lock SyncRoot { get; } = new();
    internal static List<User> Users => _users;
}
