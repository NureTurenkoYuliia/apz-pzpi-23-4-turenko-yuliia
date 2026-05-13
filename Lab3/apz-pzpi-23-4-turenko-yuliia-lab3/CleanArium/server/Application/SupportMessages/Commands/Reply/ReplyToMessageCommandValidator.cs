using Application.Abstractions;
using FluentValidation;

namespace Application.SupportMessages.Commands.Reply;

public class ReplyToMessageCommandValidator : AbstractValidator<ReplyToMessageCommand>
{
    private readonly ISupportMessageRepository _repo;

    public ReplyToMessageCommandValidator(ISupportMessageRepository repo)
    {
        _repo = repo;

        RuleFor(x => x.MessageId).NotEmpty()
             .WithMessage("Message id is required to reply.");

        RuleFor(x => x.Subject).NotEmpty().MinimumLength(4).MaximumLength(30)
             .WithMessage("Subject is required to create message.");

        RuleFor(x => x.Message).NotEmpty().MinimumLength(10).MaximumLength(120)
             .WithMessage("Message content is required to create message.");

        RuleFor(x => x)
            .MustAsync(MessageExists).WithMessage("Message not found.");
    }

    private async Task<bool> MessageExists(ReplyToMessageCommand cmd, CancellationToken ct)
    {
        return await _repo.ExistsAsync(cmd.MessageId, ct);
    }
}