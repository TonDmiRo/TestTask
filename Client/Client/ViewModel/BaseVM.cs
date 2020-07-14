using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
namespace Client.ViewModel {
    public abstract class BaseVM : INotifyPropertyChanged, IDisposable {

        #region Событие PropertyChanged
        /// <summary>Событие для извещения об изменения свойства</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Методы вызова события PropertyChanged
        /// <summary>Метод для вызова события извещения об изменении свойства</summary>
        /// <param name="propertyName">Изменившееся свойство</param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>Метод для вызова события извещения об изменении списка свойств</summary>
        /// <param name="propList">Список имён свойств</param>
        public void OnPropertyChanged(IEnumerable<string> propList) {
            foreach (string propertyName in propList.Where(name => !string.IsNullOrWhiteSpace(name)))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>Метод для вызова события извещения об изменении перечня свойств</summary>
        /// <param name="propList">Список имён свойств</param>
        public void OnPropertyChanged(params string[] propList) {
            foreach (string propertyName in propList.Where(name => !string.IsNullOrWhiteSpace(name)))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Метод для вызова события извещения об изменении всех свойств</summary>
        /// <param name="propList">Список свойств</param>
        public void OnAllPropertyChanged()
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        #endregion

        #region Dispose
        private bool isDisposed = false;
        public virtual void Dispose() {
            if (isDisposed)
                return;
            PropertyChanged = null;
            isDisposed = true;
        }
        #endregion
    }
}
