using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using FluentEmail;

namespace FluentEmailTests
{
    [TestFixture]
    public class AddressTests
    {
        [Test]
        public void SplitAddress_Test()
        {
        	var email = new Email()
        		.To("james@test.com;john@test.com", "James 1;John 2");
        	
            Assert.AreEqual(2, email.Message.To.Count);
            Assert.AreEqual("james@test.com", email.Message.To[0].Address);
            Assert.AreEqual("john@test.com", email.Message.To[1].Address);
            Assert.AreEqual("James 1", email.Message.To[0].DisplayName);
            Assert.AreEqual("John 2", email.Message.To[1].DisplayName);
        }

        [Test]
        public void SplitAddress_Test2()
        {
        	var email = new Email()
                        .To("james@test.com; john@test.com", "James 1");

            Assert.AreEqual(2, email.Message.To.Count);
            Assert.AreEqual("james@test.com", email.Message.To[0].Address);
            Assert.AreEqual("john@test.com", email.Message.To[1].Address);
            Assert.AreEqual("James 1", email.Message.To[0].DisplayName);
            Assert.AreEqual(string.Empty, email.Message.To[1].DisplayName);
        }

        [Test]
        public void SplitAddress_Test3()
        {
        	var email = new Email()
                        .To("james@test.com; john@test.com;   Fred@test.com", "James 1;;Fred");

            Assert.AreEqual(3, email.Message.To.Count);
            Assert.AreEqual("james@test.com", email.Message.To[0].Address);
            Assert.AreEqual("john@test.com", email.Message.To[1].Address);
            Assert.AreEqual("Fred@test.com", email.Message.To[2].Address);
            Assert.AreEqual("James 1", email.Message.To[0].DisplayName);
            Assert.AreEqual(string.Empty, email.Message.To[1].DisplayName);
            Assert.AreEqual("Fred", email.Message.To[2].DisplayName);
        }
    }
}
