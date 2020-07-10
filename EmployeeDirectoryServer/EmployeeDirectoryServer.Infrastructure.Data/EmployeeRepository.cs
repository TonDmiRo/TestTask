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
        public async Task<IEnumerable<Employee>> GetAllEmployees() {
            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Employees", connection)) {
                    // cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    var response = new List<Employee>();
                    await connection.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            response.Add(MapToEmployee(reader));
                        }
                    }

                    return response;
                }
            }
        }

        public async Task<Employee> GetEmployee(int id) {
            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM dbo.Employees WHERE ID = {id}", connection)) {
                    // cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    // cmd.Parameters.Add(new SqlParameter("@Id", Id));
                    Employee response = null;
                    await connection.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            response = MapToEmployee(reader);
                        }
                    }
                    return response;
                }
            }
        }

        public Task Delete(int id) {
            throw new NotImplementedException();
        }
        public Task Insert(Employee item) {
            throw new NotImplementedException();
        }

        private Employee MapToEmployee(SqlDataReader reader) {
            //int ID = (int)reader["ID"];
            //string LastName = reader["LastName"].ToString();
            //string FirstName = reader["FirstName"].ToString();
            //string MiddleName = reader["MiddleName"].ToString();
            //DateTime Birthday = (DateTime)reader["Birthday"];


            return new Employee()
            {
                ID = (int)reader["ID"],
                LastName = reader["LastName"].ToString(),
                FirstName = reader["FirstName"].ToString(),
                MiddleName = reader["MiddleName"].ToString(),
                Birthday = (DateTime)reader["Birthday"]
            };
        }

        // удалить?
        public void Create(Employee item) {
            throw new NotImplementedException();
        }
        public void Save() {
            throw new NotImplementedException();
        }
        public void Update(Employee item) {
            throw new NotImplementedException();
        }

        private readonly string _connectionString;
        public EmployeeRepository(string connectionString) {
            this._connectionString = connectionString;
        }
    }
}