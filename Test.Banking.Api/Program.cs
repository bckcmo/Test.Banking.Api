using Microsoft.EntityFrameworkCore;
using Test.Banking.Api.Abstractions.Repositories;
using Test.Banking.Api.Abstractions.Services;
using Test.Banking.Api.Repositories;
using Test.Banking.Api.Repositories.DbContexts;
using Test.Banking.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddDbContext<UserDbContext>(options => options.UseInMemoryDatabase("Users"))
    .AddDbContext<AccountDbContext>(options => options.UseInMemoryDatabase("Accounts"));
builder.Services
    .AddScoped<IUserService, UserService>()
    .AddScoped<IAccountService, AccountService>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IAccountRepository, AccountRepository>();

var app = builder.Build();

app.UseRouting();

app.UseSwagger();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSwaggerUI();

app.Run();