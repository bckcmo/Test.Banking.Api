using Test.Banking.Api.Abstractions.Repositories;
using Test.Banking.Api.Exceptions;
using Test.Banking.Api.Repositories.DbContexts;
using Test.Banking.Api.Types;

namespace Test.Banking.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext context;
    
    public UserRepository(UserDbContext context)
    {
        this.context = context;
    }

    public async Task<User> Create(User user)
    {
        try
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
        catch (ArgumentException)
        {
            throw new RepositoryException($"User with id {user.Id} already exists.");
        }

        return user;
    }

    public async Task<User?> Read(int id)
    {
        return await context.Users.FindAsync(id);
    }
}