using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bli.Common
{
    public class Validator
    {
        private static readonly string EmailPattern = @"^\S+@\S+\.\S+$";

       
        private static readonly string PhonePattern = @"^1[3-9]\d{9}$";

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, EmailPattern);
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, PhonePattern);
        }
    }
}
