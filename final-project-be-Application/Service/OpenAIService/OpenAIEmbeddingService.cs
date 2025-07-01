using final_project_be_Application.Interface;
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
