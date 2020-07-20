using System;
using System.Collections.Generic;
using System.Text;

namespace Client.ViewModel {
    public static class Validation {
        const int minAge = 14;
        const int maxAge = 110;
        public static bool IsValidDate(string text) {
            if (DateTime.TryParse(text, out DateTime enteredDateOfBirth)) {
                DateTime nowDate = DateTime.Today;
                int age = nowDate.Year - enteredDateOfBirth.Year;
                if (enteredDateOfBirth > nowDate.AddYears(-age)) { age--; }

                if (( minAge < age ) && ( age < maxAge )) { return true; }
                else { return false; }
            }
            else { return false; }
        }

    }
}
