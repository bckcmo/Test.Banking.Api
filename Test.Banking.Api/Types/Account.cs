namespace Test.Banking.Api.Types;

public class Account
{
    public Account(long id, int userId, double balance)
    {
        Id = id;
        UserId = userId;
        Balance = balance;
    }
    
    public long Id { get; set; }
    
    public int UserId { get; set; }
    
    public double Balance { get; set; }
}