using Microsoft.AspNetCore.Mvc;
using AchillesChat.Models;
using AchillesChat.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;

namespace AchillesChat.Controllers
{
    public class ChatController : Controller
    {
        private readonly CharacterReplyService _characterReply;
        private readonly AIChatService _aiService;
       // private readonly MLSimilarityService _sim;
        private readonly string _openAiKey;

        
        public ChatController(CharacterReplyService characterReply,AIChatService aiService)
        {
            _characterReply = characterReply;
            _aiService= aiService;
            
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost("api/chat")]
        public async Task<IActionResult> PostMessage([FromBody] ChatRequest request)
        {


            // 1. Try ML.NET knowledge base
            var reply = _characterReply.GetReply(request);

            // 2. OpenAi fallback
            if (reply.Mode.StartsWith("fallback"))
            {
                var persona = reply.Speaker.ToString();
                var aiReply = await _aiService.GenerateReplyAsync(request.Message, persona);

                return Ok(new
                {
                    Speaker = reply.Speaker,
                    Text = aiReply,
                    Mode = "openAi"
                });
            }
            return Ok(reply);
        }
           
     }
    
}