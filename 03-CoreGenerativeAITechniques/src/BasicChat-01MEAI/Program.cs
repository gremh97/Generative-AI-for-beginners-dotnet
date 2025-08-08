using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text;

var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if(string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

// IChatClient: 자연어 프롬프트를 보내고 응답을 받기 위한 추상 인터페이스
IChatClient client = new ChatCompletionsClient(
    // endpoint: GitHub 모델 서버의 주소 (AI 모델이 실제로 작동하는 API 위치)
    endpoint: new Uri("https://models.github.ai/inference"),
    // 인증 토큰을 전달하는 객체 (API 사용 권한 증명)
    new AzureKeyCredential(githubToken))
    // AsIChatClient: 위 모델 클라이언트를 "Phi-3.5-MoE-instruct" 모델을 사용하는 IChatClient 인터페이스로 변환
    .AsIChatClient("Phi-3.5-MoE-instruct");


// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
var response = await client.GetResponseAsync(prompt.ToString());

// display the response
Console.WriteLine(response.Text);
