using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeDirectoryServer.Domain.Core {
    public class Employee {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime Birthday { get; set; }
    }
    public class PageInfo {
        public int PageNumber { get; set; } // номер текущей страницы
        public int PageSize { get; set; } // кол-во объектов на странице
        public int TotalItems { get; set; } // всего объектов
        public int TotalPages  // всего страниц
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
    public class IndexViewModel {
        public IEnumerable<Employee> Employees { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
