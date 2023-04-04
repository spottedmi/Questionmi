using QuestionmiPanel.Database;
using QuestionmiPanel.Models.Home;
using System.Security.Claims;

namespace QuestionmiPanel.Repositories
{
    public interface IUserRepository
    {
        public int RegisterUser(RegisterForm registerForm);
        public User? GetUser(string username, bool isMain = false);
        public ClaimsPrincipal GetUserClaims(LoginForm loginForm, User user);
    }
}
