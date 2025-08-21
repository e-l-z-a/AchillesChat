using System.Text.Json.Serialization;

namespace AchillesChat.Models
{
    public class QAItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;

        // Achilles, Patroclus, or Both
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Speaker Speaker { get; set; } = Speaker.Both;

        // Optional keywords to improve matching
        public string[]? Keywords { get; set; }
    }

    public class PersonaFallback
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Speaker Speaker { get; set; } = Speaker.Both;
        public string[] SmallTalkOpeners { get; set; } = Array.Empty<string>();
        public string[] UnsureReplies { get; set; } = Array.Empty<string>();
        public string[] Boundaries { get; set; } = Array.Empty<string>();
    }

    public class KnowledgeBase
    {
        public List<QAItem> QAPairs { get; set; } = new();
        public List<PersonaFallback> Fallbacks { get; set; } = new();
    }
}