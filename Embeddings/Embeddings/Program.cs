
using System.Net.Http.Json;
using System.Text.Json.Serialization;

class Program
{
    static async Task Main()
    {
        // Step 1: Read Gemini API key
        //string apiKey =
        //    Environment.GetEnvironmentVariable("GEMINI_API_KEY")
        //    ?? throw new Exception("Gemini API key not found.");

        // Use Google AI Studio to get API Key. Free plan Available.
        string apiKey = "-------------------------";

        // Step 2: Input sentences
        string[] sentences =
        {
            "I love programming.",
            "I enjoy writing code.",
            "I like playing cricket.",
            "Python is my favorite language.",
            "I develop software applications."
        };

        // Step 3: Initialize HTTP client
        using HttpClient client = new HttpClient();

        client.DefaultRequestHeaders.Add(
            "x-goog-api-key", apiKey
        );

        // Step 4: Prepare Gemini embedding request
        var request = new
        {
            requests = sentences.Select(sentence => new
            {
                model = "models/gemini-embedding-001",

                taskType = "SEMANTIC_SIMILARITY",

                content = new
                {
                    parts = new[]
                    {
                        new { text = sentence }
                    }
                }
            }).ToArray()
        };

        // Step 5: Generate embeddings
        var response = await client.PostAsJsonAsync(
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-001:batchEmbedContents",
            request
        );

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync();

            Console.WriteLine("API Error: " + error);
            return;
        }

        var result = await response.Content
            .ReadFromJsonAsync<EmbeddingResponse>();

        if (result == null ||
            result.Embeddings.Length != sentences.Length)
        {
            throw new Exception("Embedding generation failed.");
        }

        // Step 6: Extract embedding vectors
        float[][] embeddings = result.Embeddings
            .Select(x => x.Values)
            .ToArray();

        Console.WriteLine("Embeddings Generated Successfully!");

        Console.WriteLine(
            "Vector Dimensions: " + embeddings[0].Length
        );

        // Step 7: Select reference sentence
        int referenceIndex = 0;

        Console.WriteLine("\nReference Sentence:");
        Console.WriteLine(sentences[referenceIndex]);

        // Step 8: Calculate similarity
        var similarityResults = new List<SimilarityResult>();

        for (int i = 0; i < sentences.Length; i++)
        {
            double similarity = CosineSimilarity(
                embeddings[referenceIndex],
                embeddings[i]
            );

            similarityResults.Add(
                new SimilarityResult
                {
                    Sentence = sentences[i],
                    Score = similarity
                }
            );
        }

        // Step 9: Sort results by similarity
        var sortedResults = similarityResults
            .OrderByDescending(x => x.Score)
            .ToList();

        // Step 10: Display results
        Console.WriteLine("\nSimilar Sentences:\n");

        foreach (var item in sortedResults)
        {
            Console.WriteLine(item.Sentence);

            Console.WriteLine(
                "Similarity Score: " +
                item.Score.ToString("F4")
            );

            Console.WriteLine();
        }
    }

    // Calculate Cosine Similarity
    static double CosineSimilarity(float[] A, float[] B)
    {
        if (A.Length != B.Length)
            throw new ArgumentException(
                "Vectors must have equal dimensions."
            );

        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (int i = 0; i < A.Length; i++)
        {
            dotProduct += A[i] * B[i];

            magnitudeA += A[i] * A[i];

            magnitudeB += B[i] * B[i];
        }

        double denominator =
            Math.Sqrt(magnitudeA) *
            Math.Sqrt(magnitudeB);

        if (denominator == 0)
            return 0;

        return dotProduct / denominator;
    }
}

// API Response Models

public class EmbeddingResponse
{
    [JsonPropertyName("embeddings")]
    public EmbeddingData[] Embeddings { get; set; } = [];
}

public class EmbeddingData
{
    [JsonPropertyName("values")]
    public float[] Values { get; set; } = [];
}

// Similarity Result Model

public class SimilarityResult
{
    public string Sentence { get; set; } = string.Empty;

    public double Score { get; set; }
}
