namespace Gate.Api.Infrastructure.Gateways.DynamoDb.Configuration;

public interface IDynamoDbClientConfiguration
{
    string Region { get; }
    string ServiceUrl { get; }
}

public record DynamoDbClientConfiguration : IDynamoDbClientConfiguration
{
    public string Region { get; init; } = null!;
    public string ServiceUrl { get; init; } = null!;
}

