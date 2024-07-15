namespace Gate.Api.Domain;

public interface IGetByDocumentUseCase
{
    Task<Client> ExecuteAsync(string document);
}

public class GetByDocumentUseCase(IClientGateway clientGateway) : IGetByDocumentUseCase
{
    private readonly IClientGateway _clientGateway = clientGateway;

    public async Task<Client?> ExecuteAsync(string document)
    {
        return await _clientGateway.GetByDocumentAsync(document);
    }
}
