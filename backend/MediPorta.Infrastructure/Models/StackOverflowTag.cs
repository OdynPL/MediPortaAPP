using System.Text.Json.Serialization;

namespace MediPorta.Infrastructure.Models
{
    public class StackOverflowTag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
