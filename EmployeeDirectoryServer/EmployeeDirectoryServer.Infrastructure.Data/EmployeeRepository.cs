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
        public async Task<IEnumerable<Employee>> GetEmployees1(int pageSize, int endElement) {
            int count = await Count();
            if (count <= 0) { throw new Exception("Table is empty."); }
            if (( pageSize > 0 ) && ( endElement > 0 )) {

                string sqlRequest = $"SELECT * FROM (SELECT TOP {pageSize} * FROM (" +
                    $"SELECT TOP {endElement} * FROM Employees ORDER BY ID) AS TAB1 ORDER BY ID DESC) AS TAB2 ORDER BY ID";
                using (SqlConnection connection = new SqlConnection(_connectionString)) {
                    using (SqlCommand cmd = new SqlCommand(sqlRequest, connection)) {
                        var response = new List<Employee>();
                        await connection.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync()) {
                            while (await reader.ReadAsync()) {
                                response.Add(MapToEmployee(reader));
                            }


                            return response;
                        }
                    }
                }
            }

            throw new ArgumentException();
        }
        public async Task<IEnumerable<Employee>> GetEmployees(int pageSize, int endElement) {
            int count = await Count();
            if (count <= 0) { throw new Exception("Table is empty."); }
            pageSize -= 1; // для 10 элементов TODO: https://habr.com/ru/post/54448/ 
            if (( pageSize > 0 ) && ( endElement > 0 )) {
                using (SqlConnection connection = new SqlConnection(_connectionString)) {
                    using (SqlCommand cmd = new SqlCommand("GETEmployeesPage", connection)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@pageSize",
                            Value = pageSize,
                            SqlDbType = SqlDbType.Int
                        });
                        cmd.Parameters.Add(new SqlParameter
                        {
                            ParameterName = "@endElement",
                            Value = endElement,
                            SqlDbType = SqlDbType.Int
                        });


                        var response = new List<Employee>();
                        await connection.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync()) {
                            while (await reader.ReadAsync()) {
                                response.Add(MapToEmployee(reader));
                            }
                            return response;
                        }
                    }
                }
            }
            throw new ArgumentException();
        }
        public async Task<List<Employee>> GetEmployees(string lastName, string firstName, string middleName) {
            int count = await Count();
            if (count <= 0) { throw new Exception("Table is empty."); }

            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                using (SqlCommand cmd = new SqlCommand("FindByFIO", connection)) {
                    cmd.CommandType = CommandType.StoredProcedure;
                    #region Procedure parameters
                    // @lastName
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@lastName",
                        // Value = "TRUNCATE TABLE Employees",
                        Value = lastName,
                        SqlDbType = SqlDbType.NVarChar
                    });
                    // @firstName
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@firstName",
                        Value = firstName,
                        SqlDbType = SqlDbType.NVarChar
                    });
                    // @middleName
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@middleName",
                        Value = middleName,
                        SqlDbType = SqlDbType.NVarChar
                    });
                    #endregion

                    var response = new List<Employee>();
                    await connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            response.Add(MapToEmployee(reader));
                        }
                        return response;
                    }
                }
            }
            throw new ArgumentException();
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
        public async Task<Employee> Create(Employee item) {
            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                using (SqlCommand cmd = new SqlCommand("InsertEmployee", connection)) {
                    cmd.CommandType = CommandType.StoredProcedure;
                    #region Procedure parameters
                    // @ID
                    var valueIdParameter = cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@ID",
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
                    valueIdParameter.Direction = ParameterDirection.Output;
                    await connection.OpenAsync();

                    await cmd.ExecuteNonQueryAsync();
                    var id = int.Parse(valueIdParameter.Value.ToString());
                    return await GetEmployee(id);
                }
            }
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