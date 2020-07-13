using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeDirectoryServer.Domain.Core;
using EmployeeDirectoryServer.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDirectoryServer.Infrastructure.Data {
    public class EmployeeRepository : IEmployeeRepository {
        #region GET
        public async Task<int> Count() {
            using (SqlConnection connection = new SqlConnection(_connectionString)) {

                string sqlRequest = $"SELECT COUNT(*) AS Count FROM dbo.Employees";
                using (SqlCommand cmd = new SqlCommand(sqlRequest, connection)) {

                    await connection.OpenAsync();
                    var count = await cmd.ExecuteScalarAsync();
                    int response = 0;
                    if (count is int i) {
                        response = i;
                    }
                    return response;
                }
            }
        }
        public async Task<bool> EmployeeExists(int id) {
            var emp = await GetEmployee(id);
            return emp != null;
        }
        public async Task<IEnumerable<Employee>> GetAllEmployees() {
            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                string sqlRequest = "SELECT * FROM dbo.Employees";

                using (SqlCommand cmd = new SqlCommand(sqlRequest, connection)) {
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
        public async Task<IEnumerable<Employee>> GetEmployees(int begin, int end) {
            int count = await Count();
            if (count <= 0) { throw new Exception("Table is empty."); }
            if (( begin > 0 ) && ( end > begin )) {
                if (begin < count) {
                    string sqlRequest = "SELECT * FROM dbo.Employees WHERE ID BETWEEN @begin AND @end";
                    using (SqlConnection connection = new SqlConnection(_connectionString)) {
                        using (SqlCommand cmd = new SqlCommand(sqlRequest, connection)) {

                            cmd.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@begin",
                                Value = begin,
                                SqlDbType = SqlDbType.Int
                            });
                            cmd.Parameters.Add(new SqlParameter
                            {
                                ParameterName = "@end",
                                Value = end,
                                SqlDbType = SqlDbType.Int
                            });

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
            }

            throw new NotImplementedException();
        }
        public async Task<Employee> GetEmployee(int employeeId) {
            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                string sqlRequest = "SELECT * FROM dbo.Employees WHERE ID = @employeeId";
                using (SqlCommand cmd = new SqlCommand(sqlRequest, connection)) {

                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@employeeId",
                        Value = employeeId,
                        SqlDbType = SqlDbType.Int
                    });

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
        #endregion

        // POST
        public Task<Employee> Create(Employee item) {
            //TODO: Create
            throw new NotImplementedException();
        }

        // PUT
        public async Task UpdateEmployee(Employee item) {
            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                using (SqlCommand cmd = new SqlCommand("UpdateEmployee", connection)) {
                    cmd.CommandType = CommandType.StoredProcedure;
                    #region Procedure parameters
                    // @id
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@id",
                        Value = item.ID,
                        SqlDbType = SqlDbType.Int
                    });
                    // @lastName
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@lastName",
                       // Value = "TRUNCATE TABLE Employees",
                        Value = item.LastName,
                        SqlDbType = SqlDbType.NVarChar
                    });
                    // @firstName
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@firstName",
                        Value = item.FirstName,
                        SqlDbType = SqlDbType.NVarChar
                    });
                    // @middleName
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@middleName",
                        Value = item.MiddleName,
                        SqlDbType = SqlDbType.NVarChar
                    });
                    // @birthday
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@birthday",
                        Value = item.Birthday,
                        SqlDbType = SqlDbType.Date
                    });
                    #endregion

                    await connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return;
                }
            }
        }
       
        public async Task Delete(int employeeId) {
            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                string sqlRequest = $"DELETE FROM dbo.Employees WHERE ID = @employeeId";
                using (SqlCommand cmd = new SqlCommand(sqlRequest, connection)) {
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@employeeId",
                        Value = employeeId,
                        SqlDbType = SqlDbType.Int
                    });

                    await connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return;
                }
            }
        }
        #region private
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

        private readonly string _connectionString;
        public EmployeeRepository(string connectionString) {
            this._connectionString = connectionString;
        }
        #endregion
    }
}