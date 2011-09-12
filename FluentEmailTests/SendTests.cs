using FluentEmail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentEmailTests
{
    [TestClass]
    public class SendTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            string toEmail = "bob@test.com";
            string fromEmail = "johno@test.com";
            string subject = "sup dawg";
            string body = "what be the hipitity hap?";

            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body)
                .Send();
        }
    }
}
