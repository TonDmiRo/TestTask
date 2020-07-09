using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeDirectoryServer.Domain.Core {
    public class Employee {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime Birthday { get; set; }
    }
}
