using FastEndpoints;
using FluentValidation;

namespace GetByDocument;

public sealed class Request
{
    public string Document { get; init; } = string.Empty;
}

public sealed class Validator : Validator<Request>
{
    public Validator()
    {

        RuleFor(x => x.Document)
                             .NotEmpty()
                             .NotNull();
    }
}

public sealed class Response
{
    public string Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Document { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
