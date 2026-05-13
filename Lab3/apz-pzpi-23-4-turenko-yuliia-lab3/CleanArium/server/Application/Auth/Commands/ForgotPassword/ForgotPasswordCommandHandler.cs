using Application.Abstractions;
using Domain.Models;
using MediatR;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
{
    private readonly IUserRepository _users;
    private readonly IPasswordResetTokenRepository _tokens;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(IUserRepository users, 
        IPasswordResetTokenRepository tokens, 
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _users = users;
        _tokens = tokens;
        _logger = logger;
    }

    public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct);

        var token = new PasswordResetToken
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false
        };

        await _tokens.AddAsync(token, ct);

        _logger.LogInformation("Forgot password for user: {Id} ", user.Id);

        return token.Token;
    }
}
