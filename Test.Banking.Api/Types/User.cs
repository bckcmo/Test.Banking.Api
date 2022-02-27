namespace Test.Banking.Api.Types;

public class User
{
    public User(int id, string userName)
    {
        Id = id;
        UserName = userName;
    }

    public int Id { get; set; }
    
    public string UserName { get; set; }
}