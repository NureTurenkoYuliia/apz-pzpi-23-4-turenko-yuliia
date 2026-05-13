using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SupportMessages.Commands.Update;

public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand>
{
    private readonly ISupportMessageRepository _repo;
    private readonly ILogger<UpdateMessageCommandHandler> _logger;

    public UpdateMessageCommandHandler(ISupportMessageRepository repo, ILogger<UpdateMessageCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(UpdateMessageCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.MessageId);

        entity.Subject = request.Subject;
        entity.Message = request.Message;

        await _repo.UpdateAsync(entity);

        _logger.LogInformation("USER_ACTION SupportMessage updated: {Id}", entity.Id);
    }
}
