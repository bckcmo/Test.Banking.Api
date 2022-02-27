using Microsoft.EntityFrameworkCore;
using Test.Banking.Api.Abstractions.Repositories;
using Test.Banking.Api.Exceptions;
using Test.Banking.Api.Repositories.DbContexts;
using Test.Banking.Api.Types;

namespace Test.Banking.Api.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountDbContext context;

    public AccountRepository(AccountDbContext context)
    {
        this.context = context;
    }
    
    public async Task<Account> Create(Account account)
    {
        try
        {
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }
        catch (ArgumentException)
        {
            throw new RepositoryException($"Account with id {account.Id} already exists.");
        }

        return account;
    }

    public async Task<Account?> Read(long id)
    {
        return await context.Accounts.FindAsync(id);
    }
    
    public async Task<List<Account>> ReadAllForUser(int userId)
    {
        return await context.Accounts.Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task UpdateBalance(long id, double newBalance)
    {
        var account = await context.Accounts.FindAsync(id);

        if (account is not null)
        {
            account.Balance = newBalance;
            await context.SaveChangesAsync();
        }
    }

    public async Task<Account?> Delete(long id)
    {
        var account = await context.Accounts.FindAsync(id);
        
        if (account is not null)
        {
            context.Accounts.Remove(account);
            await context.SaveChangesAsync();
        }

        return account;
    }
}