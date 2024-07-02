using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Gate.Api.Domain;

namespace Gate.Api.Infrastructure.Gateways.DynamoDb;

public class DynamoDbClientGateway : IClientGateway
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IDynamoDBContext _context;
    private const string TableName = "Client";

    public DynamoDbClientGateway(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb;
        _context = new DynamoDBContext(dynamoDb);
        EnsureTableExistsAsync().Wait();
    }

    private async Task EnsureTableExistsAsync()
    {
        var tables = await _dynamoDb.ListTablesAsync();

        if (!tables.TableNames.Contains(TableName))
        {
            var request = new CreateTableRequest
            {
                TableName = TableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new() {
                        AttributeName = "Id",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new() {
                        AttributeName = "Id",
                        KeyType = "HASH"
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            await _dynamoDb.CreateTableAsync(request);

            // Wait until the table is created
            await WaitUntilTableReadyAsync(TableName);
        }
    }

    private async Task WaitUntilTableReadyAsync(string tableName)
    {
        string status = null;
        do
        {
            await Task.Delay(5000); // wait 5 seconds before checking the status
            try
            {
                var response = await _dynamoDb.DescribeTableAsync(new DescribeTableRequest
                {
                    TableName = tableName
                });
                status = response.Table.TableStatus;
            }
            catch (ResourceNotFoundException)
            {
                // DescribeTable is eventually consistent. So you might get resource not found. 
                // So we handle the potential exception.
            }
        } while (status != "ACTIVE");
    }

    public async Task<Client?> GetByDocumentAsync(string document)
    {
        var conditions = new List<ScanCondition>
        {
            new("Document", ScanOperator.Equal, document)
        };

        var search = _context.ScanAsync<ClientDocument>(conditions);
        var results = await search.GetNextSetAsync();

        return results.Count == 0
            ? null
            : new Client
            (
                results.FirstOrDefault().Name,
                results.FirstOrDefault().Document,
                results.FirstOrDefault().Email,
                results.FirstOrDefault().Id
            );
    }

    public async Task SaveAsync(Client client)
    {
        var clientDocument = new ClientDocument
        {
           Id = client.Id.ToString(),
           Document = client.Document,
           Name = client.Name,
           Email = client.Email
        };

        await _context.SaveAsync(clientDocument);
    }
}