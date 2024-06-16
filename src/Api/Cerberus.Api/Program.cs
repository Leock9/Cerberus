
using Amazon.DynamoDBv2;
using Amazon.SQS;
using Cerberus.Api._Endpoints_;
using Cerberus.Api.Domain;
using Cerberus.Api.Infrastructure.Gateways.DynamoDb;
using FastEndpoints;
using FastEndpoints.Swagger;
using LocalStack.Client.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
builder.Services.AddSingleton<IClientGateway, DynamoDbClientGateway>();

// ** AWS **
builder.Services.AddLocalStack(builder.Configuration);
builder.Services.AddAWSServiceLocalStack<IAmazonSQS>();
builder.Services.AddAWSServiceLocalStack<IAmazonDynamoDB>();

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