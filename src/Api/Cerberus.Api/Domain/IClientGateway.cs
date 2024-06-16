namespace Cerberus.Api.Domain;

public interface IClientGateway
{
    Task SaveAsync(Client client);

    Task<Client?> GetByDocumentAsync(string document);
}
