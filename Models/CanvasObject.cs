using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SignalRSample.Models
{
    public class CanvasObject
    {
        [Key]
        public Guid ObjectId { get; set; }

        [Required]
        public string ObjectData { get; set; } = string.Empty;

        [Required]
        public Guid BoardId { get; set; }
    }
}
