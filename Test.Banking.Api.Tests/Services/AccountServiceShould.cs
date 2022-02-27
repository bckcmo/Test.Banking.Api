using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Test.Banking.Api.Abstractions.Repositories;
using Test.Banking.Api.Exceptions;
using Test.Banking.Api.Services;
using Test.Banking.Api.Types;
using Xunit;

namespace Test.Banking.Api.Tests.Services;

public class AccountServiceShould
{
    private readonly AccountService accountService;

    private readonly IAccountRepository mockAccountRepository;

    private readonly Fixture fixture;

    public AccountServiceShould()
    {

        this.mockAccountRepository = Substitute.For<IAccountRepository>();
        this.fixture = new Fixture();
        this.accountService = new AccountService(mockAccountRepository);
    }

    [Fact]
    public async Task CreateAccount()
    {
        var mockNewAccount = this.fixture.Create<Account>();
        this.mockAccountRepository.Create(Arg.Any<Account>()).Returns(mockNewAccount);
        var account = this.fixture.Create<Account>();
        account.Balance = 200;
        
        var result = await this.accountService.Create(account);

        result.Result.Should().BeEquivalentTo(mockNewAccount);
        result.Errors.Should().BeNullOrEmpty();
    }
    
    [Fact]
    public async Task CreateAccountErrorOnRepoException()
    {
        var mockNewAccount = this.fixture.Create<Account>();
        this.mockAccountRepository.Create(Arg.Any<Account>())
            .Throws(new RepositoryException("Bad"));
        var account = this.fixture.Create<Account>();
        account.Balance = 200;

        var result = await accountService.Create(account);
        
        result.Result.Should().BeNull();
        result.Errors.First().Should().Contain("Account Error");
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(50)]
    [InlineData(99)]
    public async Task NotCreateAccountWithInvalidBalance(double balance)
    {
        var account = this.fixture.Create<Account>();
        account.Balance = balance;
        
        var result = await this.accountService.Create(account);

        result.Result.Should().BeNull();
        result.Errors.First().Should().Contain("Invalid Balance");
    }
    
    [Fact]
    public async Task ReadAccount()
    {
        var mockAccount = this.fixture.Create<Account>();
        this.mockAccountRepository.Read(Arg.Any<long>()).Returns(mockAccount);

        var result = await this.accountService.Read(1);

        result.Result.Should().BeEquivalentTo(mockAccount);
        result.Errors.Should().BeNullOrEmpty();
    }
    
    [Fact]
    public async Task ReadAllAccountsForUser()
    {
        var mockAccounts = this.fixture.CreateMany<Account>().ToList();
        this.mockAccountRepository.ReadAllForUser(Arg.Any<int>()).Returns(mockAccounts);

        var result = await this.accountService.ReadUserAccounts(1);

        result.Result.Should().BeEquivalentTo(mockAccounts);
        result.Errors.Should().BeNullOrEmpty();
    }
    
    [Fact]
    public async Task WithdrawalFromAccount()
    {
        var mockExistingAccount = this.fixture.Create<Account>();
        mockExistingAccount.Balance = 1000;
        this.mockAccountRepository.Read(Arg.Any<long>()).Returns(mockExistingAccount);

        var result = await this.accountService.Withdrawal(1, 100);

        result.Result?.Balance.Should().Be(mockExistingAccount.Balance);
        result.Errors.Should().BeNullOrEmpty();
    }
    
    [Theory]
    [InlineData(100, 1)]
    [InlineData(1000, 901)]
    [InlineData(500, 500)]
    public async Task NotWithdrawalFromAccountIfInvalidBalance(double existingBalance, double withdrawal)
    {
        var mockExistingAccount = this.fixture.Create<Account>();
        mockExistingAccount.Balance = existingBalance;
        this.mockAccountRepository.Read(Arg.Any<long>()).Returns(mockExistingAccount);

        var result = await this.accountService.Withdrawal(1, withdrawal);

        result.Result?.Balance.Should().Be(mockExistingAccount.Balance);
        result.Errors.First().Should().Contain("Invalid Withdrawal");
    }
    
    [Theory]
    [InlineData(10000, 9001)]
    [InlineData(2000, 1999)]
    [InlineData(1011, 910)]
    public async Task NotWithdrawalFromAccountIfInvalidAmount(double existingBalance, double withdrawal)
    {
        var mockExistingAccount = this.fixture.Create<Account>();
        mockExistingAccount.Balance = existingBalance;
        this.mockAccountRepository.Read(Arg.Any<long>()).Returns(mockExistingAccount);

        var result = await this.accountService.Withdrawal(1, withdrawal);

        result.Result?.Balance.Should().Be(mockExistingAccount.Balance);
        result.Errors.First().Should().Contain("Invalid Withdrawal");
    }
    
    [Fact]
    public async Task ErrorOnWithdrawalIfNoAccount()
    {
        this.mockAccountRepository.Read(Arg.Any<long>()).ReturnsNull();

        var result = await this.accountService.Withdrawal(1, 100);

        result.Result.Should().BeNull();
        result.Errors.First().Should().Contain("Invalid Account");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10001)]
    [InlineData(2000000)]
    public async Task NotDepositFromAccountIfInvalidAmount(double deposit)
    {
        var mockExistingAccount = this.fixture.Create<Account>();
        this.mockAccountRepository.Read(Arg.Any<long>()).Returns(mockExistingAccount);

        var result = await this.accountService.Deposit(1, deposit);

        result.Result?.Balance.Should().Be(mockExistingAccount.Balance);
        result.Errors.First().Should().Contain("Invalid Deposit");
    }
    
    [Fact]
    public async Task ErrorOnDepositIfNoAccount()
    {
        this.mockAccountRepository.Read(Arg.Any<long>()).ReturnsNull();

        var result = await this.accountService.Deposit(1, 100);

        result.Result.Should().BeNull();
        result.Errors.First().Should().Contain("Invalid Account");
    }
}