namespace Gate.Api.Domain;

public record Client(string Name, string Document, string Email, string Id = null)
{
    public string Id { get; init; } = Id ?? Guid.NewGuid().ToString();

    public string Name { get; init; } = string.IsNullOrEmpty(Name) ? 
                                        throw new DomainException("Name is required") : Name;

    public string Document { get; init; } = DocumentValidator.CpfValidation.Validate(Document) ? 
                                            Document : throw new DomainException("Document is invalid");

    public string Email { get; init; } = string.IsNullOrEmpty(Email) || !Email.Contains('@') ?
                                        throw new DomainException("Email is invalid") : Email;
}