using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordResetTokenRepository _tokenRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IUserRepository userRepo,
        IPasswordResetTokenRepository tokenRepo,
        IPasswordHasher passwordHasher,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
        _hasher = passwordHasher;
        _logger = logger;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var token = await _tokenRepo.GetByTokenAsync(request.Token, ct);
        var user = await _userRepo.GetByIdAsync(token.UserId, ct);
        var salt = _hasher.GenerateSalt();

        user.Salt = salt;
        user.PasswordHash = _hasher.HashPassword(request.NewPassword, salt);

        await _userRepo.UpdateAsync(user, ct);

        token.IsUsed = true;
        await _tokenRepo.UpdateAsync(token, ct);

        _logger.LogInformation("Successfully reset password for user: {Id} ", user.Id);
    }
}
