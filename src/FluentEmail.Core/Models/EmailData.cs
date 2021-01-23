using System.Collections.Generic;

namespace FluentEmail.Core.Models
{
    public class EmailData
    {
        public IList<Address> ToAddresses { get; set; }
        public IList<Address> CcAddresses { get; set; }
        public IList<Address> BccAddresses { get; set; }
        public IList<Address> ReplyToAddresses { get; set; }
        public IList<Attachment> Attachments { get; set; }
        public Address FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string PlaintextAlternativeBody { get; set; }
        public Priority Priority { get; set; }
        public IList<string> Tags { get; set; }

        public bool IsHtml { get; set; }
        public IDictionary<string, string> Headers { get; set; }

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
