using System;

namespace ForkEat.Core.Exceptions;

public class PasswordValidationException : ArgumentException
{
    public PasswordValidationException(string? message) : base(message)
    {
    }
}