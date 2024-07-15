using FluentValidation;
using Gate.Api.Infrastructure.Gateways.AwsSqs;

namespace Gate.Api.Domain;

public interface ICreateClientUseCase
{
    Task ExecuteAsync(CreateClientInput input);
}

public class CreateClientUseCase(IClientGateway clientGateway, IAwsSqsGateway awsSqsGateway) : ICreateClientUseCase
{
    private readonly IClientGateway _clientGateway = clientGateway;
    private readonly IAwsSqsGateway _awsSqsGateway = awsSqsGateway;

    public async Task ExecuteAsync(CreateClientInput input)
    {
        var client = new Client(input.Name, input.Document, input.Email);

        var existingClient = await _clientGateway.GetByDocumentAsync(client.Document);

        if (existingClient != null)
            throw new ValidationException("Client already exists");

        await _clientGateway.SaveAsync(client);
        await _awsSqsGateway.SendMessageAsync(client);
    }
}
