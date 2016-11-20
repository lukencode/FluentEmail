namespace FluentEmail.Core.Models
{
    public class Address
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }

        public Address()
        {            
        }

        public Address(string emailAddress, string name = null)
        {
            EmailAddress = emailAddress;
            Name = name;
        }
    }
}
