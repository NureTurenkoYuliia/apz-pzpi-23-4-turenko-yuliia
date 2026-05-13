using Application.Abstractions;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SupportMessages.Commands.Create;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, long>
{
    private readonly ISupportMessageRepository _repo;
    private readonly ILogger<CreateMessageCommandHandler> _logger;

    public CreateMessageCommandHandler(ISupportMessageRepository repo, ILogger<CreateMessageCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<long> Handle(CreateMessageCommand request, CancellationToken ct)
    {
        var entity = new SupportMessage
        {
            UserId = request.UserId,
            Sender = UserRole.User,
            Subject = request.Subject,
            Message = request.Message,
            MessageStatus = MessageStatus.Sent,
            CreatedAt = DateTime.UtcNow,
        };

        await _repo.AddAsync(entity);

        _logger.LogInformation("USER_ACTION SupportMessage created: {Id}", entity.Id);

        return entity.Id;
    }
}