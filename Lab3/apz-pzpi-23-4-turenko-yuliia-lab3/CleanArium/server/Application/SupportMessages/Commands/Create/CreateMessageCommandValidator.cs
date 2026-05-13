using FluentValidation;

namespace Application.SupportMessages.Commands.Create;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageCommandValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MinimumLength(4).MaximumLength(30)
             .WithMessage("Subject is required to create message.");

        RuleFor(x => x.Message).NotEmpty().MinimumLength(10).MaximumLength(120)
             .WithMessage("Message content is required to create message.");
    }
}