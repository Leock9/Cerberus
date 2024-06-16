namespace Cerberus.Api.Domain;

public interface IGetByDocumentUseCase
{
    Task<Client> ExecuteAsync(string document);
}

public class GetByDocumentUseCase : IGetByDocumentUseCase
{
    private readonly IClientGateway _clientGateway;

    public GetByDocumentUseCase(IClientGateway clientGateway)
    {
        _clientGateway = clientGateway;
    }

    public async Task<Client> ExecuteAsync(string document)
    {
        return await _clientGateway.GetByDocumentAsync(document);
    }
}
