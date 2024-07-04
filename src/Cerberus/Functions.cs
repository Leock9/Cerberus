// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Cerberus;

public class Function
{
    private static readonly HttpClient client = new HttpClient();
    public string FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            Console.WriteLine("Hello from Lambda!");
            Console.WriteLine($"Record Body:");
            Console.WriteLine(record.Body);
            Console.WriteLine("Facts about Cats!");

            var response = client.GetStringAsync("https://catfact.ninja/fact").Result;

            Console.WriteLine(response);
        }
        return $"Processed {sqsEvent.Records.Count} records.";
    }
}