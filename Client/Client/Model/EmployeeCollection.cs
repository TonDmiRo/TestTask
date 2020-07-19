using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Windows.Controls;
using EmployeeDirectoryServer.Domain.Core;
using System.Net;

namespace Client.Model {
    internal static class EmployeeCollection {
        static public PageInfo PageInfo { get; private set; }
        public static ReadOnlyObservableCollection<Employee> GetResult() {
            if (Result == null) {
                Result = new ReadOnlyObservableCollection<Employee>(_employees);
            }
            return Result;
        }


        #region GET
        internal static void UpdateData() {
            if (PageInfo != null) {
                GetEmployeesPage(PageInfo.PageNumber);
            }
            else {
                GetEmployeesPage(1);
            }
        }
        internal static async void GetEmployeesPage(int page) {
            _employees.Clear();
            if (page>=1) {
                try {
                    var response = await HttpHelper.RequestGetAsync<IndexViewModel>(client, $"api/Employees/Page/{page}");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                        IndexViewModel index = response.Result;

                        int newTotalPageCount = index.PageInfo.TotalPages;
                        if (( PageInfo == null ) || ( PageInfo.TotalPages != newTotalPageCount )) {
                            OnTotalPagesChanged(newTotalPageCount);
                        }
                        PageInfo = index.PageInfo;

                        foreach (var item in index.Employees) {
                            _employees.Add(item);
                        }
                    }
                    else {
                        throw new FileNotFoundException("Page not found");
                    }

                }
                catch (Exception e) {
                    ThrowException(e.Message);
                }
            }
            else {ThrowException("Page not found");}
        }
        internal static async void FindEmployeeById(int id) {
            _employees.Clear();
            try {
                var response = await HttpHelper.RequestGetAsync<Employee>(client, $"api/Employees/{id}");

                bool resultIsNotNull = (response.Result != null);
                if (resultIsNotNull) { _employees.Add(response.Result); }
                OnTotalPagesChanged(( resultIsNotNull ) ? 1 : 0);
            }
            catch (Exception e) {
                ThrowException(e.Message);
            }
        }
        internal static async void FindEmployeesByFIO(string lastName, string firstName, string middleName) {
            _employees.Clear();

            try {
                string str1 = ( string.IsNullOrWhiteSpace(lastName) ) ? "" : $"&lastName={lastName}";
                string str2 = ( string.IsNullOrWhiteSpace(firstName) ) ? "" : $"&firstName={firstName}";
                string str3 = ( string.IsNullOrWhiteSpace(middleName) ) ? "" : $"&middleName={middleName}";

                string url = $"api/Employees/Find/?{str1}{str2}{str3}";
                var response = await HttpHelper.RequestGetAsync<IEnumerable<Employee>>(client, url);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    OnTotalPagesChanged(1);
                    foreach (var item in response.Result) {
                        _employees.Add(item);
                    }
                }
                else {
                    throw new FileNotFoundException("Page not found");
                }

            }
            catch (Exception e) {
                ThrowException(e.Message);
            }
        }
        #endregion

        // PUT
        internal async static void UpdateEmployee(int id, Employee newEmployee) {
            try {
                var response = await HttpHelper.RequestPutAsync(client, $"api/Employees/{id}", newEmployee);
            }
            catch (Exception e) {
                ThrowException(e.Message);
            }
        }
        // POST
        static object _sender;
        internal async static void InsertEmployee(object sender,Employee empl) {
            try {
                if (!ReferenceEquals(_sender, sender)) {
                    _sender = sender;
                    _employees.Clear();
                }

                var response = await HttpHelper.RequestPostAsync<Employee>(client, $"api/Employees/", empl);
                Employee employee = response.Result;
                if (employee != null) {
                    _employees.Add(employee);
                }
            }
            catch (Exception e) {
                ThrowException(e.Message);
            }
        }
        // DELETE
        internal async static void DeleteEmployeeById(Employee empl) {
            try {

                var response = await HttpHelper.RequestDeleteAsync(client, $"api/Employees/{empl.ID}");

                if (response.StatusCode != HttpStatusCode.NoContent) {
                    throw new Exception("Не удалось удалить");
                }
                _employees.Remove(empl);
            }
            catch (Exception e) {
                ThrowException(e.Message);
            }
        }


        #region Event
        public static event Action<string> ThrownException;
        private static void ThrowException(string e) {
            ThrownException?.Invoke(e);
        }

        public static event Action<int> TotalPagesChanged;
        /// <summary>
        /// Необходим для UC
        /// </summary>
        /// <param name="totalPages">
        /// totalPages > 0 - Ok;
        /// totalPages = 0 - поиск не удался;
        /// </param>
        private static void OnTotalPagesChanged(int totalPages) {
            if (totalPages < 0) { totalPages = 0; }
            TotalPagesChanged?.Invoke(totalPages);
        }
        #endregion

        #region ctor
        static EmployeeCollection() {
            _employees = new ObservableCollection<Employee>();
            InitializeHttpClient();
        }
        private static void InitializeHttpClient() {
            client = new HttpClient();
            string baseAddress;

            string str = ConfigurationManager.AppSettings["LocalHost"];
            baseAddress = str ?? "http://localhost:5000/"; 
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        #region private
        private static HttpClient client;

        private static ReadOnlyObservableCollection<Employee> Result;
        private static readonly ObservableCollection<Employee> _employees;
        #endregion
    }
}
