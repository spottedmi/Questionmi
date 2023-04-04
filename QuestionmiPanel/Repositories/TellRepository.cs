using Microsoft.EntityFrameworkCore;
using QuestionmiPanel.Database;
using System.Text.RegularExpressions;

namespace QuestionmiPanel.Repositories
{
    public class TellRepository : ITellRepository
    {
        ApplicationDbContext _context;
        IUserRepository _userRepository;
        public TellRepository(ApplicationDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<List<Tell>> GetUnpostedTells()
        {
            var unpostedTells = await _context.Tells.Where(x => !x.IsPosted && x.IsWaitingForAccept)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

            return unpostedTells;
        }

        public void ChangeTellStatus(int tellId, bool IsAccepted, int userId)
        {
            int ownerId;
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            if (user.OwnerId is not null)
                ownerId = user.OwnerId.Value;
            else
                ownerId = user.Id;

            var tell = _context.Tells.Where(x => x.Id == tellId && x.userId == ownerId).FirstOrDefault();
            if (tell is null) throw new InvalidDataException("Tell o podanym id nie istnieje");

            if (IsAccepted)
            {
                tell.IsWaitingForAccept = false;
                _context.Update(tell);
            }
            else
            {
                _context.Remove(tell);
            }

            _context.SaveChanges();
        }

        public async Task<int> CreateTell(string text, string username)
        {
            var user = _userRepository.GetUser(username);
            if(user is null)
                throw new Exception("User not found.");

            if (text.Length <= 5)
                throw new Exception("Tell must have more than 5 characters.");

            if (text.Length > 250)
                throw new Exception("Tell can't have more than 250 characters.");

            if (!Regex.IsMatch(text, "^[a-zA-z0-9 ĄąĘęĆćŁłŃńŚśÓóŹźŻż,.!#$%&*()-={}\"\'|?>]+$"))
                throw new Exception("Tell has illegall characters.");

            var similiarTellPosted = _context.Tells
                .Any(t => t.Text.ToLower().Replace(" ", "") == text.ToLower().Replace(" ", ""));

            if (similiarTellPosted)
                throw new Exception("Similar tell was posted.");

            var mappedTell = new Tell
            {
                CreatedAt = DateTime.Now,
                UsersIP = "",
                Text = text
            };

            var badWords = _context.BadWords.Where(x => x.UserId == user.Id)
            .Select(x => x.Words)
            .FirstOrDefault();

            if(badWords is not null)
            {
                var badWordsList = badWords.Split(',');

                //method1 doesn't work when words list are long. Idk why
                //var badWordsRegex = new Regex(@"\b(" + string.Join("|", badWordsList.Select(Regex.Escape).ToArray()) + @"\b)");
                
                //if (Regex.IsMatch(text, badWordsRegex.ToString()))
                    //mappedTell.IsWaitingForAccept = true;

                //method2
                var hasBadWord = text.Split(' ').Any(x => badWordsList.Contains(x));
                if(hasBadWord)
                    mappedTell.IsWaitingForAccept = true;
            }

            _context.Tells.Add(mappedTell);
            await _context.SaveChangesAsync();

            return mappedTell.Id;
        }

        public async Task Update(Tell tell)
        {
            var tellInDb = _context.Tells
                .Where(t => t.Id == tell.Id)
                .FirstOrDefault();

            if (tellInDb == null)
                throw new Exception($"Tell with id {tell.Id} not found");

            _context.Entry(tell).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
