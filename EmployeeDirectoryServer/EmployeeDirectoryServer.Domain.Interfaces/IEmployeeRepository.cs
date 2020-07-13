using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmployeeDirectoryServer.Domain.Core;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDirectoryServer.Domain.Interfaces {
    public interface IEmployeeRepository {
        //GET
        Task<int> Count();
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<IEnumerable<Employee>> GetEmployees(int begin, int end);
        Task<Employee> GetEmployee(int id);
        Task<bool> EmployeeExists(int id);
        // POST
        Task<Employee> Create(Employee item);

        // PUT
        Task UpdateEmployee(Employee item);

        //DELETE
        Task Delete(int id);
       
    }
}

