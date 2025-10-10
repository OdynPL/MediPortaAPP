using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediPorta.Infrastructure.Models
{
    public class StackOverflowTagResponse
    {
        [JsonPropertyName("items")]
        public List<StackOverflowTag> Items { get; set; } = new();

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("quota_remaining")]
        public int QuotaRemaining { get; set; }
    }
}
