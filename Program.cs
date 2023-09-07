using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Serialization;
using Azure;
using Azure.AI.Language.Conversations;

var predictionEndpoint = "https://testtextana20230907.cognitiveservices.azure.com";
var predictionKey = "78c89e0b552d4f0bb7a77b9bd7d4738b";
var userText = "我要一份大亨堡";
var projectName = "testluis";
var deploymentName = "M2";

// Create a client for the Language service model
Uri endpoint = new Uri(predictionEndpoint);
AzureKeyCredential credential = new AzureKeyCredential(predictionKey);
ConversationAnalysisClient client = new ConversationAnalysisClient(endpoint, credential);

// Call the Language service model to get intent and entities

var data = new
{
    analysisInput = new
    {
        conversationItem = new
        {
            text = userText,
            id = "1",
            participantId = "1",
        }
    },
    parameters = new
    {
        projectName,
        deploymentName,
        // Use Utf16CodeUnit for strings in .NET.
        stringIndexType = "Utf16CodeUnit",
    },
    kind = "Conversation",
};


// Send request
Response response = await client.AnalyzeConversationAsync(RequestContent.Create(data));
dynamic conversationalTaskResult = response.Content.ToDynamicFromJson(JsonPropertyNames.CamelCase);
dynamic conversationPrediction = conversationalTaskResult.Result.Prediction;
var options = new JsonSerializerOptions { WriteIndented = true };
Console.WriteLine(JsonSerializer.Serialize(conversationalTaskResult, options));
Console.WriteLine("--------------------\n");
Console.WriteLine(userText);
var topIntent = "";
if (conversationPrediction.Intents[0].ConfidenceScore > 0.5)
{
    topIntent = conversationPrediction.TopIntent;
    Console.WriteLine($"\n topIntent : {topIntent}");

    foreach (var item in conversationPrediction.entities)
    {
        Console.WriteLine($"\n entity {item}");
    }
}