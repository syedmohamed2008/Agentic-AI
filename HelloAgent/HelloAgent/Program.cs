//// See https://aka.ms/new-console-template for more information
using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;


var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

IChatClient chatClient = new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureCliCredential())
    .GetChatClient(deploymentName).AsIChatClient();

AIAgent agent = chatClient.AsAIAgent(
    name: "SupportAgent",
    instructions: "You are a helpful support agent. Answer the user's questions to the best of your ability."
    );

// Alternative initialization using API Key 
//AIAgent agent = new AzureOpenAIClient(
//new Uri(endpoint),
//new ApiKeyCredential(apikey)) .GetChatClient(model)
//AsAIAgent (instructions: "You are a friendly assistant. Keep your answers brief.");

Console.WriteLine($"Agent '{agent.Name}' is online.");

string userInput = "PM of India";

Console.WriteLine($"User: {userInput}");

AgentResponse response = await agent.RunAsync(userInput);

Console.WriteLine($"Agent: {response.Text}");
