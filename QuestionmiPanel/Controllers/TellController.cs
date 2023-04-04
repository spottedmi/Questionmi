using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionmiPanel.Models.Tell;
using QuestionmiPanel.Repositories;
using System.Net;
using System.Runtime.CompilerServices;

namespace QuestionmiPanel.Controllers
{
    public class TellController : Controller
    {
        private IUserRepository _userRepostiory;
        private readonly ITellRepository _tellRepository;
        public TellController(IUserRepository userRepository, ITellRepository tellRepository)
        {
            _userRepostiory = userRepository;
            _tellRepository = tellRepository;
        }

        [Authorize]
        [HttpGet("Tell/Queue")]
        public async Task<IActionResult> Queue()
        {
            var tells = await _tellRepository.GetUnpostedTells();
            return View(tells);
        }

        [Authorize]
        [HttpPost("Tell/ChangeTellStatus")]
        public IActionResult ChangeTellStatus([FromBody] TellStatusForm tellStatusForm)
        {
            try
            {
                var uid = HttpContext.Session.GetInt32("UID");
                if (uid is null)
                {
                    return RedirectToAction("Logout", "Home");
                }

                _tellRepository.ChangeTellStatus(tellStatusForm.TellId, tellStatusForm.IsAccepted, uid.Value);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("{username}")]
        public IActionResult SendTell([FromRoute] string username)
        {
            var user = _userRepostiory.GetUser(username, true);
            if(user is null)
                return NotFound();

            return View(new {
                Username = user.Username
            });
        }

        [AllowAnonymous]
        [HttpPost("{username}")]
        public async Task<IActionResult> SendTell([FromRoute] string username, [FromBody] TellForm tell)
        {
            try
            {
                await _tellRepository.CreateTell(tell.Text, username);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
