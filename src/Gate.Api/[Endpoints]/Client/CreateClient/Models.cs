using FastEndpoints;
using FluentValidation;

namespace CreateClient;

public class Request
{
    public string Name { get; init; } = string.Empty;

    public string Document { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;
}

public class Validator : Validator<Request>
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

public class Response
{
    public string Message => "OK";
}
