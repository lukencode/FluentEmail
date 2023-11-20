using System.Collections.Generic;
using Newtonsoft.Json;

namespace FluentEmail.MailPace;

public class MailPaceSendRequest
{
    [JsonProperty("from")] public string From { get; set; }
    [JsonProperty("to")] public string To { get; set; }
    [JsonProperty("cc", NullValueHandling = NullValueHandling.Ignore)] public string Cc { get; set; }
    [JsonProperty("bcc", NullValueHandling = NullValueHandling.Ignore)] public string Bcc { get; set; }
    [JsonProperty("subject")] public string Subject { get; set; }
    [JsonProperty("htmlbody", NullValueHandling = NullValueHandling.Ignore)] public string HtmlBody { get; set; }
    [JsonProperty("textbody", NullValueHandling = NullValueHandling.Ignore)] public string TextBody { get; set; }
    [JsonProperty("replyto", NullValueHandling = NullValueHandling.Ignore)] public string ReplyTo { get; set; }
    [JsonProperty("attachments")] public List<MailPaceAttachment> Attachments { get; set; } = new(0);
    [JsonProperty("tags")] public List<string> Tags { get; set; } = new(0);
}