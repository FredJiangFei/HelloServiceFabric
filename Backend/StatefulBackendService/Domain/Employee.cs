using System;
using System.Fabric.Management.ServiceModel;

namespace StatefulBackendService.Domain
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public int Vote { get; set; }

        public void VoteToEmployee()
        {
            Vote++;
        }

    }
}
