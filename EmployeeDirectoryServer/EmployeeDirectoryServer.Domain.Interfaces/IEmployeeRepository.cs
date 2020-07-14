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
        Task<bool> EmployeeExists(int id);
        Task<Employee> GetEmployee(int id);


        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<IEnumerable<Employee>> GetEmployees(int pageSize, int endElement);
        Task<List<Employee>> GetEmployees(string lastName, string firstName, string middleName);
        // POST
        Task<Employee> Create(Employee item);

        // PUT
        Task UpdateEmployee(Employee item);

        //DELETE
        Task Delete(int id);
       
    }
}

