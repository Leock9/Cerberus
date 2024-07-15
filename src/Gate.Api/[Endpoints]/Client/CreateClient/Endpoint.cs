using FastEndpoints;
using FluentValidation;
using Gate.Api.Domain;
using System.Net;

namespace CreateClient;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public ILogger<Endpoint> Log { get; set; } = null!;
    public ICreateClientUseCase? CreateClientUseCase { get; set; }

    public override void Configure()
    {
        AllowAnonymous();
        Post("/client");
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        try
        {
            await CreateClientUseCase?.ExecuteAsync(Map.ToRequest(r))!;
            await SendAsync(new Response(), cancellation: c);
        }
        catch (DomainException dx)
        {
            ThrowError(dx.Message);
        }
        catch(ValidationException vx)
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