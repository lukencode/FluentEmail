using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentEmail;
using System.IO;

namespace FluentEmailTests
{
    [TestClass]
    public class TemplateEmailTests
    {
        const string toEmail = "bob@test.com";
        const string fromEmail = "johno@test.com";
        const string subject = "sup dawg";

        [TestMethod]
        public void Is_Valid_With_File()
        {
            var email = Email
                        .From(fromEmail)
                        .To(toEmail)
                        .Subject(subject)
                        .UsingTemplate(@"C:\Temp\Test.txt")
                        .Replace("<%TEST1%>", "TESING FLUENTEMAIL");
            try
            {
                email.Send();
            }
            catch (FileNotFoundException ex)
            {
                //File Not found fail Test
                Assert.Fail(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                //Sending email failed, Template Test was successfull
                Assert.IsNotNull(email);
            }
        }

    }
}
