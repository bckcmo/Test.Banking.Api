using Microsoft.EntityFrameworkCore;
using Test.Banking.Api.Types;

namespace Test.Banking.Api.Repositories.DbContexts;

public class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    protected AccountDbContext()
    {
    }
        
    public DbSet<Account> Accounts { get; set; }

}