using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalRSample.Models
{
    public class Board
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BoardId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public ICollection<CanvasObject> CanvasObjects { get; set; } = new List<CanvasObject>();

        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
