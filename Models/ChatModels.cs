using System.Text.Json.Serialization;

namespace AchillesChat.Models
{
    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;

        // Optional: you can send short history from the client to keep tone consistent
        public List<ChatTurn>? History { get; set; }
    }

    public class ChatTurn
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Speaker Speaker { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class ChatResponse
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Speaker Speaker { get; set; }

        public string Text { get; set; } = string.Empty;

        // Debug/explainability for interviews
        public string Mode { get; set; } = "rule-based | similarity | fallback";
        public List<string>? MatchedSources { get; set; }
    }
}