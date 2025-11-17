using System.Text.Json.Serialization;

namespace PlaywrightAutomation.Models
{
    public class GetTransactionByLoginNameRequest
    {
        [JsonPropertyName("subscriberID")]
        public int SubscriberID { get; set; }
        
        [JsonPropertyName("loginName")]
        public string LoginName { get; set; } = string.Empty;
        
        [JsonPropertyName("batchSize")]
        public int BatchSize { get; set; }
        
        [JsonPropertyName("fromDate")]
        public string FromDate { get; set; } = string.Empty;
        
        [JsonPropertyName("toDate")]
        public string ToDate { get; set; } = string.Empty;
    }
}
