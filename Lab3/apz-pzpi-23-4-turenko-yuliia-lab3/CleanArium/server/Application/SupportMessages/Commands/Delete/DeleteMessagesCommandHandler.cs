using Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.SupportMessages.Commands.Delete;

public class DeleteMessagesCommandHandler : IRequestHandler<DeleteMessagesCommand>
{
    private readonly ISupportMessageRepository _repo;
    private readonly ILogger<DeleteMessagesCommandHandler> _logger;

    public DeleteMessagesCommandHandler(ISupportMessageRepository repo, ILogger<DeleteMessagesCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task Handle(DeleteMessagesCommand request, CancellationToken cancellationToken)
    {
        var messages = await _repo.GetHistoryAsync(request.FirstMessageId);

        foreach (var msg in messages.OrderByDescending(x => x.CreatedAt))
        {
            await _repo.DeleteAsync(msg);
        }

        _logger.LogInformation("Successfully deleted history of messages with first: {Id} ", request.FirstMessageId);
    }
}