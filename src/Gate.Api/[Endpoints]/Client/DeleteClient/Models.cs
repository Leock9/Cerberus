using FastEndpoints;
using FluentValidation;

namespace DeleteClient
{
    public sealed class Request
    {
        public string Name { get; init; } = string.Empty;

        public string Document { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Address { get; init; } = string.Empty;

        internal sealed class Validator : Validator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                                .NotEmpty()
                                .NotNull();

                RuleFor(x => x.Document)
                                    .NotEmpty()
                                    .NotNull();

                RuleFor(x => x.Email)
                                    .NotEmpty()
                                    .NotNull()
                                    .EmailAddress();

                RuleFor(x => x.Address)
                                    .NotEmpty()
                                    .NotNull();
            }
        }
    }

    public sealed class Response
    {
        public string Message => "Your username will be deleted from our database in 5 minutes.";
    }
}
