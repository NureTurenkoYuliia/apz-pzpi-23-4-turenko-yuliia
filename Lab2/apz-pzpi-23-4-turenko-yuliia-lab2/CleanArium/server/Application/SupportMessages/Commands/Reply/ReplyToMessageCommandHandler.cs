using Application.Abstractions;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SupportMessages.Commands.Reply;

public class ReplyToMessageCommandHandler : IRequestHandler<ReplyToMessageCommand, long>
{
    private readonly ISupportMessageRepository _repo;
    private readonly IUserRepository _userRepo;
    private readonly ILogger<ReplyToMessageCommandHandler> _logger;

    public ReplyToMessageCommandHandler(
        ISupportMessageRepository repo,
        IUserRepository userRepo,
        ILogger<ReplyToMessageCommandHandler> logger)
    {
        _repo = repo;
        _userRepo = userRepo;
        _logger = logger;
    }

    public async Task<long> Handle(ReplyToMessageCommand request, CancellationToken ct)
    {
        UserRole role = await _userRepo.GetUserRoleByIdAsync(request.UserId, ct);

        var entity = new SupportMessage
        {
            UserId = request.UserId,
            ReplyToMessageId = request.MessageId,
            Sender = role,
            Subject = request.Subject,
            Message = request.Message,
            MessageStatus = MessageStatus.Sent,
            CreatedAt = DateTime.UtcNow,
        };

        await _repo.AddAsync(entity);

        _logger.LogInformation("USER_ACTION SupportMessage created replying to message: {Id}", request.MessageId);

        return entity.Id;
    }
}