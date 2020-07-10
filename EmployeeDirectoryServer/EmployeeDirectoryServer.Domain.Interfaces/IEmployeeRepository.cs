using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmployeeDirectoryServer.Domain.Core;

namespace EmployeeDirectoryServer.Domain.Interfaces {
    public interface IEmployeeRepository {
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<Employee> GetEmployee(int id);
        Task Insert(Employee item);
        Task Delete(int id);

        // удалить?
        void Save();
        void Update(Employee item);
        void Create(Employee item);
    }
}

