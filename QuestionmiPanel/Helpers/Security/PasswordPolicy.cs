using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionmiPanel.Helpers.Security
{
    public class PasswordPolicy
    {
        private const string _allowedCharacters = "qwertyuiopasdfghjklzxcvbnm1234567890!@#$%&";
        private const string _specialCharacters = "!@#$%&";
        private const bool _requiredUppercase = true;
        private const bool _requredSpecialCharacters = true;
        private const bool _requiredNumber = true;
        private const int _minPasswordLength = 8;
        private const int _maxPasswordLength = 32;

        public static bool VerifyPassword(string password)
        {
            if (password.Length < _minPasswordLength || password.Length > _maxPasswordLength) return false;
            if (password.ToLower().Any(c => !_allowedCharacters.Contains(c))) return false;

            if (_requiredUppercase && password.All(c => char.IsLower(c))) return false;
            if (_requredSpecialCharacters && !password.Any(c => _specialCharacters.Contains(c))) return false;
            if (_requiredNumber && !password.Any(c => char.IsNumber(c))) return false;

            return true;
        }
    }
}
