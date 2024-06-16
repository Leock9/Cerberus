using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda;
public class Function
{
    private static readonly AmazonCognitoIdentityProviderClient _cognitoClient = new AmazonCognitoIdentityProviderClient();
    private static readonly string _userPoolId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_ID");

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            var cpf = record.Body;

            context.Logger.LogLine($"Processing CPF: {cpf}");

            try
            {
                var request = new AdminCreateUserRequest
                {
                    UserPoolId = _userPoolId,
                    Username = cpf,
                    UserAttributes = new List<AttributeType>
                        {
                            new AttributeType
                            {
                                Name = "custom:cpf",
                                Value = cpf
                            }
                        }
                };

                var response = await _cognitoClient.AdminCreateUserAsync(request);

                context.Logger.LogLine($"User {cpf} created successfully.");
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error creating user: {ex.Message}");
            }
        }
    }
}
