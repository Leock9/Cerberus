
using Cerberus.Api.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "hh:mm:ss ";
});

builder.Services.AddFastEndpoints();
builder.Services.AddHealthChecks();

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "swagger";
        s.Title = "Cerberus API";
        s.Version = "v1";
        s.Description = "Documentation about endpoints";
    };

    o.EnableJWTBearerAuth = false;
    o.ShortSchemaNames = false;
    o.RemoveEmptyRequestSchema = true;
});

builder.Services.AddHttpClient();

// ** USECASES **
builder.Services.AddScoped<ICreateClientUseCase, CreateClientUseCase>();
builder.Services.AddScoped<IGetByDocumentUseCase, GetByDocumentUseCase>();

// ** GATEWAYS **


var app = builder.Build();
app.MapHealthChecks("/health");

app.UseFastEndpoints(c =>
{
    c.Endpoints.ShortNames = false;

    c.Endpoints.Configurator = ep =>
    {
        ep.Summary(s =>
        {
            s.Response<ErrorResponse>(400);
            s.Response(401);
            s.Response(403);
            s.Responses[200] = "OK";
        });

        ep.PostProcessors(FastEndpoints.Order.After, new GlobalLoggerPostProcces
        (
            LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<GlobalLoggerPostProcces>()
        ));
    };
}).UseSwaggerGen();

app.Run();