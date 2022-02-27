using Test.Banking.Api.Types;

namespace Test.Banking.Api.Utils;

public static class TransactionValidator
{
    private const double MinBalance = 100.00;
    
    private const double MaxDeposit = 10000.00;

    private const int MaxWithdrawalPercent = 90;
    
    public static void ValidateBalance(double balance, TransactionType transaction, IList<string> errors)
    {
        if (balance < MinBalance)
        {
            var errorPrefix = transaction == TransactionType.Open ? "Invalid Balance: Balance" : "Invalid Withdrawal: New balance";
            errors.Add($"{errorPrefix} cannot be below ${MinBalance}.");
        }
    }
    
    public static void ValidateWithdrawalAmount(double currentBalance, double amount, IList<string> errors)
    {
        if (amount / currentBalance > MaxWithdrawalPercent / 100.0)
        {
            errors.Add($"Invalid Withdrawal: Withdrawal cannot be greater than {MaxWithdrawalPercent}% of total balance.");
        }
    }
    
    public static void ValidateDepositAmount(double amount, IList<string> errors)
    {
        if (amount > MaxDeposit)
        {
            errors.Add($"Invalid Deposit: Deposit amount is great than ${MaxDeposit}.");
        }
        
        if (amount <= 0)
        {
            errors.Add($"Invalid Deposit: Deposit amount must be greater than $0.00.");
        }
    }
}