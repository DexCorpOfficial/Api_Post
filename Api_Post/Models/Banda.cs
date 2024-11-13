using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Post.Models
{
    public class Banda
    {
        public int ID { get; set; }

        [Required]
        public int IDdeMusico { get; set; }
        public Cuenta Musico { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(150)]
        public string Biografia { get; set; }

        [Required]
        public bool Activo { get; set; } = true;
    }
}
