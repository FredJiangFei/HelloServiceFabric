namespace ActorCompany.Commands
{
    public class CompanyCreateCommand
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string Tel { get; set; }
    }
}
