using System.Collections.Generic;
using Newtonsoft.Json;

namespace FluentEmail.MailPace;

public class MailPaceResponse
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)] public string Id { get; set; }
    [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)] public string Status { get; set; }
    [JsonProperty("error")] public string Error { get; set; }
    [JsonProperty("errors")] public Dictionary<string, List<string>> Errors { get; set; } = new();
}