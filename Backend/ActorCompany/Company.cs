using System.Runtime.Serialization;

namespace ActorCompany
{
    [DataContract]
    public class Company
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

        public void EditName(string name)
        {
            Name = name;
        }
    }
}
