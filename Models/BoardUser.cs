namespace SignalRSample.Models
{
    public class BoardUser
    {
        public Guid BoardId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public bool IsShared { get; set; }

    }
}
