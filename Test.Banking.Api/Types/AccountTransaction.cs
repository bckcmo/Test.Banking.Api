namespace Test.Banking.Api.Types;

public class AccountTransaction
{
    public AccountTransaction(double amount)
    {
        this.Amount = amount;
    }
    
    public double Amount { get; set; }
}