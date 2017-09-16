namespace ActorCompany.Interfaces.Commands
{
    public class CompanyCreateCommand
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string Tel { get; set; }
    }
}
