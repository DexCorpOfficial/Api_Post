using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api_Post.Models
{
    public class Comentario
    {
        public int ID { get; set; }

        [Required]
        public int IDdePost { get; set; }

        [JsonIgnore] // Ignorar la propiedad al serializar
        public Post Post { get; set; }  // Relación con el Post

        [Required]
        public int IDdeCuenta { get; set; }

        [JsonIgnore] // Ignorar la propiedad al serializar
        public Cuenta Cuenta { get; set; }  // Relación con la Cuenta

        [Required]
        [StringLength(200)] // Limitar el contenido del comentario a 200 caracteres
        public string Contenido { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public bool Activo { get; set; } = true; // Valor por defecto para la columna "activo"
    }
}