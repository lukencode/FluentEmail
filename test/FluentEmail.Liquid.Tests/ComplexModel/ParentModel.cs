using System.Collections.Generic;

namespace FluentEmail.Liquid.Tests.ComplexModel
{
    public class ParentModel
    {
        public string Id { get; set; }
        public NameDetails ParentName { get; set; }
        public List<NameDetails> ChildrenNames { get; set; } = new List<NameDetails>();
    }

    public class NameDetails
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
    }
}