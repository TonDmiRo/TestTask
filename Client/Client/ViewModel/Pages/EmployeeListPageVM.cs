﻿using System;
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

        public int MaxEmployeePage {
            get { return _maxEmployeePage; }
            set {
                _maxEmployeePage = value;
                OnPropertyChanged();
            }
        }
        public string SelectedEmployeePage {
            get { return _selectedEmployeePage; }
            set {
                _selectedEmployeePage = value;
                OnPropertyChanged();
            }
        }

        private ICommand _switchToEmployeePageCommand;
        public ICommand SwitchToEmployeePageCommand => _switchToEmployeePageCommand ?? ( _switchToEmployeePageCommand = new RelayCommand(SwitchToEmployeePage) );


        private ICommand _changeSelectedEmployeeCommand;
        public ICommand ChangeSelectedEmployeeCommand => _changeSelectedEmployeeCommand ?? ( _changeSelectedEmployeeCommand = new RelayCommand(ChangeSelectedEmployee) );
        private void ChangeSelectedEmployee(object parameter) {
            Employee newEmployee = new Employee
            {
                ID = SelectedEmployee.ID,
                LastName = ( string.IsNullOrWhiteSpace(NewEmployeeLastName) != true ) ? NewEmployeeLastName : SelectedEmployee.LastName,
                FirstName = ( string.IsNullOrWhiteSpace(NewEmployeeFirstName) != true ) ? NewEmployeeFirstName : SelectedEmployee.FirstName,
                MiddleName = ( string.IsNullOrWhiteSpace(NewEmployeeMiddleName) != true ) ? NewEmployeeMiddleName : SelectedEmployee.MiddleName
            };

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


        private ICommand _deleteByIdCommand;
        public ICommand DeleteByIdCommand => _deleteByIdCommand ?? ( _deleteByIdCommand = new RelayCommand(DeleteById) );
        private void DeleteById(object parameter) {
            EmployeeCollection.DeleteEmployeeById(SelectedEmployee);
            if (SelectedEmployee == null) {
                MessageBox.Show("Done!");
            }
        }




        private void SwitchToEmployeePage(object parameter) {
            int pageNum = Convert.ToInt32(SelectedEmployeePage);
            MaxEmployeePage = EmployeeCollection.PageInfo.TotalPages;

            pageNum = ( pageNum > MaxEmployeePage ) ? MaxEmployeePage : pageNum;
            SelectedEmployeePage = pageNum.ToString();
            EmployeeCollection.GetEmployeesPage(pageNum);
            MaxEmployeePage = EmployeeCollection.PageInfo.TotalPages;
            Employees = EmployeeCollection.GetResult();
        }
        #region private
        public EmployeeListPageVM(ReadOnlyObservableCollection<Employee> employees) {
            Employees = employees;
            EmployeeCollection.TotalPagesChanged += new Action<int>(InitializeProp);
        }
        public EmployeeListPageVM() {
            MaxEmployeePage = 1;

            Employees = new ReadOnlyObservableCollection<Employee>(new ObservableCollection<Employee>() {
            new Employee() { ID = 12, LastName = "LLLLLLLL", FirstName = "FFFFFFF", MiddleName = "MMMMMMM", Birthday = new DateTime(10, 10, 01) },
            new Employee() { ID = 12, LastName = "LLLLLLLL", FirstName = "FFFFFFF", MiddleName = "MMMMMMM", Birthday = new DateTime(10, 10, 01) },
            new Employee() { ID = 12, LastName = "LLLLLLLL", FirstName = "FFFFFFF", MiddleName = "MMMMMMM", Birthday = new DateTime(10, 10, 01) },
            new Employee() { ID = 12, LastName = "LLLLLLLL", FirstName = "FFFFFFF", MiddleName = "MMMMMMM", Birthday = new DateTime(10, 10, 01) }
            });
        }
        private void InitializeProp(int obj) {
            MaxEmployeePage = obj;
            int pageNum = ( string.IsNullOrEmpty(SelectedEmployeePage) ) ? 1 : Convert.ToInt32(SelectedEmployeePage);
            SelectedEmployeePage = ( pageNum > obj ) ? MaxEmployeePage.ToString() : pageNum.ToString();
        }
        private int _maxEmployeePage;
        private string _selectedEmployeePage;
        private Employee _selectedEmployee;
        #endregion
    }
}