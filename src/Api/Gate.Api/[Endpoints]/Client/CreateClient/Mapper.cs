using Gate.Api.Domain;
using FastEndpoints;

namespace CreateClient;

public class Mapper : Mapper<Request, Response, object>
{
    public CreateClientInput ToRequest(Request r) => new
    (
        r.Name,
        r.Document,
        r.Email
    );
}