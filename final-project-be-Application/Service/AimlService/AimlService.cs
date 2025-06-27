using final_project_be_Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace final_project_be_Application.Service.AimlService
{
	public class AimlService : IAimlService
	{
		private readonly HttpClient _httpClient;

		public AimlService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<string> GetChatResponseAsync(string userPrompt)
		{
			// API endpoint và key
			var apiUrl = "https://api.aimlapi.com/v1/chat/completions";
			var apiKey = "e5a53dcd276a4a2e883f309b8e8b375b";

			// Dữ liệu gửi lên API
			var requestData = new
			{
				model = "meta-llama/Llama-Vision-Free",
				messages = new[]
				{
				new { role = "system", content = "You are a subject matter expert and quiz creator." },
				new { role = "user", content = userPrompt }
			},
				temperature = 0.7,
				max_tokens = 256
			};

			// Gửi request đến API
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
			var response = await _httpClient.PostAsJsonAsync(apiUrl, requestData);

			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = await response.Content.ReadAsStringAsync();
				var jsonDocument = JsonDocument.Parse(jsonResponse);
				return jsonDocument.RootElement.GetProperty("choices")[0]
					.GetProperty("message")
					.GetProperty("content")
					.GetString();
			}
			else
			{
				return $"Error: {response.StatusCode}";
			}
		}
	}
}
