namespace Cerberus.Api.Domain;

public interface IClientGateway
{
    void Create(Client client);

    Task<Client> GetByDocumentAsync(string document);
}
