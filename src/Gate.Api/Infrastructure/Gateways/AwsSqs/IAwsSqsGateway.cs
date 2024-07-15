using Amazon.SQS.Model;

namespace Gate.Api.Infrastructure.Gateways.AwsSqs;

public interface IAwsSqsGateway
{
    Task<SendMessageResponse> SendMessageAsync<T>(T message);
}
