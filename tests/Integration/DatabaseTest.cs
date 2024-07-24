using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Integration;

public class DatabaseTest
{
    public AppDbContext DbContext { get; private set; }
    public Mock<ICurrentUserContext> CurrentUserContextMock { get; private set; }
    private readonly SqliteConnection _connection;
    private readonly ApplicationUser _user;

    public DatabaseTest(IUser user)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        CurrentUserContextMock = new Mock<ICurrentUserContext>();

        DbContext = new AppDbContext(options);
        DbContext.SetCurrentUser(CurrentUserContextMock.Object); DbContext.Database.EnsureCreated();
        _user = (ApplicationUser)user;
        DbContext.Users.Add(_user);
        DbContext.SaveChanges();
        CurrentUserContextMock.Setup(c => c.GetCurrentUserId()).Returns(_user.Id);
    }

    public DatabaseTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        CurrentUserContextMock = new Mock<ICurrentUserContext>();

        DbContext = new AppDbContext(options);
        DbContext.SetCurrentUser(CurrentUserContextMock.Object);
        DbContext.Database.EnsureCreated();
    }
}