using FluentValidation;

namespace Gate.Api.Domain;

public interface IDeleteClientUseCase
{
    Task ExecuteAsync(string document);
}

public class DeleteClientUseCase : IDeleteClientUseCase
{
    private readonly IClientGateway _clientGateway;

    public DeleteClientUseCase(IClientGateway clientGateway)
    {
        _clientGateway = clientGateway;
    }

    public async Task ExecuteAsync(string document)
    {
        var existingClient = await _clientGateway.GetByDocumentAsync(document);

        if (existingClient == null)
            throw new ValidationException("Client doesnt exist.");

        //Ideal seria criar uma nova fila e um novo consumer também
        await _clientGateway.Delete(existingClient);
    }
}
