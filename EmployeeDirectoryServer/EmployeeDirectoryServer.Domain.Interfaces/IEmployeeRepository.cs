using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmployeeDirectoryServer.Domain.Core;

namespace EmployeeDirectoryServer.Domain.Interfaces {
    public interface IEmployeeRepository : IDisposable {
        IEnumerable<Employee> GetAllEmployees();
        Employee GetEmployee(int id);
        void Delete(int id);

        void Save();
        void Update(Employee item);
        void Create(Employee item);
    }
}

