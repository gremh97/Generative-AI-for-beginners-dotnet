using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;
using Microsoft.Extensions.Configuration;
using System.Text;

// This example shows how to use the OpenAI chat completion service with GitHub's API.
// It requires a GitHub token with access to the OpenAI API.
// You can set the token in your environment variables or in user secrets.
// To set the token in user secrets, run the following command in your project directory:
// dotnet user-secrets set GITHUB_TOKEN
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}
var modelId = "Phi-3.5-mini-instruct";
var uri = "https://models.github.ai/inference";


// create client
var client = new OpenAIClient(new ApiKeyCredential(githubToken), 
    new OpenAIClientOptions { Endpoint = new Uri(uri) });

// Create a chat completion service
// semantic kernel constructs a chat completion service using the OpenAI client
var builder = Kernel.CreateBuilder();
// Register OpenAI client to the semantic kernel
builder.AddOpenAIChatCompletion(modelId, client);

// Get the chat completion service
Kernel kernel = builder.Build();
// 채팅 전용 컴포넌트 가져오기 (IChatCompletionService는 채팅 API 인터페이스)
var chat = kernel.GetRequiredService<IChatCompletionService>();

// Create a chat history instance to keep track of the conversation
var history = new ChatHistory();
// Add system message to the chat history
// This message sets the context for the chat, instructing the AI on how to respond
// You can customize this message to change the AI's behavior
// In this case, the AI is instructed to be a useful chatbot that replies in a funny way and uses emojis if possible
history.AddSystemMessage("You are a useful chatbot. If you don't know an answer, say 'I don't know!'. Always reply in a funny way. Use emojis if possible.");

// Start the chat loop
// This loop will continue until the user enters an empty question
while (true)
{
    Console.Write("Q: ");
    var userQ = Console.ReadLine();         // Read user input from the console
    if (string.IsNullOrEmpty(userQ))
    {
        break;      // Exit the loop if the user input is empty (just pressing Enter)
    }
    history.AddUserMessage(userQ);

    var sb = new StringBuilder();
    // LLMs are stateless(상태를 기억하지 못하는 상태), so we must send the full chat history with each request
    // to help the model maintain context and generate coherent responses.
    var result = chat.GetStreamingChatMessageContentsAsync(history); 
    // 'result' streams the AI's newly generated response (Answer for current question)
    // based on the full 'history' sent to the model.
    Console.Write("AI: ");
    // Receives the current response in multiple streaming chunks,
    // outputting each chunk until the full reply is complete.
    // This allows for a more interactive experience, as the AI's response is displayed in real-time.
    await foreach (var item in result)
    {
        sb.Append(item);
        Console.Write(item.Content);
    }
    Console.WriteLine();

    history.AddAssistantMessage(sb.ToString());
}