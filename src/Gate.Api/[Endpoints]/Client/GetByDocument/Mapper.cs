using FastEndpoints;
using Gate.Api.Domain;

namespace GetByDocument;

public sealed class Mapper : Mapper<Request, Response, object>
{
    public Response ToResponse(Client c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Document = c.Document,
        Email = c.Email
    };
}