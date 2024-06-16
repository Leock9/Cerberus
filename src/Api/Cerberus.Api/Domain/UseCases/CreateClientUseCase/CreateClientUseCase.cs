namespace Cerberus.Api.Domain;

public interface ICreateClientUseCase
{
    Task ExecuteAsync(CreateClientInput input);
}

public class CreateClientUseCase : ICreateClientUseCase
{
    private readonly IClientGateway _clientGateway;

    public CreateClientUseCase(IClientGateway clientGateway)
    {
        _clientGateway = clientGateway;
    }

    public async Task ExecuteAsync(CreateClientInput input)
    {
        var client = new Client(input.Name, input.Document, input.Email);
        await _clientGateway.Create(client);
    }
}
