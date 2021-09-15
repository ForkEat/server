namespace ForkEat.Core.Contracts
{
    public class LoginUserResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}