using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Lambda;

public class Function
{
    private readonly AmazonCognitoIdentityProviderClient _cognitoClient;

    public Function()
    {
        _cognitoClient = new AmazonCognitoIdentityProviderClient(new AmazonCognitoIdentityProviderConfig
        {
            RegionEndpoint = RegionEndpoint.USEast1
        });
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Starting function execution.");

        var requestBody = request.Body;
        var requestBodyJson = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
        var username = requestBodyJson["username"];
        var password = requestBodyJson["password"];

        var authRequest = new AdminInitiateAuthRequest
        {
            UserPoolId = "your-user-pool-id",
            ClientId = "your-client-id",
            AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH,
            AuthParameters = new Dictionary<string, string>
            {
                { "USERNAME", username },
                { "PASSWORD", password }
            }
        };
        var authResponse = await _cognitoClient.AdminInitiateAuthAsync(authRequest);

        var responseBody = new Dictionary<string, string>
        {
            { "token", authResponse.AuthenticationResult.IdToken }
        };
        
        var responseBodyJson = System.Text.Json.JsonSerializer.Serialize(responseBody);
        var response = new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = responseBodyJson,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            }
        };

        context.Logger.LogInformation("Function execution complete.");
        return response;
    }
}