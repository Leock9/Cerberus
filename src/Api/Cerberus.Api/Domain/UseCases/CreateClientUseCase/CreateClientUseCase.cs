namespace Cerberus.Api.Domain;

public interface ICreateClientUseCase
{
    Task ExecuteAsync(CreateClientInput input);
}

public class CreateClientUseCase(IClientGateway clientGateway) : ICreateClientUseCase
{
    private readonly IClientGateway _clientGateway = clientGateway;

    public async Task ExecuteAsync(CreateClientInput input)
    {
        var client = new Client(input.Name, input.Document, input.Email);
        await _clientGateway.SaveAsync(client);
    }
}
