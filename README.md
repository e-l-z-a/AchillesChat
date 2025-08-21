AchillesChat

AchillesChat is a persona-based chatbot built with ASP.NET Core and ML.NET.
It simulates friendly conversations with Achilles and Patroclus from The Song of Achilles.

The chatbot uses a custom knowledge base, ML.NET similarity scoring, and fallback small talk. Optionally, it can integrate with OpenAI GPT for out-of-scope questions.


---

🚀 Features

Knowledge Base Q&A

Define questions, answers, speaker (Achilles, Patroclus, Both), and optional keywords.


ML.NET Similarity

Uses FeaturizeText() to vectorize Q&A and user queries.

Computes cosine similarity to find the best matching answer.


Fallback System

Responds with small talk or unsure replies if no high-confidence match is found.


Persona Simulation

Each answer is tagged with the speaker.

Keeps conversations in-character.


Optional GPT Integration

Extendable to handle queries outside the knowledge base.




---

🏗️ Project Structure

AchillesChat/
│
├── Models/
│   ├── KnowledgeModel.cs           # Knowledge base Q&A model
│   ├── PersonaEnum.cs  # speaker
│   └── ChatModel.cs    # Container for all Q&A and fallback entries
│
├── Services/
│   ├── KnowledgeBaseService.cs  # Loads Q&A data from JSON
│   ├── MLSimilarityService.cs   # ML.NET TF-IDF vectorization + cosine similarity
│   ├── CharacterReplyService.cs           # Combines similarity + fallback logic
│   └── AIChatService.cs (optional) # GPT fallback
│
├── Controllers/
│   └── ChatController.cs        # API endpoints for chat requests
│
├── view/
│   └── chat                  # Frontend chat UI
│
└── Program.cs       # ASP.NET Core setup


---

⚡ How It Works

1. User asks a question → ChatController receives it.


2. ML Similarity → MLSimilarityService computes similarity score with all Q&A entries.


3. Threshold Check → If top score > 0.55 → returns answer from Q&A.


4. Fallback → If no confident match:

Returns a small talk reply or an unsure response.

Optionally, queries OpenAI GPT for dynamic in-character reply.





---

🔑 Knowledge Base Example

new QAItem
{
    Question = "Who are you?",
    Answer = "I am Achilles, son of Peleus and Thetis.",
    Speaker = Speaker.Achilles,
    Keywords = new[] { "identity", "hero", "name" }
},
new QAItem
{
    Question = "What is your bond with Achilles?",
    Answer = "Achilles and I share a bond deeper than comradeship.",
    Speaker = Speaker.Patroclus,
    Keywords = new[] { "friendship", "relationship", "bond" }
}


---

🖥️ Running Locally

Prerequisites

.NET 7 SDK or later

(Optional) OpenAI API Key if GPT fallback enabled


Steps

# Clone repo
git clone https://github.com/e-l-z-a/AchillesChat.git
cd AchillesChat

# Restore dependencies
dotnet restore

# Run project
dotnet run

Open http://localhost:5000 in your browser to chat with Achilles/Patroclus.


---

🔧 Configuration

Similarity Threshold

Set in MLSimilarityService.cs or when calling FindBestMatch():

float threshold = 0.55f; // ignore low-confidence matches

OpenAI API Key (Optional)

appsettings.json:


{
  "OpenAI": {
    "ApiKey": "your_api_key_here"
  }
}


---

✨ Future Improvements

Use sentence embeddings (OpenAI / HuggingFace) for more accurate similarity.

Add conversation memory to remember context.

Enhance personality depth (emotions, moods).

Improve frontend (chat bubbles, typing animations).



---

📜 Author
Sandra Leo
