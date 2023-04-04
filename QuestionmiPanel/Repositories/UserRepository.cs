using Microsoft.AspNetCore.Authentication.Cookies;
using QuestionmiPanel.Database;
using QuestionmiPanel.Helpers.Security;
using QuestionmiPanel.Models.Home;
using System.Security.Claims;

namespace QuestionmiPanel.Repositories
{
    public class UserRepository : IUserRepository
    {
        ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public int RegisterUser(RegisterForm registerForm)
        {
            if (registerForm.Password != registerForm.PasswordRepeat) throw new InvalidDataException("Podane hasła są różne");
            if (!PasswordPolicy.VerifyPassword(registerForm.Password)) throw new InvalidDataException("Hasło nie spełnia wymagań");
            if (_context.Users.Any(u => u.Username == registerForm.Username)) throw new InvalidDataException("Konto z tym adresem email już istnieje");

            var accessCode = _context.AccessCodes.Where(c => c.Code == registerForm.AccessCode).FirstOrDefault();
            if (accessCode is null) throw new InvalidDataException("Niepoprawny kod dostępu");

            var newUser = new User
            {
                Username = registerForm.Username,
                Password = PasswordHasher.Hash(registerForm.Password),
                OwnerId = accessCode.UserId
            };

            _context.Add(newUser);
            _context.Remove(accessCode);
            _context.SaveChanges();

            return newUser.Id;
        }

        public User? GetUser(string username, bool isMain)
        {
            if(isMain)
                return _context.Users.Where(x => (x.Username == username || x.Username.ToLower().Replace(" ", "_") == username) && x.OwnerId == null).FirstOrDefault();

            return _context.Users.Where(x => x.Username == username || x.Username.ToLower().Replace(" ", "_") == username).FirstOrDefault();
        }

        public ClaimsPrincipal GetUserClaims(LoginForm loginForm, User user)
        {
            if (!PasswordHasher.Verify(user.Password, loginForm.Password))
                throw new UnauthorizedAccessException();

            var claims = new List<Claim> { new Claim("Authorized", loginForm.Username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }
    }
}
