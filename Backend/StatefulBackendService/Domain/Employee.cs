using System;

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

        public Employee Copy()
        {
            return new Employee
            {
                Id = Id,
                Name = Name,
                Age = Age,
                Vote = Vote
            };
        }

        public void Edit(Employee employee)
        {
            Name = employee.Name;
            Age = employee.Age;
        }

    }
}
