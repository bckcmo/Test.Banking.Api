using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Test.Banking.Api.Abstractions.Repositories;
using Test.Banking.Api.Exceptions;
using Test.Banking.Api.Services;
using Test.Banking.Api.Types;
using Xunit;

namespace Test.Banking.Api.Tests.Services;

public class UserServiceShould
{
    private readonly UserService userService;

    private readonly IUserRepository mockUserRepository;

    private readonly Fixture fixture;

    public UserServiceShould()
    {
        this.mockUserRepository = Substitute.For<IUserRepository>();
        this.fixture = new Fixture();
        this.userService = new UserService(mockUserRepository);
    }

    [Fact]
    public async Task CreateUser()
    {
        var mockNewUser = this.fixture.Create<User>();
        this.mockUserRepository.Create(Arg.Any<User>()).Returns(mockNewUser);
        var user = this.fixture.Create<User>();
        
        var result = await this.userService.Create(user);

        result.Result.Should().BeEquivalentTo(mockNewUser);
        result.Errors.Should().BeNullOrEmpty();
    }
    
    [Fact]
    public async Task CreateAccountErrorOnRepoException()
    {
        var mockNewUser = this.fixture.Create<User>();
        this.mockUserRepository.Create(Arg.Any<User>())
            .Throws(new RepositoryException("Bad"));
        var account = this.fixture.Create<Account>();
        account.Balance = 200;

        var result = await userService.Create(mockNewUser);
        
        result.Result.Should().BeNull();
        result.Errors.First().Should().Contain("User Error");
    }
    
    [Fact]
    public async Task ReadUser()
    {
        var mockNewUser = this.fixture.Create<User>();
        this.mockUserRepository.Read(Arg.Any<int>()).Returns(mockNewUser);
        
        var result = await this.userService.Read(1);

        result.Result.Should().BeEquivalentTo(mockNewUser);
        result.Errors.Should().BeNullOrEmpty();
    }
}