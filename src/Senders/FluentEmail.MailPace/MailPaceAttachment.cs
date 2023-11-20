using Newtonsoft.Json;

namespace FluentEmail.MailPace;

public class MailPaceAttachment
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("content")] public string Content { get; set; }
    [JsonProperty("content_type")] public string ContentType { get; set; }
    [JsonProperty("cid", NullValueHandling = NullValueHandling.Ignore)] public string Cid { get; set; }
}