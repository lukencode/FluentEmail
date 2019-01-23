using System.Collections.Generic;

namespace FluentEmail.Core.Models
{
    public class EmailData
    {
        public List<Address> ToAddresses { get; set; }
        public List<Address> CcAddresses { get; set; }
        public List<Address> BccAddresses { get; set; }
        public List<Address> ReplyToAddresses { get; set; }
        public List<Attachment> Attachments { get; set; }
        public Address FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string PlaintextAlternativeBody { get; set; }
        public Priority Priority { get; set; }
        public List<string> Tags { get; set; }

        public bool IsHtml { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public EmailData()
        {
            ToAddresses = new List<Address>();
            CcAddresses = new List<Address>();
            BccAddresses = new List<Address>();
            ReplyToAddresses = new List<Address>();
            Attachments = new List<Attachment>();
            Tags = new List<string>();
            Headers = new Dictionary<string, string>();
        }
    }
}
