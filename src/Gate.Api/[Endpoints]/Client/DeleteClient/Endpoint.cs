
using FastEndpoints;
using FluentValidation;
using Gate.Api.Domain;
using System.Net;
namespace DeleteClient;

public sealed class Endpoint : Endpoint<Request, Response>
{
    public ILogger<Endpoint> Log { get; set; } = null!;
    public IDeleteClientUseCase? DeleteClientUseCase { get; set; } = null!;

    public override void Configure()
    {
        AllowAnonymous();
        Delete("/client");
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        try
        {
            await DeleteClientUseCase?.ExecuteAsync(r.Document)!;
            await SendAsync(new Response(), cancellation: c);
        }
        catch (DomainException dx)
        {
            ThrowError(dx.Message);
        }
        catch (ValidationException vx)
        {
            ThrowError(vx.Message, (int)HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            Log.LogError("Ocorreu um erro inesperado ao executar o endpoint:{typeof(Endpoint).Namespace}. {ex.Message}", typeof(Endpoint).Namespace, ex.Message);
            ThrowError("Unexpected Error", (int)HttpStatusCode.BadRequest);
        }
    }
}