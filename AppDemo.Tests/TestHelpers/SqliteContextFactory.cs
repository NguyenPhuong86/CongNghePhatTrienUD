using AppDemo.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AppDemo.Tests.TestHelpers;

internal sealed class SqliteContextScope : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    public ApplicationDbContext Context { get; }

    public SqliteContextScope()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;
        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
