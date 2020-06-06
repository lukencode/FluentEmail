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

        public override string ToString()
        {
            return Name == null ? EmailAddress : $"{Name} <{EmailAddress}>";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Address otherAddress = (Address)obj;
                return this.EmailAddress == otherAddress.EmailAddress && this.Name == otherAddress.Name;
            }
        }
    }
}
