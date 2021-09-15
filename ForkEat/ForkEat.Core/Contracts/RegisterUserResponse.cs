using System;

namespace ForkEat.Core.Contracts
{
    public class RegisterUserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}