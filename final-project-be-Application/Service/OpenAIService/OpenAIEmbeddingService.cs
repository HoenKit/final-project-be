using final_project_be_Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Service.OpenAIService
{
    public class OpenAIEmbeddingService : IOpenAIEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenAIEmbeddingService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["OpenAI:ApiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }
        public async Task<string> GetChatCompletionAsync(string prompt)
        {
            var request = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "user", content = prompt }
            },
                temperature = 0.7
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);
            return result.choices[0].message.content.ToString();
        }

        public async Task<string> UploadFileToOpenAIAsync(IFormFile pdfFile)
        {
            using var form = new MultipartFormDataContent();
            using var stream = pdfFile.OpenReadStream();

            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

            form.Add(fileContent, "file", pdfFile.FileName);
            form.Add(new StringContent("assistants"), "purpose"); // 🔹 phải là assistants

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/files")
            {
                Content = form
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey); // 🔹 thêm API key

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            return result.id;
        }

        public async Task<string> GenerateQuizFromPdfAsync(string fileId, int number, string difficulty)
        {
            var request = new
            {
                model = "gpt-4.1", // hoặc gpt-4o nếu bạn có
                input = new object[]
                {
                    new {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "input_text", text =
                                $"Please generate exactly {number} multiple-choice questions from this PDF. " +
                                $"Difficulty level: {difficulty}. " +
                                "Each question should include a field 'QuestionType' with one of these values: 'SingleChoice' or 'MultipleChoice'. " +
                                "Return ONLY a valid JSON array with the following structure (no explanation, no extra text, no markdown): " +
                                @"[
                                    {
                                        ""QuestionText"": ""..."",
                                        ""QuestionType"": ""SingleChoice"",
                                        ""Answers"": [
                                            { ""Text"": ""..."", ""IsCorrect"": true },
                                            { ""Text"": ""..."", ""IsCorrect"": false },
                                            { ""Text"": ""..."", ""IsCorrect"": false },
                                            { ""Text"": ""..."", ""IsCorrect"": false }
                                        ]
                                    }
                                ]"
                            },
                            new { type = "input_file", file_id = fileId }
                        }
                    }
                },
                temperature = 0.7
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses")
            {
                Content = content
            };
            httpRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(httpRequest);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI API error: {response.StatusCode} - {json}");
            }

            dynamic? result = JsonConvert.DeserializeObject(json);

            // Responses API -> output_text nằm ở result.output[0].content[0].text
            string outputText = result?.output?[0]?.content?[0]?.text?.ToString() ?? string.Empty;
            return outputText;
        }


        public async Task<float[]> GetEmbeddingAsync(string input)
        {
            var body = new
            {
                model = "text-embedding-3-small",
                input = input
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var retryPolicy = Policy
                .Handle<HttpRequestException>(ex => ex.Message.Contains("429"))
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt));

            HttpResponseMessage response = await retryPolicy.ExecuteAsync(() =>
                _httpClient.PostAsync("https://api.openai.com/v1/embeddings", content)
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);
            return result.data[0].embedding.ToObject<float[]>();
        }

    }

}
