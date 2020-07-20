using System;
using System.Windows.Input;

using Client.Model;
using Client.ViewModel.Pages;

using EmployeeDirectoryServer.Domain.Core;

namespace Client.ViewModel {
    internal class EmployeesCreationVM : BaseVM {
        public EmployeesCreationVM() {
            Content = new EmployeeListPageVM();
        }
        public BasePageVM Content { get; private set; }
        public string NewEmployeeLastName { get; set; }
        public string NewEmployeeFirstName { get; set; }
        public string NewEmployeeMiddleName { get; set; }
        public string NewEmployeeBirthday { get; set; }

        private ICommand _insertEmployeeCommand;
        public ICommand InsertEmployeeCommand => _insertEmployeeCommand ?? ( _insertEmployeeCommand = new RelayCommand(InsertEmployee, CanInsertEmployee) );
        private void InsertEmployee(object parameter) {
            Employee newEmployee = new Employee
            {
                ID = -1,
                LastName = NewEmployeeLastName,
                FirstName = NewEmployeeFirstName,
                MiddleName = NewEmployeeMiddleName,
                Birthday = DateTime.Parse(NewEmployeeBirthday)
            };
            EmployeeCollection.InsertEmployee(this, newEmployee);
            Content.Employees = EmployeeCollection.GetResult();
        }
        private bool CanInsertEmployee(object parameter) {
            return Validation.IsValidDate(NewEmployeeBirthday);
            // TODO: FIO > char[2];
        }
    }
}
