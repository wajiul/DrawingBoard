using SignalRSample.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DrawingBoard.Models
{
    public class BoardModel
    {
        public string BoardId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class BoardCanvasModel
    {
        public string BoardId { get; set; } = string.Empty;
        public ICollection<CanvasObject> CanvasObjects { get; set; } = new List<CanvasObject>();
    }

    public class BoardWithAuthorModel: BoardModel
    {
        public string Author { get; set; } = string.Empty;
        public string Email { get;set; } = string.Empty;    
    }
}
