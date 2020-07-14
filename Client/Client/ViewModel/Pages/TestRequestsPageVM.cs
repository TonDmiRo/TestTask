using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Client.Model;
using EmployeeDirectoryServer.Domain.Core;
namespace Client.ViewModel.Pages {
    internal class TestRequestsPageVM : BasePageVM {
        public string EmployeeID { get; set; }

        public TestRequestsPageVM() {
            EmployeeID = "21";
        }


        #region Commands
        private ICommand _getAllEmployeesCommand;


        public ICommand GetAllEmployeesCommand => _getAllEmployeesCommand ?? ( _getAllEmployeesCommand = new RelayCommand(GetAllEmployees) );
        private void GetAllEmployees(object parameter) {
            int id = Convert.ToInt32(EmployeeID);

            try {
                EmployeeCollection.FindEmployeeById(id);
            }
            catch (Exception) {

                throw;
            }

            Employees = EmployeeCollection.GetResult();
        }
        #endregion

        #region private

        #endregion
    }
}
