using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ForkEat.Core.Contracts;
using ForkEat.Core.Domain;
using ForkEat.Core.Exceptions;
using ForkEat.Core.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace ForkEat.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository repository;

        private readonly IPasswordValidator passwordValidator;

        public AuthenticationService(IUserRepository repository, IPasswordValidator passwordValidator)
        {
            this.repository = repository;
            this.passwordValidator = passwordValidator;
        }

        public async Task<LoginUserResponse> Login(LoginUserRequest request)
        {
            User user = await repository.FindUserByEmail(request.Email);

            EnsureUserFoundAndPasswordMatch(request, user);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GetJwtSecretFromEnv();

            SecurityTokenDescriptor tokenDescriptor = BuildTokenDescriptor(user, key);
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            var response = new LoginUserResponse()
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = tokenHandler.WriteToken(token)
            };

            return response;
        }

        private static SecurityTokenDescriptor BuildTokenDescriptor(User user, byte[] key)
        {
            return new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
        }

        private static byte[] GetJwtSecretFromEnv()
        {
            return Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ??
                                           throw new ArgumentException("JWT_SECRET Env variable is not set"));
        }

        private static void EnsureUserFoundAndPasswordMatch(LoginUserRequest request, User user)
        {
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new InvalidCredentialsException();
            }
        }

        public async Task<RegisterUserResponse> Register(RegisterUserRequest request)
        {
            await EnsureUserDoesntExists(request);
            
            var user = new User()
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password
            };

            EnsurePasswordIsValid(user);

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            
            user = await repository.InsertUser(user);
            
            return new RegisterUserResponse(user);
        }

        private void EnsurePasswordIsValid(User user)
        {
            var validationResult = passwordValidator.Validate(user);

            if (!validationResult.IsValid)
            {
                throw new PasswordValidationException("Password does not match the specified requirements");
            }
        }

        private async Task EnsureUserDoesntExists(RegisterUserRequest request)
        {
            bool userExists = await this.repository.UserExistsByEmail(request.Email);
            if (userExists)
            {
                throw new ArgumentException($"A user with email \"{request.Email}\" already exists");
            }
        }
    }
}