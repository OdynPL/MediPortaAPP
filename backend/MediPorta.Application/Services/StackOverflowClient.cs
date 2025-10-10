using MediPorta.Application.Interfaces;
using MediPorta.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MediPorta.Application.Services
{
    public class StackOverflowClient : IStackOverflowClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public StackOverflowClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<List<StackOverflowTag>> GetTagsAsync(int count)
        {
            var client = _httpClientFactory.CreateClient("StackOverflow");
            var apiKey = _configuration["StackOverflow:ApiKey"];
            var allTags = new List<StackOverflowTag>();
            int page = 1;

            while (allTags.Count < count)
            {
                var url = $"tags?page={page}&pagesize=100&order=desc&sort=popular&site=stackoverflow&key={apiKey}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                {
                    break;
                }

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<StackOverflowTagResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.Items == null || data.Items.Count == 0) break;

                allTags.AddRange(data.Items);

                if (!data.HasMore) 
                    break;

                page++;
                await Task.Delay(200);
            }

            return allTags.Take(count).ToList();
        }
    }
}
