using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace SignalRSample.Models
{
    public class CanvasDto
    {
        public Guid BoardId { get; set; }
        public ICollection<CanvasObject> CanvasObjects { get; set; }
        public CanvasDto()
        {
            CanvasObjects = new List<CanvasObject>();
        }

        public CanvasDto DeepCopy()
        {
            var jsonString = JsonSerializer.Serialize(this);
            return JsonSerializer.Deserialize<CanvasDto>(jsonString);
        }
    }
}
