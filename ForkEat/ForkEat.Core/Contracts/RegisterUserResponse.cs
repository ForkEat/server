using System;
using ForkEat.Core.Domain;

namespace ForkEat.Core.Contracts;

public class RegisterUserResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }

    public RegisterUserResponse()
    {
    }

    public RegisterUserResponse(User user)
    {
        Id = user.Id;
        Email = user.Email;
        UserName = user.UserName;
    }
}