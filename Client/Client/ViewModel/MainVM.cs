using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Client.Model;
using Client.View;
using Client.ViewModel.Pages;
using EmployeeDirectoryServer.Domain.Core;

namespace Client.ViewModel {
    public enum PageNumEnum {
        /// <summary>Нет страницы</summary>
        None = -1,
        /// <summary>Титульная страница</summary>
        List = 0,
        /// <summary>Первая страница</summary>
        Table = 1
    }
    class MainVM : BaseVM {
        public bool IsConnected { get; set; }
        public MainVM() {
            EmployeeCollection.ThrownException += new Action<string>(ExceptionMessageHandler);
            InitializePages();
        }
        private void ExceptionMessageHandler(string str) {
            MessageBox.Show(str);
            IsConnected = false;
        }

        private ICommand _refreshContentCommand;
        public ICommand RefreshContentCommand => _refreshContentCommand ?? ( _refreshContentCommand = new RelayCommand(RefreshContent) );
        private void RefreshContent(object parameter) {
            EmployeeCollection.UpdateData();
            Content.Employees = EmployeeCollection.GetResult();
            IsConnected = true;
        }

        private ICommand _openWindowForAddingCommand;
        public ICommand OpenWindowForAddingCommand => _openWindowForAddingCommand ?? ( _openWindowForAddingCommand = new RelayCommand(OpenWindowForAdding) );
        private void OpenWindowForAdding(object parameter) {
            using (var window = new EmployeesCreationV()) {

                window.ShowDialog();
            }
            EmployeeCollection.GetEmployeesPage(1);
        }

        #region SearchById
        public string EmployeeID { get; set; }
        private ICommand _searchByIdCommand;
        public ICommand SearchByIdCommand => _searchByIdCommand ?? ( _searchByIdCommand = new RelayCommand(SearchById) );
        private void SearchById(object parameter) {
            int id = Convert.ToInt32(EmployeeID);

            EmployeeCollection.FindEmployeeById(id);
            Content.Employees = EmployeeCollection.GetResult();
        }
        #endregion

        #region FindEmployeesByFIO
        public string LastNameForSearch {
            get => lastNameForSearch; set {
                lastNameForSearch = value;
                OnPropertyChanged();
            }
        }
        public string FirstNameForSearch {
            get => firstNameForSearch; set {
                firstNameForSearch = value;
                OnPropertyChanged();
            }
        }
        public string MiddleNameForSearch {
            get => middleNameForSearch; set {
                middleNameForSearch = value;
                OnPropertyChanged();
            }
        }
        public bool ValueCheckboxForSearch { get; set; }

        private ICommand _findEmployeesByFIOCommand;
        public ICommand FindEmployeesByFIOCommand => _findEmployeesByFIOCommand ?? ( _findEmployeesByFIOCommand = new RelayCommand(FindEmployeesByFIO) );
        private void FindEmployeesByFIO(object parameter) {

            EmployeeCollection.FindEmployeesByFIO(LastNameForSearch, FirstNameForSearch, MiddleNameForSearch);
            Content.Employees = EmployeeCollection.GetResult();
            IsConnected = true;

            if (!ValueCheckboxForSearch) { LastNameForSearch = FirstNameForSearch = MiddleNameForSearch = string.Empty; }
        }
        #endregion

        #region Pages
        public BasePageVM Content {
            get => _content;
            set {
                _content = value;
                OnPropertyChanged();
            }
        }
        private readonly Dictionary<PageNumEnum, BasePageVM> dictPages = new Dictionary<PageNumEnum, BasePageVM>();
        private void InitializePages() {

            try {
                EmployeeCollection.GetEmployeesPage(1);
            }
            catch (Exception) {

                throw;
            }

            dictPages.Add(PageNumEnum.None, new TestRequestsPageVM());
            dictPages.Add(PageNumEnum.List, new EmployeeListPageVM(EmployeeCollection.GetResult()));

            Content = dictPages[PageNumEnum.List];
            SelectedPageNumEnum = PageNumEnum.List;

            Keys = dictPages.Keys;

        }

        public IEnumerable<PageNumEnum> Keys { get; private set; }
        public PageNumEnum SelectedPageNumEnum {
            get => selectedPageNumEnum;
            set {
                selectedPageNumEnum = value;
                OnGoPage(selectedPageNumEnum);
            }
        }
        private void OnGoPage(PageNumEnum page) {
            if (dictPages.ContainsKey(page)) {
                Content = dictPages[page];
            }
        }
        private PageNumEnum selectedPageNumEnum;
        private BasePageVM _content;
        private string lastNameForSearch;
        private string firstNameForSearch;
        private string middleNameForSearch;
        #endregion
    }
}
