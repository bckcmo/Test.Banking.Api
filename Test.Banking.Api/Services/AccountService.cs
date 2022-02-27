using Test.Banking.Api.Abstractions.Repositories;
using Test.Banking.Api.Abstractions.Services;
using Test.Banking.Api.Exceptions;
using Test.Banking.Api.Types;
using Test.Banking.Api.Utils;

namespace Test.Banking.Api.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        this.accountRepository = accountRepository;
    }
    
    public async Task<ServiceResult<Account>> Create(Account account)
    {
        var errors = new List<string>();
        Account? newAccount = null;
        
        TransactionValidator.ValidateBalance(account.Balance, TransactionType.Open, errors);
        TransactionValidator.ValidateDepositAmount(account.Balance, errors);

        if (!errors.Any())
        {
            try
            {
                newAccount = await this.accountRepository.Create(account);
            }
            catch (RepositoryException e)
            {
                errors.Add($"Account Error: {e.Message}");
            }
        }

        return new ServiceResult<Account>
        {
            Result = newAccount,
            Errors = errors,
        };
    }

    public async Task<ServiceResult<Account?>> Read(int id)
    {
        return new ServiceResult<Account?>
        {
            Result = await this.accountRepository.Read(id),
        };
    }
    
    public async Task<ServiceResult<List<Account>>> ReadUserAccounts(int userId)
    {
        return new ServiceResult<List<Account>>
        {
            Result = await this.accountRepository.ReadAllForUser(userId),
        };
    }

    public async Task<ServiceResult<Account>> Withdrawal(int id, double amount)
    {
        var account = await this.accountRepository.Read(id);

        return new ServiceResult<Account>
        {
            Result = account,
            Errors = await this.ProcessWithdrawal(account, amount),
        };
    }

    public async Task<ServiceResult<Account>> Deposit(int id, double amount)
    {
        var account = await this.accountRepository.Read(id);

        return new ServiceResult<Account>
        {
            Result = account,
            Errors = await this.ProcessDeposit(account, amount),
        };
    }

    public async Task<ServiceResult<Account>> Delete(int id)
    {
        return new ServiceResult<Account>
        {
            Result = await this.accountRepository.Delete(id)
        };
    }

    private async Task<List<string>> ProcessWithdrawal(Account? account, double amount)
    {
        var errors = new List<string>();
        
        if (account is null)
        {
            errors.Add("Invalid Account: Account number doest not exist.");
            return errors;
        }
        
        var updatedBalance = account.Balance - amount;
        
        TransactionValidator.ValidateBalance(updatedBalance, TransactionType.Withdrawal, errors);
        TransactionValidator.ValidateWithdrawalAmount(account.Balance, amount, errors);

        if (!errors.Any())
        {
            await this.accountRepository.UpdateBalance(account.Id, updatedBalance);
        }

        return errors;
    }
    
    private async Task<List<string>> ProcessDeposit(Account? account, double amount)
    {
        var errors = new List<string>();
        
        if (account is null)
        {
            errors.Add("Invalid Account: Account number doest not exist.");
            return errors;
        }
        
        TransactionValidator.ValidateDepositAmount(amount, errors);
        
        var updatedBalance = account.Balance + amount;

        if (!errors.Any())
        {
            await this.accountRepository.UpdateBalance(account.Id, updatedBalance);
        }

        return errors;
    }
}