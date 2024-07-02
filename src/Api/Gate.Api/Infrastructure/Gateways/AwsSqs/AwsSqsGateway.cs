using Amazon.SQS;
using Amazon.SQS.Model;
using Gate.Api.Infrastructure.Gateways.AwsSqs.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Gate.Api.Infrastructure.Gateways.AwsSqs;

public class AwsSqsGateway
(
	ILogger<AwsSqsGateway> logger,
	IAmazonSQS amazonSQS,
	IAwsSqsConfiguration awsSqsConfiguration
) : IAwsSqsGateway
{
	public readonly ILogger<AwsSqsGateway> _logger = logger;
	public readonly IAmazonSQS _amazonSQS = amazonSQS;
	public readonly IAwsSqsConfiguration _awsSqsConfiguration = awsSqsConfiguration;

	public Task<SendMessageResponse> SendMessageAsync<T>(T message)
	{
		try
		{
			var queueUrl = _awsSqsConfiguration.QueueUrl ??
				throw new ArgumentNullException(nameof(_awsSqsConfiguration.QueueUrl));

			var request = new SendMessageRequest
				(
					queueUrl, JsonSerializer.Serialize(message)
				);

			return _amazonSQS.SendMessageAsync(request);
		}
		catch (Exception ex)
		{
            _logger.LogError(ex, "Error sending message to SQS");
			throw;
		}
	}
}
