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

        #region Event
        public static event Action<string> ThrownException;
        private static void ThrowException(string e) {
            ThrownException?.Invoke(e);
        }
        public static event Action<int> TotalPagesChanged;
        private static void OnTotalPagesChanged(int e) {
            TotalPagesChanged?.Invoke(e);
        }
        #endregion
        internal static async void GetEmployeesPage(int page) {
            _employees.Clear();

            try {
                var response = await HttpHelper.RequestGetAsync<IndexViewModel>(client, $"api/Employees/Page/{page}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    IndexViewModel index = response.Result;
                    PageInfo = index.PageInfo;
                    OnTotalPagesChanged(PageInfo.TotalPages);
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
        internal static async void FindEmployeeById(int id) {
            _employees.Clear();
            try {
                var response = await HttpHelper.RequestGetAsync<Employee>(client, $"api/Employees/{id}");

                Employee employee = response.Result;
                if (employee != null) {
                    _employees.Add(employee);
                }
            }
            catch (Exception e) {
                ThrowException(e.Message);
            }
        }
        internal static async void FindEmployeesByFIO(string lastName,string firstName,string middleName) {
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



        internal async static void UpdateEmployee(int id, Employee newEmployee) {
            try {
                var response = await HttpHelper.RequestPutAsync(client, $"api/Employees/{id}", newEmployee);
            }
            catch (Exception e) {
                ThrowException(e.Message);
            }
        }

        internal async static void InsertEmployee(Employee empl) {
            try {
                var response = await HttpHelper.RequestPostAsync<Employee>(client, $"api/Employees/", empl);
                _employees.Clear();
                Employee employee = response.Result;
                if (employee != null) {
                    _employees.Add(employee);
                }
            }
            catch (Exception e) {
                ThrowException(e.Message);
            }
        }
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

        #region private
        private static ReadOnlyObservableCollection<Employee> Result;

        private static readonly ObservableCollection<Employee> _employees;
        static EmployeeCollection() {
            _employees = new ObservableCollection<Employee>();
            InitializeHttpClient();
        }

        private static HttpClient client;
        private static void InitializeHttpClient() {
            client = new HttpClient();
            string baseAddress;
#if ( DEBUG)
            string str = ConfigurationManager.AppSettings["LocalHost"];
            baseAddress = str != null ? str : "http://localhost:56104/"; // для View 
#else
            baseAddress = ConfigurationManager.AppSettings["LocalHost"];
#endif
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion
    }
}
