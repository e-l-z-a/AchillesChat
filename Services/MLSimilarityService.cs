

using AchillesChat.Models;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace AchillesChat.Services
{
    public class MLSimilarityService
    {
       
        private readonly MLContext _ml;
        private readonly ITransformer _model;
        private readonly PredictionEngine<QueryInput, QueryVector> _engine;
        private readonly List<(QAItem item, float[] vector)> _knowledgeVectors;

        private readonly List<QAItem> _chitChat;
        public MLSimilarityService(KnowledgeBaseService kb)
        {
            _ml = new MLContext();

            // Build a small dataset from Q&A text (Question + Keywords)
            var data = kb.QAPairs.Select(q => new QueryInput
            {
                Text = q.Question + " " + string.Join(" ", q.Keywords ?? Array.Empty<string>())
            });

            var trainData = _ml.Data.LoadFromEnumerable(data);

            // Text featurization -> sparse TF-IDF-ish vector
            var pipeline = _ml.Transforms.Text.FeaturizeText("Features", nameof(QueryInput.Text));

            _model = pipeline.Fit(trainData);
            _engine = _ml.Model.CreatePredictionEngine<QueryInput, QueryVector>(_model);

            // Precompute vectors for all Q&A entries
            _knowledgeVectors = kb.QAPairs
                .Select(q =>
                {
                    var vec = _engine.Predict(new QueryInput
                    {
                        Text = q.Question + " " + string.Join(" ", q.Keywords ?? Array.Empty<string>())
                    });
                    return (q, vec.Features.ToArray());
                })
                .ToList();

            //smalltalk
            _chitChat = kb.QAPairs.Where(q => q.Keywords != null && q.Keywords.Contains("smalltalk")).ToList();
        }
        public QAItem GetRandomChitChat()
        {
            if (_chitChat.Count == 0)
                return new QAItem { Answer = "I do not know how to respond.", Speaker = Speaker.Both };

            var rnd = new Random();
            return _chitChat[rnd.Next(_chitChat.Count)];
        }
        public (QAItem item, float score)? FindBestMatch(string query)
        {
            var queryVec = _engine.Predict(new QueryInput { Text = query }).Features.ToArray();

            float bestScore = 0;
            QAItem? bestItem = null;

            foreach (var (item, vec) in _knowledgeVectors)
            {
                float score = CosineSimilarity(queryVec, vec);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestItem = item;
                }
            }

            if (bestItem == null) return null;
            return (bestItem, bestScore);
        }

        private static float CosineSimilarity(float[] a, float[] b)
        {
            float dot = 0, magA = 0, magB = 0;
            var len = Math.Min(a.Length, b.Length);
            for (int i = 0; i < len; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }
            if (magA == 0 || magB == 0) return 0;
            return dot / (float)(Math.Sqrt(magA) * Math.Sqrt(magB));
        }

        public class QueryInput
        {
            public string Text { get; set; } = string.Empty;
        }

        private class QueryVector
        {
            [VectorType]
            public float[] Features { get; set; } = Array.Empty<float>();
        }
    }
}