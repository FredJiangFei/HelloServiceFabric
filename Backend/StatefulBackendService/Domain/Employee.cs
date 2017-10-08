using System;

namespace StatefulBackendService.Domain
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public int Vote { get; set; }

    }
}
