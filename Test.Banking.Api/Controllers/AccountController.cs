using Microsoft.AspNetCore.Mvc;
using Test.Banking.Api.Abstractions.Services;
using Test.Banking.Api.Types;

namespace Test.Banking.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService accountService;
    
    public AccountController(IAccountService accountService)
    {
        this.accountService = accountService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> CreateAccount(Account account)
    {
        var result = await this.accountService.Create(account);
        return result.Errors.Any() ? this.UnprocessableEntity(result.Errors) : this.Created($"/account/{account.Id}", result.Result);
    }
    
    [HttpGet]
    [Route("{accountId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ReadAccount(int accountId)
    {
        var result = await this.accountService.Read(accountId);
        return result.Result is null ? this.NotFound() : this.Ok(result.Result);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ReadUserAccounts([FromQuery] int userId)
    {
        var result = await this.accountService.ReadUserAccounts(userId);
        return this.Ok(result.Result);
    }
    
    [HttpPost]
    [Route("{accountId:int}/Withdrawal")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> WithdrawalAccount(int accountId, AccountUpdate update)
    {
        var result = await this.accountService.Withdrawal(accountId, update.Amount);
        if (result.Result is null)
        {
            return this.NotFound();
        }

        return result.Errors.Any() ? this.UnprocessableEntity(result.Errors) : this.Ok(result.Result);
    }
    
    [HttpPost]
    [Route("{accountId:int}/Deposit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DepositAccount(int accountId, AccountUpdate update)
    {
        var result = await this.accountService.Deposit(accountId, update.Amount);
        if (result.Result is null)
        {
            return this.NotFound();
        }

        return result.Errors.Any() ? this.UnprocessableEntity(result.Errors) : this.Ok(result.Result);
    }
    
    [HttpDelete]
    [Route("{accountId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAccount(int accountId)
    {
        await this.accountService.Delete(accountId);
        return this.NoContent();
    }
}