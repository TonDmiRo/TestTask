using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EmployeeDirectoryServer.Domain.Core;
using EmployeeDirectoryServer.Domain.Interfaces;
using EmployeeDirectoryServer.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EmployeeDirectoryServer.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase {
        #region GET
        [HttpGet]
        public async Task<ActionResult<IndexViewModel>> GetEmployees() {
            return await GetEmployeesPage(1);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id) {
            var response = await _employeeRepository.GetEmployee(id);
            if (response == null) { return NotFound(); }
            return response;
        }
        [HttpGet("/api/Employees/Page/{page:int}")]
        public async Task<ActionResult<IndexViewModel>> GetEmployeesPage(int page) {
            int totalEmployees = await _employeeRepository.Count();

            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = _pageSize, TotalItems = totalEmployees };

            if (( totalEmployees > 0 ) && ( page > 0 ) && ( page <= pageInfo.TotalPages )) {
                int endElement = page * _pageSize;
                IEnumerable<Employee> employees = await _employeeRepository.GetEmployees(_pageSize, endElement);


                IndexViewModel ivm = new IndexViewModel { PageInfo = pageInfo, Employees = employees };
                return ivm;
            }
            return NotFound();
        }

        [HttpGet("/api/Employees/Find/")]
        public async Task<ActionResult<IEnumerable<Employee>>> FindEmployeesByFIO(string lastName = "", string firstName = "", string middleName = "") {
            if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(middleName)) {

                return BadRequest();
            }

            bool str1 = IsOnlyLetters(lastName);
            bool str2 = IsOnlyLetters(firstName);
            bool str3 = IsOnlyLetters(middleName);

            if (str1 && str2 && str3) {
                return await _employeeRepository.GetEmployees(lastName, firstName, middleName);

                // https://docs.microsoft.com/ru-ru/aspnet/core/web-api/action-return-types?view=aspnetcore-2.1#actionresultt-type   
                // C# не поддерживает операторы неявных приведений в интерфейсах 
                // if _employeeRepository.GetEmployees <returns>IEnumerable<Employee></returns> :

                // var employees = await _employeeRepository.GetEmployees(lastName, firstName, middleName);
                // var list = employees.ToList();
                // return list;
            }
            else {
                return BadRequest();
            }
        }

        /// <summary>
        /// TODO: удалить?
        /// </summary>
        /// <returns>
        /// Зависит от версии. https://docs.microsoft.com/ru-ru/aspnet/core/web-api/action-return-types?view=aspnetcore-3.1
        /// 3.0:|IEnumerable<Employee>
        /// 2.1: Task<IEnumerable<Employee>> || IAsyncEnumerable<Employee>
        /// </returns>
        [HttpGet("/api/Employees/All/")]
        public async Task<IEnumerable<Employee>> GetAllEmployees() {
            return await _employeeRepository.GetAllEmployees();
        }
        #endregion

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, Employee employee) {
            if (id != employee.ID) { return BadRequest(); }
            try {
                bool idExists = await _employeeRepository.EmployeeExists(id);
                if (!idExists) {
                    return NotFound(id);
                }
                await _employeeRepository.UpdateEmployee(employee);
            }
            catch (Exception) {
                return NotFound(id);
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> Post([FromBody] Employee value) {
            return await _employeeRepository.Create(value);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(int id) {
            bool idExists = await _employeeRepository.EmployeeExists(id);
            if (!idExists) { return NotFound(id); }
            try {
                await _employeeRepository.Delete(id);
            }
            catch (Exception e) {
                return Problem(detail: e.Message);

            }
            return NoContent();
        }

        #region private
        private readonly IEmployeeRepository _employeeRepository;
        private readonly int _pageSize; // количество объектов на страницу
        public EmployeesController(IWebHostEnvironment hostEnv) {
            string connectionString = new ConfigurationBuilder().SetBasePath(hostEnv.ContentRootPath).AddJsonFile("dbsettings.json").Build().GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentException("No connection string in config.json"); }


            _employeeRepository = new EmployeeRepository(connectionString);
            _pageSize = 10;
        }

        readonly string onlyLetters = @"^[A-Za-zА-Яа-яёЁ]{0,30}$";
        private bool IsOnlyLetters(string str) {
            inputRegex = new Regex(onlyLetters);
            Match match = inputRegex.Match(str);
            return match.Success;
        }
        Regex inputRegex;
        #endregion
    }
}