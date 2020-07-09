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
       
        [HttpGet]
        public async Task<IEnumerable<Employee>> Get() {
            throw new NotImplementedException();
        }
        [HttpGet("{id:int}")]
        public async Task<Employee> Get(int id) {
            throw new NotImplementedException();
        }

    }
}