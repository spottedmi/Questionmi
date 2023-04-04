using System;
using System.Text.Json.Serialization;

namespace QuestionmiPanel.Database
{
    public class Tell
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UsersIP { get; set; }
        public string Text { get; set; }
        public bool IsPosted { get; set; }
        public bool IsWaitingForAccept { get; set; }
        public int userId { get; set; }
    }
}
