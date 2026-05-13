using Application.Abstractions;
using FluentValidation;

namespace Application.SupportMessages.Commands.Update;

public class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommand>
{
    private readonly ISupportMessageRepository _repo;

    public UpdateMessageCommandValidator(ISupportMessageRepository repo)
    {
        _repo = repo;

        RuleFor(x => x.MessageId).NotEmpty()
             .WithMessage("Message id is required to update.");

        RuleFor(x => x.Subject).MinimumLength(8).MaximumLength(30);

        RuleFor(x => x.Message).MinimumLength(10).MaximumLength(120);

        RuleFor(x => x)
            .MustAsync(MessageExists).WithMessage("Message not found.")
            .MustAsync(MessageBelongsToUser).WithMessage("You can't update this message.");
    }

    private async Task<bool> MessageExists(UpdateMessageCommand cmd, CancellationToken ct)
    {
        return await _repo.ExistsAsync(cmd.MessageId, ct);
    }

    private async Task<bool> MessageBelongsToUser(UpdateMessageCommand cmd, CancellationToken ct)
    {
        return await _repo.MessageBelongsToUserAsync(cmd.MessageId, cmd.UserId);
    }
}
