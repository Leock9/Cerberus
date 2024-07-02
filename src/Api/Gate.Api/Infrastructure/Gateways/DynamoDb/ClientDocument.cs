using Amazon.DynamoDBv2.DataModel;

namespace Gate.Api.Infrastructure.Gateways.DynamoDb;

[DynamoDBTable("Client")]
public class ClientDocument
{
    [DynamoDBHashKey]
    public string Id { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Document { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Name { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Email { get; set; } = string.Empty;
}
