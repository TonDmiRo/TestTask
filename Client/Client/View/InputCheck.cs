using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Client.View {
    static class InputCheck {
        const string onlyNumber = @"^[1-9]{1}\d{0,}$";
        const string onlyLetters = @"^[A-Za-zА-Яа-яёЁ]{1,}$";

        const string onlyDateTime = @"^[0-9\.-\/]*$";  // YYYY-MM-DD
        const string onlyDateTime1 = @"^[1-9]{1}[0-9]{3}-(0[1-9]|1[012])-(0[1-9]|1[0-9]|2[0-9]|3[01])$";  // YYYY-MM-DD
        const string onlyDateTime2 = @"^(0[1-9]|1[0-9]|2[0-9]|3[01])\/(0[1-9]|1[012])\/[1-9]{1}[0-9]{3}$"; // DD/MM/YYYY
        const string onlyDateTime3 = @"^(0[1-9]|1[0-9]|2[0-9]|3[01])\.(0[1-9]|1[012])\.[1-9]{1}[0-9]{3}$"; // DD.MM.YYYY

        static Regex inputRegex;// = new Regex(onlyNumber);
        internal static bool IsOnlyNumber(string str) {
            inputRegex = new Regex(onlyNumber);
            Match match = inputRegex.Match(str);
            return match.Success;
        }

        internal static bool IsOnlyLetters(string str) {
            inputRegex = new Regex(onlyLetters);
            Match match = inputRegex.Match(str);
            return match.Success;
        }

        internal static bool IsOnlyDateTime(string str) {
            inputRegex = new Regex(onlyDateTime);
            Match match = inputRegex.Match(str);
            return ( match.Success);
        }
    }
}
