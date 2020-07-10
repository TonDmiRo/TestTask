using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeDirectoryServer.Domain.Core;
using EmployeeDirectoryServer.Domain.Interfaces;
using EmployeeDirectoryServer.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EmployeeDirectoryServer.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase {
        readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IWebHostEnvironment hostEnv) {
            string connectionString = new ConfigurationBuilder().SetBasePath(hostEnv.ContentRootPath).AddJsonFile("dbsettings.json").Build().GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("No connection string in config.json");

            _employeeRepository = new EmployeeRepository(connectionString);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// Зависит от версии. https://docs.microsoft.com/ru-ru/aspnet/core/web-api/action-return-types?view=aspnetcore-3.1
        /// 3.0:|IEnumerable<Employee>
        /// 2.1: Task<IEnumerable<Employee>> || IAsyncEnumerable<Employee>
        /// </returns>
        [HttpGet]
        public async Task<IEnumerable<Employee>> Get() {
            return await _employeeRepository.GetAllEmployees();
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> Get(int id) {
            var response = await _employeeRepository.GetEmployee(id);
            if (response == null) { return NotFound(); }
            return response;
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody] Employee value) {
            await _employeeRepository.Insert(value);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task Delete(int id) {
            await _employeeRepository.Delete(id);
        }
    }
}