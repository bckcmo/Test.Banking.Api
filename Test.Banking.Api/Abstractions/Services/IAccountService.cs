using Test.Banking.Api.Types;

namespace Test.Banking.Api.Abstractions.Services;

public interface IAccountService
{
    Task<ServiceResult<Account>> Create(Account account);

    Task<ServiceResult<Account?>> Read(int id);

    Task<ServiceResult<List<Account>>> ReadUserAccounts(int userId);

    Task<ServiceResult<Account>> Withdrawal(int id, double amount);
    
    Task<ServiceResult<Account>> Deposit(int id, double amount);

    Task<ServiceResult<Account>> Delete(int id);
}