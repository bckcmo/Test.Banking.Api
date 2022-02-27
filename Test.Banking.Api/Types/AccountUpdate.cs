namespace Test.Banking.Api.Types;

public class AccountUpdate
{
    public AccountUpdate(double amount)
    {
        this.Amount = amount;
    }
    
    public double Amount { get; set; }
}