using System.Threading.Tasks;
using FluentEmail.Core;
using NUnit.Framework;

namespace FluentEmail.Mailgun.Tests
{
    public class MailgunSenderTests
    {
        const string toEmail = "bentest1@mailinator.com";
        const string fromEmail = "ben@test.com";
        const string subject = "sup dawg";
        const string body = "what be the hipitity hap?";

        [SetUp]
        public void SetUp()
        {
            var sender = new MailgunSender("sandboxcf5f41bbf2f84f15a386c60e253b5fe9.mailgun.org", "key-8d32c046d7f14ada8d5ba8253e3e30de");
            Email.DefaultSender = sender;
        }

        [Test]
        public async Task CanSendEmail()
        {
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

            Assert.IsTrue(response.Successful);
        }
    }
}
