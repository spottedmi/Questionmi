using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionmiPanel.Models;
using QuestionmiPanel.Models.Home;
using QuestionmiPanel.Repositories;
using System.Diagnostics;
using System.Security.Claims;

namespace QuestionmiPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _userRepository;
        public HomeController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var uid = HttpContext.Session.GetInt32("UID");
            var credentials = HttpContext.Session.GetString("Credentials");
            if (string.IsNullOrEmpty(credentials) || uid is null)
            {
                await Logout();
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(RegisterForm registerForm)
        {
            try
            {
                _userRepository.RegisterUser(registerForm);
                return RedirectToAction("Login");
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginForm loginForm)
        {
            try
            {
                var user = _userRepository.GetUser(loginForm.Username);
                if (user is null)
                    throw new UnauthorizedAccessException();

                var principal = _userRepository.GetUserClaims(loginForm, user);

                await HttpContext.SignInAsync(principal);

                HttpContext.Session.SetInt32("UID", user.Id);
                HttpContext.Session.SetString("Credentials", $"{user.Username}");

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewBag.Error = "Nieprawidłowe dane logowania";
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}