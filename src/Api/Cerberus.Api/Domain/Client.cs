namespace Cerberus.Api.Domain;

public record Client(string Name, string Document, string Email)
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; init; } = string.IsNullOrEmpty(Name) ? 
                                        throw new DomainException("Name is required") : Name;

    public string Document { get; init; } = DocumentValidator.CpfValidation.Validate(Document) ? 
                                            Document : throw new DomainException("Document is invalid");

    public string Email { get; init; } = string.IsNullOrEmpty(Email) || !Email.Contains('@') ?
                                        throw new DomainException("Email is invalid") : Email;
}
