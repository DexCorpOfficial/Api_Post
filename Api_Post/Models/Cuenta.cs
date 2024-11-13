using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Post.Models
{
    public class Cuenta
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string genero { get; set; }

        [Required]
        [StringLength(255)]
        public string Contrasenia { get; set; }

        [Required]
        public DateTime Fecha_Creacion { get; set; } = DateTime.Now;

        [Required]
        public DateTime fecha_nac { get; set; }

        public byte[] foto_perfil { get; set; }

        [Required]
        public bool Musico { get; set; }

        [Required]
        public bool Privado { get; set; }

        public string Biografia { get; set; }

        [Required]
        public bool Activo { get; set; } = true;
    }
}
