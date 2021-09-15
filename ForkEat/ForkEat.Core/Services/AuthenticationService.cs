﻿using System;
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

        public AuthenticationService(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<LoginUserResponse> Login(LoginUserRequest request)
        {
            var user = await repository.FindUserByEmail(request.Email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new InvalidCredentialsException();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new ArgumentException("JWT_SECRET Env variable is not set"));
            
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            var response = new LoginUserResponse()
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = tokenString
            };

            return response;
        }

        public Task<User> Register(RegisterUserRequest request)
        {
            var user = new User()
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password
            };
            
            return repository.InsertUser(user);
        }
    }
}