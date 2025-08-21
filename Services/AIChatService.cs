
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using OpenAI.Chat;

public class AIChatService
{
    private readonly ChatClient _client;

    public AIChatService( IConfiguration config)
    {
        var apiKey = config["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("Missing OpenAI Key in config");

        _client = new ChatClient(model: "gpt-4o-mini", apiKey);
    }

    public async Task<string> GenerateReplyAsync(string userMessage, string persona)
    {
        var systemPrompt = $"You are {persona} from the novel 'The Song of Achilles'. Reply in-character, stay friendly, and only reveal what this character would reasonably know.";
        var chatResponse = await _client.CompleteChatAsync(new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userMessage)
        });

        return chatResponse.Value.Content[0].Text ?? "...";
    }
}