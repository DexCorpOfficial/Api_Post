using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api_Post.Models
{
    public class Responde
    {
        [Required]
        public int IDdePadre { get; set; } // Relación con el comentario padre

        [JsonIgnore] // Ignorar la propiedad al serializar
        public Comentario Padre { get; set; } // Relación con el Comentario "Padre"

        [Required]
        public int IDdeHijo { get; set; } // Relación con el comentario hijo

        [JsonIgnore] // Ignorar la propiedad al serializar
        public Comentario Hijo { get; set; } // Relación con el Comentario "Hijo"
    }
}
