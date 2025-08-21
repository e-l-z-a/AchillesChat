using System.Text.Json;
using AchillesChat.Models;

namespace AchillesChat.Services
{
    public class KnowledgeBaseService
    {
        private readonly KnowledgeBase _kb;

        public KnowledgeBaseService(IWebHostEnvironment env)
        {
            var path = Path.Combine(env.ContentRootPath, "Data", "knowledge_base.json");
            _kb = JsonSerializer.Deserialize<KnowledgeBase>(File.ReadAllText(path),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new KnowledgeBase();
        }

        public IReadOnlyList<QAItem> QAPairs => _kb.QAPairs;
        public IReadOnlyList<PersonaFallback> Fallbacks => _kb.Fallbacks;

        public PersonaFallback GetFallback(Speaker speaker) =>
            _kb.Fallbacks.FirstOrDefault(f => f.Speaker == speaker)
            ?? _kb.Fallbacks.First(f => f.Speaker == Speaker.Both);
    }
}