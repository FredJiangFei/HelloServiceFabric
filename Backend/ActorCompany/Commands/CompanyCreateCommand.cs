using System.Runtime.Serialization;

namespace ActorCompany.Commands
{
    [DataContract]
    public class CompanyCreateCommand
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public string Tel { get; set; }
    }
}
