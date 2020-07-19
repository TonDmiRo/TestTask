using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Client.Model;
using EmployeeDirectoryServer.Domain.Core;

namespace Client.ViewModel.Pages {
    class EmployeeListPageVM : BasePageVM {
        public Employee SelectedEmployee {
            get => _selectedEmployee; set {
                _selectedEmployee = value;
                NewEmployeeLastName = value?.LastName;
                NewEmployeeFirstName = value?.FirstName;
                NewEmployeeMiddleName = value?.MiddleName;
                NewEmployeeBirthday = value?.Birthday.ToString("d");

                OnPropertyChanged(nameof(NewEmployeeLastName), nameof(NewEmployeeFirstName), nameof(NewEmployeeMiddleName), nameof(NewEmployeeBirthday));
                OnPropertyChanged();
            }
        }
        public string NewEmployeeLastName { get; set; }
        public string NewEmployeeFirstName { get; set; }
        public string NewEmployeeMiddleName { get; set; }
        public string NewEmployeeBirthday { get; set; }

        public int LastPage {
            get { return _lastEmployeesPage; }
            set {
                _lastEmployeesPage = value;
                OnPropertyChanged();
            }
        }
        public string SelectedPage {
            get { return _selectedEmployeesPage; }
            set {
                _selectedEmployeesPage = value;
                OnPropertyChanged();
            }
        }




        private ICommand _changeSelectedEmployeeCommand;
        public ICommand ChangeSelectedEmployeeCommand => _changeSelectedEmployeeCommand ?? ( _changeSelectedEmployeeCommand = new RelayCommand(ChangeSelectedEmployee, CanChangeSelectedEmployee) );
        private void ChangeSelectedEmployee(object parameter) {
            Employee newEmployee = new Employee
            {
                ID = SelectedEmployee.ID,
                LastName = ( string.IsNullOrWhiteSpace(NewEmployeeLastName) != true ) ? NewEmployeeLastName : SelectedEmployee.LastName,
                FirstName = ( string.IsNullOrWhiteSpace(NewEmployeeFirstName) != true ) ? NewEmployeeFirstName : SelectedEmployee.FirstName,
                MiddleName = ( string.IsNullOrWhiteSpace(NewEmployeeMiddleName) != true ) ? NewEmployeeMiddleName : SelectedEmployee.MiddleName
            };
            // TODO: Month
            int laborActivity = DateTime.Now.Year - 14;
            DateTime dateTime = DateTime.Parse(NewEmployeeBirthday);
            if (( dateTime > new DateTime(1900, 1, 1) ) && ( dateTime.Year < laborActivity )) {
                newEmployee.Birthday = dateTime;
            }
            else { newEmployee.Birthday = SelectedEmployee.Birthday; }

            EmployeeCollection.UpdateEmployee(SelectedEmployee.ID, newEmployee);
            UpdateSlectedEmployee(newEmployee);
            MessageBox.Show("Done!");
        }
        private void UpdateSlectedEmployee(Employee emp) {
            SelectedEmployee.LastName = emp.LastName;
            SelectedEmployee.FirstName = emp.FirstName;
            SelectedEmployee.MiddleName = emp.MiddleName;
            SelectedEmployee.Birthday = emp.Birthday;
            SelectedEmployee = null;
        }
        private bool CanChangeSelectedEmployee(object parameter) {
            bool b1 = DateTime.TryParse(NewEmployeeBirthday, out DateTime test);
            if (( b1 ) && ( test.Year > 14 )) {
                return true;
            }
            else {
                return false;
            }
        }


        private ICommand _deleteByIdCommand;
        public ICommand DeleteByIdCommand => _deleteByIdCommand ?? ( _deleteByIdCommand = new RelayCommand(DeleteById) );
        private void DeleteById(object parameter) {
            string message = $"Вы хотите удалить сотрудника(ID: {SelectedEmployee.ID})?";
            MessageBoxResult result = MessageBox.Show(message, "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                EmployeeCollection.DeleteEmployeeById(SelectedEmployee);
            }
        }



        private ICommand _switchToEmployeePageCommand;
        public ICommand SwitchToEmployeePageCommand => _switchToEmployeePageCommand ?? ( _switchToEmployeePageCommand = new RelayCommand(SwitchToEmployeePage) );
        private void SwitchToEmployeePage(object parameter) {
            if (!string.IsNullOrWhiteSpace(SelectedPage)) {
                int pageNum = Convert.ToInt32(SelectedPage);
                LastPage = EmployeeCollection.PageInfo.TotalPages;
                pageNum = ( pageNum > LastPage ) ? LastPage : pageNum;

                EmployeeCollection.GetEmployeesPage(pageNum);
                Employees = EmployeeCollection.GetResult();

                SelectedPage = pageNum.ToString();
            }
            else {
                MessageBox.Show("Введите число!");
            }
        }



        #region ctor
        public EmployeeListPageVM(ReadOnlyObservableCollection<Employee> employees) {
            Employees = employees;
            EmployeeCollection.TotalPagesChanged += new Action<int>(TotalPagesChangeHandler);
        }
        private void TotalPagesChangeHandler(int newTotalPageCount) {
            int currentPageNumber = ( string.IsNullOrEmpty(SelectedPage) ) ? 1 : Convert.ToInt32(SelectedPage);

            LastPage = newTotalPageCount;
            if (( LastPage <= 1 ) || ( currentPageNumber >= LastPage )) {
                SelectedPage = LastPage.ToString();
            }
            else {

                SelectedPage = ( currentPageNumber == 0 ) ? "1" : currentPageNumber.ToString();
            }
        }


        public EmployeeListPageVM() {
            LastPage = 1;
            SelectedPage = "1";
#if ( DEBUG )
            Employees = new ReadOnlyObservableCollection<Employee>(new ObservableCollection<Employee>() {
            new Employee() { ID = -1, LastName = "Белозёров", FirstName = "Аввакуум", MiddleName = "Олегович", Birthday = new DateTime(1988, 01, 01) },
            new Employee() { ID = -1, LastName = "Беляев", FirstName = "Денис", MiddleName = "Александрович", Birthday = new DateTime(1977, 01, 11) },
            new Employee() { ID = -1, LastName = "Зыков", FirstName = "Мирослав", MiddleName = "Оскарович", Birthday = new DateTime(1966, 11, 10) },
            new Employee() { ID = -1, LastName = "Александров", FirstName = "Варлаам", MiddleName = "Германович", Birthday = new DateTime(1955, 11, 11) }
            });
#else
            Employees = new ReadOnlyObservableCollection<Employee>(new ObservableCollection<Employee>());
#endif

        }
        #endregion

        #region private
        private int _lastEmployeesPage;
        private string _selectedEmployeesPage;
        private Employee _selectedEmployee;
        #endregion
    }
}
