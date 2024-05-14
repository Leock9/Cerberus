using System;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Lambda;

public class Function
{
    private static readonly HttpClient client = new();
    public string FunctionHandler(HttpRequest req)
    {
        Console.WriteLine("Hello from Lambda!");
        Console.WriteLine($"Record Body:");
        Console.WriteLine(record.Body);
        Console.WriteLine("Facts about Cats!");

        var response = client.GetStringAsync("https://catfact.ninja/fact").Result;

        Console.WriteLine(response);
    }
}