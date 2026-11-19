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

// Alternative initialization using API Key 
//AIAgent agent = new AzureOpenAIClient(
//new Uri(endpoint),
//new ApiKeyCredential(apikey)) .GetChatClient(model)
//AsAIAgent (instructions: "You are a friendly assistant. Keep your answers brief.");

// 3. Define the Agent's Anatomy
AIAgent supportAgent = chatClient.AsAIAgent(
    name: "NetworkSupport",
    instructions: "You are a Tier 1 IT Support Agent. Your answers must be concise, professional, and limited strictly to troubleshooting network and VPN connectivity. Keep your answers brief."
);

Console.WriteLine($"Agent '{supportAgent.Name}' is online.\n");

// 4. Execute the Agent
string userIssue = "I am getting a DNS resolution error when connecting to the corporate VPN from a coffee shop. Keep your answers brief.";
Console.WriteLine($"User: {userIssue}\n");

AgentResponse response = await supportAgent.RunAsync(userIssue);
Console.WriteLine($"Agent: {response.Text}");
