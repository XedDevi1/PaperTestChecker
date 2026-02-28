using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using PaperTestChecker.Configuration;

namespace PaperTestChecker.Services;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _http;
    private readonly AiSettings _settings;
    private readonly ILogger<GeminiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private const string AnalysisPrompt = """
        You are a test-checking assistant. Analyze the uploaded photo of a paper test.

        The photo shows a test paper with questions and answer options. 
        The student has marked their chosen answers on the paper.

        Your task:
        1. Identify each question and its answer options from the photo
        2. Determine which answer the student selected/marked
        3. Determine the correct answer for each question
        4. Grade each question (correct or incorrect)
        5. Provide brief, helpful feedback for incorrect answers explaining why the correct answer is right
        6. For incorrect answers, suggest 1-3 specific reading materials or topics the student should study to improve
        7. For each question, generate exactly 4 answer options (including the correct answer and 3 plausible distractors)

        Respond ONLY with valid JSON in exactly this format (no markdown, no code fences):
        {
          "questions": [
            {
              "questionNumber": 1,
              "questionText": "The full question text",
              "studentAnswer": "What the student marked",
              "correctAnswer": "The correct answer",
              "isCorrect": true,
              "feedback": "Brief explanation (empty string if correct)",
              "recommendedReadings": ["Topic or book to study"],
              "options": ["Option A", "Option B", "Option C", "Option D"]
            }
          ],
          "totalScore": 3,
          "maxScore": 5
        }

        Important rules:
        - If you cannot read a question or answer clearly, note it in the feedback
        - recommendedReadings should be empty array [] for correct answers
        - feedback should be empty string "" for correct answers
        - totalScore is the count of correct answers
        - maxScore is the total number of questions
        - options must always contain exactly 4 items, one of which is the correctAnswer
        - options distractors should be plausible but clearly wrong
        """;

    public GeminiService(HttpClient http, IOptions<AiSettings> options, ILogger<GeminiService> logger)
    {
        _http = http;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<GeminiAnalysisResult> AnalyzeTestImageAsync(byte[] imageBytes, string mimeType)
    {
        var base64Image = Convert.ToBase64String(imageBytes);
        var dataUri = $"data:{mimeType};base64,{base64Image}";

        var requestBody = new
        {
            model = _settings.Model,
            messages = new object[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = AnalysisPrompt },
                        new
                        {
                            type = "image_url",
                            image_url = new { url = dataUri }
                        }
                    }
                }
            },
            temperature = 0.1,
            max_tokens = 4096,
            response_format = new { type = "json_object" }
        };

        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
        {
            Content = content
        };
        request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");

        _logger.LogInformation("Sending image ({Size} bytes, {MimeType}) to Groq API, model: {Model}",
            imageBytes.Length, mimeType, _settings.Model);

        const int maxRetries = 3;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            var response = await _http.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return ParseResponse(responseBody);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt < maxRetries)
            {
                var delay = attempt * 3;
                _logger.LogWarning("Rate limited (attempt {Attempt}/{Max}), retrying in {Delay}s", attempt, maxRetries, delay);
                await Task.Delay(TimeSpan.FromSeconds(delay));

                // Recreate request since HttpRequestMessage can only be sent once
                request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");
                continue;
            }

            _logger.LogError("AI API returned {StatusCode}: {Body}", response.StatusCode, responseBody);
            throw new InvalidOperationException(
                response.StatusCode == System.Net.HttpStatusCode.TooManyRequests
                    ? "AI API rate limit exceeded. Please wait a minute and try again."
                    : $"AI API error ({response.StatusCode}): {responseBody}");
        }

        throw new InvalidOperationException("AI API: max retries exceeded.");
    }

    private GeminiAnalysisResult ParseResponse(string responseBody)
    {
        using var doc = JsonDocument.Parse(responseBody);
        var root = doc.RootElement;

        var text = root
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
            ?? throw new InvalidOperationException("Empty response from AI");

        text = text.Trim();
        if (text.StartsWith("```json"))
            text = text[7..];
        if (text.StartsWith("```"))
            text = text[3..];
        if (text.EndsWith("```"))
            text = text[..^3];
        text = text.Trim();

        _logger.LogInformation("AI response: {Text}", text);

        var result = JsonSerializer.Deserialize<GeminiAnalysisResult>(text, JsonOptions)
            ?? throw new InvalidOperationException("Failed to parse AI analysis result");

        return result;
    }
}
