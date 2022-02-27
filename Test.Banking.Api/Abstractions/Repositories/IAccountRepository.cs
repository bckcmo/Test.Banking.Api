using Test.Banking.Api.Types;

namespace Test.Banking.Api.Abstractions.Repositories;

public interface IAccountRepository
{
    public Task<Account> Create(Account account);

    public Task<Account?> Read(long id);

    Task<List<Account>> ReadAllForUser(int userId);

    public Task UpdateBalance(long id, double newBalance);

    public Task<Account?> Delete(long id);
}