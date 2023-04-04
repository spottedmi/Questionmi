using System.Security.Cryptography;

namespace QuestionmiPanel.Helpers.Security
{
    public class PasswordHasher
    {
        private const int _iterations = 10000;
        private const int _saltLength = 20;

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(_saltLength);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _iterations);
            var hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[40];
            Array.Copy(salt, 0, hashBytes, 0, _saltLength);
            Array.Copy(hash, 0, hashBytes, _saltLength, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool Verify(string hash, string input)
        {
            byte[] hashBytes = Convert.FromBase64String(hash);

            byte[] salt = new byte[_saltLength];
            Array.Copy(hashBytes, 0, salt, 0, _saltLength);

            var pbkdf2 = new Rfc2898DeriveBytes(input, salt, _iterations);
            byte[] result = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
                if (hashBytes[i + _saltLength] != result[i])
                    return false;

            return true;
        }
    }
}
