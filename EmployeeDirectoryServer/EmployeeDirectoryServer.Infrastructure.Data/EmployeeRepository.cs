using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeDirectoryServer.Domain.Core;
using EmployeeDirectoryServer.Domain.Interfaces;

namespace EmployeeDirectoryServer.Infrastructure.Data {
    public class EmployeeRepository : IEmployeeRepository {
        private readonly string _connectionString;
        public EmployeeRepository(string connectionString) {
            this._connectionString = connectionString;
        }

        public void Create(Employee item) {
            throw new NotImplementedException();
        }

        public void Delete(int id) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public IEnumerable<Employee> GetAllEmployees() {
            throw new NotImplementedException();
        }

        public Employee GetEmployee(int id) {
            throw new NotImplementedException();
        }

        public void Save() {
            throw new NotImplementedException();
        }

        public void Update(Employee item) {
            throw new NotImplementedException();
        }
    }
}