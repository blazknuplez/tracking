using Tracking.Ef.Entities;

namespace Tracking.Ef.DataSeed;

internal static class AccountsDataSeed
{
    public static IEnumerable<Account> Data => new[]
    {
        new Account { Id = 1, Name = "Active account", IsActive = true },
        new Account { Id = 2, Name = "Inactive account", IsActive = false },
        new Account { Id = 3, Name = "Another active account", IsActive = true },
    };
}