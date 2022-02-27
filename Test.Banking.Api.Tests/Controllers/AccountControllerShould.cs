using System.Collections.Generic;
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

public class AccountControllerShould
{
    private readonly AccountController controller;

    private readonly IAccountService mockAccountService;

    private readonly Fixture fixture;

    public AccountControllerShould()
    {
        this.mockAccountService = Substitute.For<IAccountService>();
        this.fixture = new Fixture();
        this.controller = new AccountController(mockAccountService);
    }

    [Fact]
    public async Task CreatAccount()
    {
        var mockResponse = fixture.Create<ServiceResult<Account>>();
        mockResponse.Errors = new List<string>();
        var mockAccount = fixture.Create<Account>();
        this.mockAccountService.Create(Arg.Any<Account>()).Returns(mockResponse);
        
        var result = await this.controller.CreateAccount(mockAccount);
        
        (result as CreatedResult).StatusCode.Should().Be(201);
        (result as CreatedResult).Value.Should().BeEquivalentTo(mockResponse.Result);
    }
    
    [Fact]
    public async Task NotCreatAccountIfErrors()
    {
        var mockResponse = fixture.Create<ServiceResult<Account>>();
        var mockAccount = fixture.Create<Account>();
        this.mockAccountService.Create(Arg.Any<Account>()).Returns(mockResponse);

        var result = await this.controller.CreateAccount(mockAccount);
        
        (result as UnprocessableEntityObjectResult).StatusCode.Should().Be(422);
        (result as UnprocessableEntityObjectResult).Value.Should().BeEquivalentTo(mockResponse.Errors);
    }
    
    [Fact]
    public async Task ReturnAccountIfFound()
    {
        var mockResponse = fixture.Create<ServiceResult<Account>>();
        mockResponse.Errors = new List<string>();
        this.mockAccountService.Read(Arg.Any<int>()).Returns(mockResponse);

        var result = await this.controller.ReadAccount(1);
        
        (result as OkObjectResult).StatusCode.Should().Be(200);
        (result as OkObjectResult).Value.Should().BeEquivalentTo(mockResponse.Result);
    }
    
    [Fact]
    public async Task Return404IfAccountNotFound()
    {
        var mockResponse = fixture.Create<ServiceResult<Account?>>();
        mockResponse.Result = null;
        this.mockAccountService.Read(Arg.Any<int>()).Returns(mockResponse);

        var result = await this.controller.ReadAccount(1);

        (result as NotFoundResult).StatusCode.Should().Be(404);
    }
    
    [Fact]
    public async Task ReturnAllAccountsForUser()
    {
        var mockResponse = fixture.Create<ServiceResult<List<Account>>>();
        mockResponse.Errors = new List<string>();
        this.mockAccountService.ReadUserAccounts(Arg.Any<int>()).Returns(mockResponse);

        var result = await this.controller.ReadUserAccounts(1);
        
        (result as OkObjectResult).StatusCode.Should().Be(200);
        (result as OkObjectResult).Value.Should().BeEquivalentTo(mockResponse.Result);
    }

    [Fact]
    public async Task WithdrawalFromAccount()
    {
        var mockResponse = fixture.Create<ServiceResult<Account>>();
        mockResponse.Errors = new List<string>();
        this.mockAccountService.Withdrawal(Arg.Any<int>(), Arg.Any<double>()).Returns(mockResponse);

        var result = await this.controller.WithdrawalAccount(1, new AccountTransaction(100));
        
        (result as OkObjectResult).StatusCode.Should().Be(200);
        (result as OkObjectResult).Value.Should().BeEquivalentTo(mockResponse.Result);
    }
    
    [Fact]
    public async Task ReturnErrorsOnInvalidWithdrawal()
    {
        var mockResponse = fixture.Create<ServiceResult<Account>>();
        this.mockAccountService.Withdrawal(Arg.Any<int>(), Arg.Any<double>()).Returns(mockResponse);

        var result = await this.controller.WithdrawalAccount(1, new AccountTransaction(100));
        
        (result as UnprocessableEntityObjectResult).StatusCode.Should().Be(422);
        (result as UnprocessableEntityObjectResult).Value.Should().BeEquivalentTo(mockResponse.Errors);
    }
    
    [Fact]
    public async Task Return404IfAccountNotFoundOnWithdrawal()
    {
        var mockResponse = fixture.Create<ServiceResult<Account?>>();
        mockResponse.Result = null;
        this.mockAccountService.Withdrawal(Arg.Any<int>(), Arg.Any<double>()).Returns(mockResponse);

        var result = await this.controller.WithdrawalAccount(1, new AccountTransaction(1));

        (result as NotFoundResult).StatusCode.Should().Be(404);
    }
    
    [Fact]
    public async Task DepositToAccount()
    {
        var mockResponse = fixture.Create<ServiceResult<Account>>();
        mockResponse.Errors = new List<string>();
        this.mockAccountService.Deposit(Arg.Any<int>(), Arg.Any<double>()).Returns(mockResponse);

        var result = await this.controller.DepositAccount(1, new AccountTransaction(100));
        
        (result as OkObjectResult).StatusCode.Should().Be(200);
        (result as OkObjectResult).Value.Should().BeEquivalentTo(mockResponse.Result);
    }
    
    [Fact]
    public async Task ReturnErrorsOnInvalidDeposit()
    {
        var mockResponse = fixture.Create<ServiceResult<Account>>();
        this.mockAccountService.Deposit(Arg.Any<int>(), Arg.Any<double>()).Returns(mockResponse);

        var result = await this.controller.DepositAccount(1, new AccountTransaction(100));
        
        (result as UnprocessableEntityObjectResult).StatusCode.Should().Be(422);
        (result as UnprocessableEntityObjectResult).Value.Should().BeEquivalentTo(mockResponse.Errors);
    }
    
    [Fact]
    public async Task Return404IfAccountNotFoundOnDeposit()
    {
        var mockResponse = fixture.Create<ServiceResult<Account?>>();
        mockResponse.Result = null;
        this.mockAccountService.Deposit(Arg.Any<int>(), Arg.Any<double>()).Returns(mockResponse);

        var result = await this.controller.DepositAccount(1, new AccountTransaction(1));

        (result as NotFoundResult).StatusCode.Should().Be(404);
    }
    
    [Fact]
    public async Task DeleteAccount()
    {
        var mockResponse = fixture.Create<ServiceResult<Account>>();
        var mockAccount = fixture.Create<Account>();
        this.mockAccountService.Delete(Arg.Any<int>()).Returns(mockResponse);

        var result = await this.controller.DeleteAccount(1);
        
        (result as NoContentResult).StatusCode.Should().Be(204);
    }
}