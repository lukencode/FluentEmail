using System.IO;

namespace FluentEmail.Core.Models
{
    public class Attachment
    {
        /// <summary>
        /// Gets or sets whether the attachment is intended to be used for inline images (changes the paramater name for providers such as MailGun)
        /// </summary>
        public bool IsInline { get; set; }
        public string Filename { get; set; }
        public Stream Data { get; set; }
        public string ContentType { get; set; }
        public string ContentId { get; set; }
    }
}
