using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.View.Pages {
    /// <summary>
    /// Interaction logic for EmployeeListUC.xaml
    /// </summary>
    public partial class EmployeeListUC : UserControl {
        public EmployeeListUC() {
            InitializeComponent();
           
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {

            string text = ( sender as TextBox ).Text;
            if (text.Length >= 3 || !InputCheck.IsOnlyNumber(text + e.Text)) {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Space) {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewTextInputLetter(object sender, TextCompositionEventArgs e) {

            string text = ( sender as TextBox ).Text;
            if (text.Length >= 30 || !InputCheck.IsOnlyLetters(text + e.Text)) {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewTextInputDateTime(object sender, TextCompositionEventArgs e) {

            string text = ( sender as TextBox ).Text;
            if (text.Length >= 10 || !InputCheck.IsOnlyDateTime(text + e.Text)) {
                e.Handled = true;
            }
        }

        private void HandleCanExecute(object sender, CanExecuteRoutedEventArgs e) {

            if (e.Command == ApplicationCommands.Cut ||
                 e.Command == ApplicationCommands.Copy ||
                 e.Command == ApplicationCommands.Paste) {

                e.CanExecute = false;
                e.Handled = true;

            }

        }

    }
}
