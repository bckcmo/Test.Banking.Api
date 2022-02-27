using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Test.Banking.Api.Repositories;
using Test.Banking.Api.Repositories.DbContexts;
using Test.Banking.Api.Types;
using Xunit;

namespace Test.Banking.Api.Tests.Repositories;

public class AccountRepositoryShould
{
    private readonly AccountRepository accountRepository;

    private readonly Fixture fixture;

    private readonly AccountDbContext testDbContext;

    public AccountRepositoryShould()
    {
        this.fixture = new Fixture();

        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();
        var builder = new DbContextOptionsBuilder<AccountDbContext>();
        builder.UseInMemoryDatabase("test")
            .UseInternalServiceProvider(serviceProvider);
        this.testDbContext = new AccountDbContext(builder.Options);
        
        this.accountRepository = new AccountRepository(testDbContext);
    }

    [Fact]
    public async Task CreateAccount()
    {
        var newAccount = this.fixture.Create<Account>();
        newAccount.Id = 0;

        var result = await this.accountRepository.Create(newAccount);

        result.Id.Should().NotBe(0);
        result.Balance.Should().Be(newAccount.Balance);
    }
    
    [Fact]
    public async Task CreateAccountWithId()
    {
        var newAccount = this.fixture.Create<Account>();
        newAccount.Id = 101;

        var result = await this.accountRepository.Create(newAccount);

        result.Should().BeEquivalentTo(newAccount);
    }
    
    [Fact]
    public async Task ReadAccount()
    {
        var newAccount = this.fixture.Create<Account>();
        this.testDbContext.Add(newAccount);
        await this.testDbContext.SaveChangesAsync();
        
        var result = await this.accountRepository.Read(newAccount.Id);

        result.Should().BeEquivalentTo(newAccount);
    }
    
    [Fact]
    public async Task ReadAllAccountsForUser()
    {
        var userId = 10293;
        var newAccounts = this.fixture.CreateMany<Account>(10).ToList();

        foreach (var account in newAccounts)
        {
            account.UserId = userId;
            testDbContext.Add(account);
        }
        
        await this.testDbContext.SaveChangesAsync();

        var result = await this.accountRepository.ReadAllForUser(userId);

        result.Should().BeEquivalentTo(newAccounts);
    }
    
    [Fact]
    public async Task UpdateAccountBalance()
    {
        var updatedBalance = 1000.89;
        var newAccount = this.fixture.Create<Account>();
        await this.testDbContext.AddAsync(newAccount);
        await this.testDbContext.SaveChangesAsync();

        await this.accountRepository.UpdateBalance(newAccount.Id, updatedBalance);
        
        var updatedAccount = this.testDbContext.Accounts.Find(newAccount.Id);

        updatedAccount?.Balance.Should().Be(updatedBalance);
    }
    
    [Fact]
    public async Task DeleteAccount()
    {
        var newAccount = this.fixture.Create<Account>();
        await this.testDbContext.AddAsync(newAccount);
        await this.testDbContext.SaveChangesAsync();
        
        await this.accountRepository.Delete(newAccount.Id);

        var deletedAccount = this.testDbContext.Accounts.Find(newAccount.Id);

        deletedAccount.Should().BeNull();
    }
}