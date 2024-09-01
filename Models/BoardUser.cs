using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SignalRSample.Models
{
    public class BoardUser
    {
        public string BoardId { get; set; } = string.Empty;
        public string BoardTitle { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        [DisplayName("Name")]
        public string UserName { get; set; } = string.Empty;

    }
}
