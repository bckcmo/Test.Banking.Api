using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Test.Banking.Api.Repositories;
using Test.Banking.Api.Repositories.DbContexts;
using Test.Banking.Api.Types;
using Xunit;

namespace Test.Banking.Api.Tests.Repositories;

public class UserRepositoryShould
{
    private readonly UserRepository userRepository;

    private readonly Fixture fixture;

    private readonly UserDbContext testDbContext;

    public UserRepositoryShould()
    {
        this.fixture = new Fixture();
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: "Test")
            .Options;
        this.testDbContext = new UserDbContext(options);
        this.userRepository = new UserRepository(testDbContext);
    }

    [Fact]
    public async Task CreateUser()
    {
        var newUser = this.fixture.Create<User>();
        newUser.Id = 0;

        var result = await this.userRepository.Create(newUser);
        
        result.Id.Should().NotBe(0);
        result.UserName.Should().Be(newUser.UserName);
    }

    [Fact]
    public async Task CreateUserWithId()
    {
        var newUser = this.fixture.Create<User>();
        newUser.Id = 101;

        var result = await this.userRepository.Create(newUser);

        result.Should().BeEquivalentTo(newUser);
    }

    [Fact]
    public async Task ReadUser()
    {
        var newUser = this.fixture.Create<User>();
        this.testDbContext.Add(newUser);
        await this.testDbContext.SaveChangesAsync();
        
        var result = await this.userRepository.Read(newUser.Id);

        result.Should().BeEquivalentTo(newUser);
    }
}