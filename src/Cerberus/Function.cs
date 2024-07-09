using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Cerberus
{
    public class Function
    {
        private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
        private readonly string _userPoolId = Environment.GetEnvironmentVariable("UserPoolId")!;
        private readonly string _userPoolClientId = Environment.GetEnvironmentVariable("UserPoolClientId")!;

        public Function()
        {
            if (_userPoolId == null || _userPoolClientId == null)
                throw new ArgumentNullException("UserPoolId and UserPoolClientId must be set.");

            _cognitoClient = new AmazonCognitoIdentityProviderClient();
        }

        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            foreach (var record in sqsEvent.Records)
            {
                try
                {
                    using var jsonDoc = JsonDocument.Parse(record.Body);
                    var formattedJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });

                    var options = new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = null
                    };
                    var message = JsonSerializer.Deserialize<UserMessage>(record.Body);

                    // Verifica se o usuário já existe no Cognito
                    var listUsersRequest = new ListUsersRequest
                    {
                        UserPoolId = _userPoolId,
                        Filter = $"cpf = \"{message!.Document}\""
                    };

                    var listUsersResponse = await _cognitoClient.ListUsersAsync(listUsersRequest);

                    if (listUsersResponse.Users.Count == 0)
                    {
                        var createUserRequest = new AdminCreateUserRequest
                        {
                            UserPoolId = _userPoolId,
                            Username = message.Document,
                            UserAttributes = new List<AttributeType>
                            {
                                new AttributeType { Name = "cpf", Value = message.Document }
                            },
                            TemporaryPassword = "Teste@123!"
                        };

                        var createUserResponse = await _cognitoClient.AdminCreateUserAsync(createUserRequest);
                        context.Logger.LogLine($"Usuário criado: {createUserResponse.User.Username}");

                        var adminSetUserPasswordRequest = new AdminSetUserPasswordRequest
                        {
                            UserPoolId = _userPoolId,
                            Username = message.Document,
                            Password = "Teste@123!",
                            Permanent = true
                        };
                        
                        await _cognitoClient.AdminSetUserPasswordAsync(adminSetUserPasswordRequest);
                        context.Logger.LogLine($"Senha permanente definida para o usuário: {message.Email}");
                    }
                    else
                    {
                        context.Logger.LogLine($"Usuário com CPF {message.Document} já existe.");
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"Erro ao processar o registro {record.MessageId}: {ex.Message}");
                    throw;
                }
            }
        }

        public record UserMessage
        {
            public string Name { get; init; } = string.Empty;
            public string Document { get; init; } = string.Empty;
            public string Email { get; init; } = string.Empty;
        }
    }
}
