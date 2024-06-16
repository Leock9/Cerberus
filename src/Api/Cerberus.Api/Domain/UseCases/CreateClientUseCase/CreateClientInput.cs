namespace Cerberus.Api.Domain;

public record CreateClientInput
{
    public string Name { get; init; } = string.Empty;
    public string Document { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
