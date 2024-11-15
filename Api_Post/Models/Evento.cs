using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Post.Models
{
    public class Evento
    {
        public int ID { get; set; }

        [Required]
        public int IDdeCuenta { get; set; }

        public Cuenta Cuenta { get; set; }

        [Required]
        public DateTime fecha_ini { get; set; }

        [Required]
        public DateTime fecha_fin { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [StringLength(150)]
        public string Descripcion { get; set; }

        [Required]
        public bool Activo { get; set; } = true;
    }

}
