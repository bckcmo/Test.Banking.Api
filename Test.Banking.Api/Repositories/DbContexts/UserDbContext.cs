using Microsoft.EntityFrameworkCore;
using Test.Banking.Api.Types;

namespace Test.Banking.Api.Repositories.DbContexts;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    protected UserDbContext()
    {
    }
    
    public DbSet<User?> Users { get; set; }
}