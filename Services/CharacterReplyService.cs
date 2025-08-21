using AchillesChat.Models;
using System.Text.RegularExpressions;

namespace AchillesChat.Services
{
    public class CharacterReplyService
    {
        private readonly KnowledgeBaseService _kb;
        private readonly MLSimilarityService _sim;

        public CharacterReplyService(KnowledgeBaseService kb, MLSimilarityService sim)
        {
            _kb = kb;
            _sim = sim;
        }

        public ChatResponse GetReply(ChatRequest request)
        {
            var user = request.Message.Trim();

            // 1) Simple rule-based intents (greetings, who are you, etc.)
            var rule = TryRuleBased(user);
            if (rule is not null) return rule;

            // 2) Knowledge similarity search using ML.NET
            var match = _sim.FindBestMatch(user);
            if (match is not null && match.Value.score >= 0.25f) // tweakable threshold
            {
                var item = match.Value.item;
                return new ChatResponse
                {
                    Speaker = item.Speaker,
                    Text = item.Answer,
                    Mode = $"ml-similarity (score={match.Value.score:F2})",
                    MatchedSources = new List<string> { item.Id }
                };
            }

            // 3) In-character fallback with strict boundary
            var speaker = PickSpeaker(user);
            var fb = _kb.GetFallback(speaker);

            if (IsOutOfScope(user))
            {
                var boundary = fb.Boundaries.Length > 0 ? fb.Boundaries[Random.Shared.Next(fb.Boundaries.Length)]
                                                        : "That’s beyond what I can speak of.";
                return new ChatResponse
                {
                    Speaker = speaker,
                    Text = boundary,
                    Mode = "fallback-boundary"
                };
            }

            var unsure = fb.UnsureReplies.Length > 0 ? fb.UnsureReplies[Random.Shared.Next(fb.UnsureReplies.Length)]
                                                     : "I’m not certain about that.";
            return new ChatResponse
            {
                Speaker = speaker,
                Text = unsure,
                Mode = "fallback-unsure"
            };
        }

        // ------------------ helpers ------------------

        private static bool IsGreeting(string s) =>
            Regex.IsMatch(s, @"\b(hi|hello|hey|greetings|yo)\b", RegexOptions.IgnoreCase);

        private static bool IsWhoAreYou(string s) =>
            Regex.IsMatch(s, @"who\s+are\s+you|your\s+name|introduce", RegexOptions.IgnoreCase);

        private static bool IsOutOfScope(string s)
        {
            // Anything obviously modern or after Trojan War -> out of scope
            string[] modern = { "internet", "iphone", "ai", "ml", "microsoft", "google",
                "covid", "america", "detroit", "instagram", "airplane", "rocket", "quantum", "python", "c#" };

            var lower = s.ToLowerInvariant();
            return modern.Any(m => lower.Contains(m));
        }

        private ChatResponse? TryRuleBased(string user)
        {
            if (IsGreeting(user))
            {
                var fb = _kb.GetFallback(Speaker.Both);
                var opener = fb.SmallTalkOpeners.Length > 0 ?
                             fb.SmallTalkOpeners[Random.Shared.Next(fb.SmallTalkOpeners.Length)]
                             : "Hello.";
                return new ChatResponse
                {
                    Speaker = Speaker.Both,
                    Text = opener,
                    Mode = "rule-based:greeting"
                };
            }

            if (IsWhoAreYou(user))
            {
                return new ChatResponse
                {
                    Speaker = Speaker.Both,
                    Text = "I am Achilles, son of Peleus, and I am Patroclus, his companion. Speak, and we’ll answer as we can.",
                    Mode = "rule-based:intro"
                };
            }

            return null;
        }

        private static Speaker PickSpeaker(string user)
        {
            // Simple heuristic: battle/valor -> Achilles; feelings/observations -> Patroclus; otherwise Both
            var lower = user.ToLowerInvariant();
            if (Regex.IsMatch(lower, @"battle|fight|hector|armor|spear|speed|war|agamemnon|briseis"))
                return Speaker.Achilles;
            if (Regex.IsMatch(lower, @"feel|love|friend|memory|childhood|song|healer|gentle|kind"))
                return Speaker.Patroclus;
            return Speaker.Both;
        }
    }
}