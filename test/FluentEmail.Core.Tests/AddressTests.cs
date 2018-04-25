using NUnit.Framework;

namespace FluentEmail.Core.Tests
{
    [TestFixture]
	public class AddressTests
	{
		[Test]
		public void SplitAddress_Test()
		{
			var email = Email
				.From("test@test.com")
				.To("james@test.com;john@test.com", "James 1;John 2");

			Assert.AreEqual(2, email.Data.ToAddresses.Count);
			Assert.AreEqual("james@test.com", email.Data.ToAddresses[0].EmailAddress);
			Assert.AreEqual("john@test.com", email.Data.ToAddresses[1].EmailAddress);
			Assert.AreEqual("James 1", email.Data.ToAddresses[0].Name);
			Assert.AreEqual("John 2", email.Data.ToAddresses[1].Name);
		}

		[Test]
		public void SplitAddress_Test2()
		{
			var email = Email
                .From("test@test.com")
                .To("james@test.com; john@test.com", "James 1");

			Assert.AreEqual(2, email.Data.ToAddresses.Count);
			Assert.AreEqual("james@test.com", email.Data.ToAddresses[0].EmailAddress);
			Assert.AreEqual("john@test.com", email.Data.ToAddresses[1].EmailAddress);
			Assert.AreEqual("James 1", email.Data.ToAddresses[0].Name);
			Assert.AreEqual(string.Empty, email.Data.ToAddresses[1].Name);
		}

		[Test]
		public void SplitAddress_Test3()
		{
			var email = Email
                .From("test@test.com")
                .To("james@test.com; john@test.com;   Fred@test.com", "James 1;;Fred");

			Assert.AreEqual(3, email.Data.ToAddresses.Count);
			Assert.AreEqual("james@test.com", email.Data.ToAddresses[0].EmailAddress);
			Assert.AreEqual("john@test.com", email.Data.ToAddresses[1].EmailAddress);
			Assert.AreEqual("Fred@test.com", email.Data.ToAddresses[2].EmailAddress);
			Assert.AreEqual("James 1", email.Data.ToAddresses[0].Name);
			Assert.AreEqual(string.Empty, email.Data.ToAddresses[1].Name);
			Assert.AreEqual("Fred", email.Data.ToAddresses[2].Name);
		}

        [Test]
        public void SetFromAddress()
        {
            var email = new Email();
            email.SetFrom("test@test.test", "test");

            Assert.AreEqual("test@test.test", email.Data.FromAddress.EmailAddress);
            Assert.AreEqual("test", email.Data.FromAddress.Name);
        }

        #region Refactored tests using setup through constructor.
        [Test]
		public void New_SplitAddress_Test()
		{
			var email = new Email()
				.To("james@test.com;john@test.com", "James 1;John 2");

			Assert.AreEqual(2, email.Data.ToAddresses.Count);
			Assert.AreEqual("james@test.com", email.Data.ToAddresses[0].EmailAddress);
			Assert.AreEqual("john@test.com", email.Data.ToAddresses[1].EmailAddress);
			Assert.AreEqual("James 1", email.Data.ToAddresses[0].Name);
			Assert.AreEqual("John 2", email.Data.ToAddresses[1].Name);
		}


		[Test]
		public void New_SplitAddress_Test2()
		{
			var email = new Email()
				.To("james@test.com; john@test.com", "James 1");

			Assert.AreEqual(2, email.Data.ToAddresses.Count);
			Assert.AreEqual("james@test.com", email.Data.ToAddresses[0].EmailAddress);
			Assert.AreEqual("john@test.com", email.Data.ToAddresses[1].EmailAddress);
			Assert.AreEqual("James 1", email.Data.ToAddresses[0].Name);
			Assert.AreEqual(string.Empty, email.Data.ToAddresses[1].Name);
		}


		public void New_SplitAddress_Test3()
		{
			var email = new Email()
				.To("james@test.com; john@test.com;   Fred@test.com", "James 1;;Fred");

			Assert.AreEqual(3, email.Data.ToAddresses.Count);
			Assert.AreEqual("james@test.com", email.Data.ToAddresses[0].EmailAddress);
			Assert.AreEqual("john@test.com", email.Data.ToAddresses[1].EmailAddress);
			Assert.AreEqual("Fred@test.com", email.Data.ToAddresses[2].EmailAddress);
			Assert.AreEqual("James 1", email.Data.ToAddresses[0].Name);
			Assert.AreEqual(string.Empty, email.Data.ToAddresses[1].Name);
			Assert.AreEqual("Fred", email.Data.ToAddresses[2].Name);
		}
		#endregion
	}
}
