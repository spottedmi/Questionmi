using QuestionmiPanel.Database;

namespace QuestionmiPanel.Repositories
{
    public interface ITellRepository
    {
        public Task<List<Tell>> GetUnpostedTells();
        public void ChangeTellStatus(int tellId, bool IsAccepted, int userId);
        public Task<int> CreateTell(string text, string username);
    }
}
