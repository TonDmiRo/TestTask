using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using EmployeeDirectoryServer.Domain.Core;

namespace Client.ViewModel.Pages {
    internal class BasePageVM : BaseVM {

        public ReadOnlyObservableCollection<Employee> Employees {
            get {
                return _employees;
            }
            set {
                _employees = value;

                OnPropertyChanged();
            }
        }
        public BasePageVM() {

        }
        private ReadOnlyObservableCollection<Employee> _employees;
    }
}
