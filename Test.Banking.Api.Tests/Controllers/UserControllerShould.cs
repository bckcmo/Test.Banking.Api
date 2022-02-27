using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Test.Banking.Api.Abstractions.Services;
using Test.Banking.Api.Controllers;
using Test.Banking.Api.Types;
using Xunit;

namespace Test.Banking.Api.Tests.Controllers;

public class UserControllerShould
{
    private readonly UserController controller;
    
    private readonly IUserService mockUserService;

    private readonly Fixture fixture;
    
    public UserControllerShould()
    {
        this.mockUserService = Substitute.For<IUserService>();
        this.fixture = new Fixture();
        this.controller = new UserController(mockUserService);
    }

    [Fact]
    public async Task CreateUser()
    {
        var mockResponse = fixture.Create<ServiceResult<User?>>();
        var mockUser = fixture.Create<User>();
        mockUserService.Create(Arg.Any<User>()).Returns(mockResponse);

        var result = await controller.CreateUser(mockUser);

        (result as CreatedResult).StatusCode.Should().Be(201);
        (result as CreatedResult).Value.Should().BeEquivalentTo(mockResponse.Result);
    }

    [Fact]
    public async Task ReturnUserIfFound()
    {
        var mockResponse = fixture.Create<ServiceResult<User?>>();
        this.mockUserService.Read(Arg.Any<int>()).Returns(mockResponse);

        var result = await this.controller.ReadUser(1);

        (result as OkObjectResult).StatusCode.Should().Be(200);
        (result as OkObjectResult).Value.Should().BeEquivalentTo(mockResponse.Result);
    }
    
    [Fact]
    public async Task Return404IfUserNotFound()
    {
        var mockResponse = fixture.Create<ServiceResult<User?>>();
        mockResponse.Result = null;
        this.mockUserService.Read(Arg.Any<int>()).Returns(mockResponse);

        var result = await this.controller.ReadUser(1);

        (result as NotFoundResult).StatusCode.Should().Be(404);
    }
}