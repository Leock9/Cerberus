using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SQS;
using Amazon.SQS.Model;
using FastEndpoints;
using FastEndpoints.Swagger;
using Gate.Api._Endpoints_;
using Gate.Api.Domain;
using Gate.Api.Infrastructure.Gateways.AwsSqs;
using Gate.Api.Infrastructure.Gateways.AwsSqs.Configuration;
using Gate.Api.Infrastructure.Gateways.DynamoDb;
using Gate.Api.Infrastructure.Gateways.DynamoDb.Configuration;
using LocalStack.Client.Extensions;

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

// ** CORS **
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// ** USECASES **
builder.Services.AddScoped<ICreateClientUseCase, CreateClientUseCase>();
builder.Services.AddScoped<IGetByDocumentUseCase, GetByDocumentUseCase>();

// ** GATEWAYS **
builder.Services.AddSingleton<IClientGateway, DynamoDbClientGateway>();
builder.Services.AddSingleton<IAwsSqsGateway, AwsSqsGateway>();

// ** AWS **
builder.Services.AddLocalStack(builder.Configuration);
builder.Services.AddAWSServiceLocalStack<IAmazonSQS>();
builder.Services.AddAWSServiceLocalStack<IAmazonDynamoDB>();

// ** CONFIGURATIONS **
builder.Services.AddSingleton<IAwsSqsConfiguration>(_ => builder.Configuration.GetSection("AwsSqsConfiguration").Get<AwsSqsConfiguration>());
builder.Services.AddSingleton<IDynamoDbClientConfiguration>(_ => builder.Configuration.GetSection("DynamoDbClientConfiguration").Get<DynamoDbClientConfiguration>());

var dynamoConfig = builder.Configuration.GetSection("DynamoDbClientConfiguration").Get<DynamoDbClientConfiguration>() ??
    throw new Exception("DynamoDbClientConfiguration not found");

builder.Services.AddSingleton(dynamoConfig);

builder.Services.AddSingleton<IDynamoDBContext>
(
    s => new DynamoDBContext
    (
        s.GetRequiredService<IAmazonDynamoDB>()
    )
);

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

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

        ep.PostProcessors(Order.After, new GlobalLoggerPostProcces
        (
            LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<GlobalLoggerPostProcces>()
        ));
    };
}).UseSwaggerGen();

if (builder.Configuration.GetSection("LocalStack").GetValue<bool>("UseLocalStack"))
{
    var sqsClient = app.Services.GetRequiredService<IAmazonSQS>();
    var createQueueRequest = new CreateQueueRequest
    {
        QueueName = "client"
    };

    var createQueueResponse = await sqsClient.CreateQueueAsync(createQueueRequest);
    Console.WriteLine($"Queue created with URL: {createQueueResponse.QueueUrl}");
}


app.Run();
